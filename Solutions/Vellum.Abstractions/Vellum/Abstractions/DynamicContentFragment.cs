// <copyright file="DynamicContentFragment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ImpromptuInterface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Converters;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Vellum.Abstractions;

/// <summary>
/// A dynamic view over a <see cref="ContentFragment"/>'s metadata that coerces values to the property
/// types declared by the target interfaces the fragment is duck-typed onto.
/// </summary>
public class DynamicContentFragment : DynamicObject
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, Type>> TargetPropertyMapCache = new();

    private static readonly Dictionary<string, PropertyInfo> ContentFragmentProperties =
        typeof(ContentFragment)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(property => property.Name, property => property, StringComparer.OrdinalIgnoreCase);

    private static readonly ISerializer NestedObjectSerializer = new SerializerBuilder().Build();

    private static readonly IDeserializer NestedObjectDeserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().IgnoreFields().Build();

    private readonly ContentFragment contentFragment;
    private readonly Dictionary<string, dynamic> dictionary;
    private readonly IServiceProvider serviceProvider;
    private readonly Dictionary<string, Type> targetProperties;
    private readonly ILogger? logger;
    private readonly bool strictMemberAccess;

    private List<CoercionFailure>? failureCollector;

    public DynamicContentFragment(ContentFragment contentFragment, IServiceProvider serviceProvider, params Type[] targetTypes)
    {
        this.contentFragment = contentFragment;
        this.serviceProvider = serviceProvider;
        this.dictionary = new Dictionary<string, dynamic>(contentFragment.MetaData, StringComparer.OrdinalIgnoreCase);
        this.targetProperties = GetTargetPropertyMap(targetTypes);
        this.logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<DynamicContentFragment>();
        this.strictMemberAccess = serviceProvider.GetService<ContentExtensibilityOptions>()?.StrictMemberAccess ?? false;
    }

    public int Count
    {
        get
        {
            return this.dictionary.Count;
        }
    }

    private string? FilePath
    {
        get
        {
            return this.dictionary.TryGetValue("FilePath", out object? filePath) ? filePath?.ToString() : null;
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        bool fromMetaData = this.dictionary.TryGetValue(binder.Name, out object? raw);

        if (!fromMetaData)
        {
            if (ContentFragmentProperties.TryGetValue(binder.Name, out PropertyInfo? property))
            {
                raw = property.GetValue(this.contentFragment, null);
            }
            else if (this.strictMemberAccess && !this.targetProperties.ContainsKey(binder.Name))
            {
                // The member exists nowhere: not in the metadata, not on ContentFragment, and not on any
                // target interface. In strict mode fail the binding so template typos surface loudly.
                result = null;

                return false;
            }
        }

        this.targetProperties.TryGetValue(binder.Name, out Type? targetType);

        result = this.Coerce(binder.Name, raw, targetType);

        if (fromMetaData && !ReferenceEquals(result, raw))
        {
            // Memoize the coerced value so repeated reads (templates touch the same property many times)
            // don't re-run converters or re-materialize lists. Local to this view, like all writes.
            this.dictionary[binder.Name] = result!;
        }

        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        // Writes are local to this view; the underlying (potentially cached) ContentFragment is never mutated.
        this.dictionary[binder.Name] = value!;

        return true;
    }

    /// <summary>
    /// Attempts to coerce every member declared by the target interfaces, without waiting for the members
    /// to be accessed during rendering, and reports every failure rather than stopping at the first.
    /// </summary>
    /// <returns>The coercion failures; empty when every member coerces cleanly.</returns>
    public IReadOnlyList<CoercionFailure> Validate()
    {
        List<CoercionFailure> failures = [];
        this.failureCollector = failures;

        try
        {
            foreach ((string memberName, Type targetType) in this.targetProperties)
            {
                if (!this.dictionary.TryGetValue(memberName, out object? raw)
                    && ContentFragmentProperties.TryGetValue(memberName, out PropertyInfo? property))
                {
                    raw = property.GetValue(this.contentFragment, null);
                }

                try
                {
                    this.Coerce(memberName, raw, targetType);
                }
                catch (InvalidOperationException exception)
                {
                    failures.Add(new CoercionFailure(memberName, raw, targetType, this.FilePath, exception.Message));
                }
            }
        }
        finally
        {
            this.failureCollector = null;
        }

        return failures;
    }

    private static Dictionary<string, Type> GetTargetPropertyMap(Type[] targetTypes)
    {
        // Thousands of fragments share the same handful of target type sets; build each map once.
        string cacheKey = string.Join("|", targetTypes.Select(type => type.AssemblyQualifiedName));

        return TargetPropertyMapCache.GetOrAdd(cacheKey, _ => BuildTargetPropertyMap(targetTypes));
    }

    private static Dictionary<string, Type> BuildTargetPropertyMap(Type[] targetTypes)
    {
        Dictionary<string, Type> map = new(StringComparer.OrdinalIgnoreCase);

        foreach (Type targetType in targetTypes)
        {
            foreach (Type type in targetType.GetInterfaces().Prepend(targetType))
            {
                foreach (PropertyInfo property in type.GetProperties())
                {
                    map.TryAdd(property.Name, property.PropertyType);
                }
            }
        }

        return map;
    }

    private static object? Default(Type targetType)
    {
        Type? underlying = Nullable.GetUnderlyingType(targetType);

        if (underlying is not null || !targetType.IsValueType)
        {
            return null;
        }

        return Activator.CreateInstance(targetType);
    }

    private static Type? GetEnumerableElementType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        return type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))?
            .GetGenericArguments()[0];
    }

    private object? Coerce(string memberName, object? value, Type? targetType)
    {
        if (targetType is null)
        {
            // The member is not declared on any target interface; expose the raw metadata value.
            return value;
        }

        if (value is null)
        {
            return Default(targetType);
        }

        Type effective = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (effective.IsInstanceOfType(value))
        {
            return value;
        }

        if (value is ContentFragment nestedFragment && effective.IsInterface)
        {
            // A fragment-valued member mapped to an interface duck-types recursively, so object graphs
            // (e.g. a post's Author fragment) get the same projection as the root fragment.
            DynamicContentFragment nested = new(nestedFragment, this.serviceProvider, effective);

            return Impromptu.DynamicActLike(nested, effective);
        }

        if (value is string stringValue)
        {
            return this.CoerceString(memberName, stringValue, effective);
        }

        if (value is Dictionary<object, object> nestedObject)
        {
            return this.CoerceNestedObject(memberName, nestedObject, effective);
        }

        if (value is IEnumerable enumerable)
        {
            // Any enumerable whose runtime type isn't already assignable to the target (that case exits
            // above) is projected per element — covers YAML lists (List<object>) and code-constructed
            // metadata such as List<ContentFragment> mapped onto IEnumerable<TInterface>.
            return this.CoerceEnumerable(memberName, value, enumerable, GetEnumerableElementType(effective));
        }

        try
        {
            return Convert.ChangeType(value, effective, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            this.ReportFailure(memberName, value, effective, ex.Message);

            return value;
        }
    }

    private object? CoerceString(string memberName, string value, Type targetType)
    {
        if (targetType == typeof(bool))
        {
            return bool.TryParse(value, out bool boolResult) ? boolResult : this.FailWithDefault(memberName, value, targetType);
        }

        if (targetType.IsEnum)
        {
            return Enum.TryParse(targetType, value, ignoreCase: true, out object? enumResult) ? enumResult : this.FailWithDefault(memberName, value, targetType);
        }

        if (targetType == typeof(DateTime))
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime invariantDate))
            {
                return invariantDate;
            }

            return DateTime.TryParse(value, out DateTime currentCultureDate) ? currentCultureDate : this.FailWithDefault(memberName, value, targetType);
        }

        if (targetType == typeof(DateTimeOffset))
        {
            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset invariantDate))
            {
                return invariantDate;
            }

            return DateTimeOffset.TryParse(value, out DateTimeOffset currentCultureDate) ? currentCultureDate : this.FailWithDefault(memberName, value, targetType);
        }

        if (targetType.IsPrimitive || targetType == typeof(decimal))
        {
            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
            {
                return this.FailWithDefault(memberName, value, targetType);
            }
        }

        return value;
    }

    private object CoerceEnumerable(string memberName, object original, IEnumerable enumerable, Type? elementType)
    {
        List<object?> items = [.. enumerable.Cast<object?>()];

        if (items.Count != 0 && items[0] is Dictionary<object, object>)
        {
            IConverter<Dictionary<object, object>>? converter = this.TryGetConverter(memberName);

            if (converter is null)
            {
                throw new InvalidOperationException($"No converter for content type '{memberName.AsConverter()}' is registered with the container.");
            }

            IList converted = elementType is null
                ? new List<object>()
                : (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;

            foreach (object? item in items)
            {
                if (item is Dictionary<object, object> entry)
                {
                    converted.Add(converter.Convert(entry));
                }
            }

            return converted;
        }

        if (elementType == typeof(string))
        {
            return items.ConvertAll(x => x?.ToString());
        }

        if (elementType is not null)
        {
            IList typed = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;

            foreach (object? item in items)
            {
                typed.Add(this.Coerce(memberName, item, elementType));
            }

            return typed;
        }

        return original;
    }

    private object? CoerceNestedObject(string memberName, Dictionary<object, object> value, Type targetType)
    {
        // A member-named converter takes precedence, mirroring the list-of-mappings convention.
        IConverter<Dictionary<object, object>>? converter = this.TryGetConverter(memberName);

        if (converter is not null)
        {
            return converter.Convert(value);
        }

        if (targetType.IsClass && !targetType.IsAbstract)
        {
            // Round-trip through YAML so nested frontmatter mappings deserialize into the POCO the
            // interface declares (e.g. a Dates block onto IBlogPost.Dates), including nested members.
            try
            {
                string yaml = NestedObjectSerializer.Serialize(value);

                return NestedObjectDeserializer.Deserialize(yaml, targetType);
            }
            catch (YamlException ex)
            {
                this.ReportFailure(memberName, value, targetType, ex.Message);

                return Default(targetType);
            }
        }

        return value;
    }

    private IConverter<Dictionary<object, object>>? TryGetConverter(string memberName)
    {
        try
        {
            return this.serviceProvider.GetContent<IConverter<Dictionary<object, object>>>(memberName.AsConverter());
        }
        catch (InvalidOperationException)
        {
            // No content is registered for the converter key; the caller decides whether that is an error.
            return null;
        }
    }

    private object? FailWithDefault(string memberName, object? value, Type targetType)
    {
        this.ReportFailure(memberName, value, targetType, $"the value could not be parsed as {targetType.Name}");

        return Default(targetType);
    }

    private void ReportFailure(string memberName, object? rawValue, Type targetType, string reason)
    {
        CoercionFailure failure = new(memberName, rawValue, targetType, this.FilePath, reason);

        this.failureCollector?.Add(failure);
        this.logger?.LogWarning(
            "Failed to coerce frontmatter member '{MemberName}' value '{RawValue}' to {TargetType} in '{FilePath}': {Reason}",
            failure.MemberName,
            failure.RawValue,
            failure.TargetType.Name,
            failure.FilePath,
            failure.Reason);
    }
}