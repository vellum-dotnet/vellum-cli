// <copyright file="DynamicContentFragment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Converters;

namespace Vellum.Abstractions;

/// <summary>
/// A dynamic view over a <see cref="ContentFragment"/>'s metadata that coerces values to the property
/// types declared by the target interfaces the fragment is duck-typed onto.
/// </summary>
public class DynamicContentFragment : DynamicObject
{
    private readonly ContentFragment contentFragment;
    private readonly Dictionary<string, dynamic> dictionary;
    private readonly IServiceProvider serviceProvider;
    private readonly Dictionary<string, Type> targetProperties;

    public DynamicContentFragment(ContentFragment contentFragment, IServiceProvider serviceProvider, params Type[] targetTypes)
    {
        this.contentFragment = contentFragment;
        this.serviceProvider = serviceProvider;
        this.dictionary = new Dictionary<string, dynamic>(contentFragment.MetaData, StringComparer.InvariantCultureIgnoreCase);
        this.targetProperties = BuildTargetPropertyMap(targetTypes);
    }

    public int Count
    {
        get
        {
            return this.dictionary.Count;
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (!this.dictionary.TryGetValue(binder.Name, out object? raw))
        {
            raw = typeof(ContentFragment)
                .GetProperty(binder.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)?
                .GetValue(this.contentFragment, null);
        }

        this.targetProperties.TryGetValue(binder.Name, out Type? targetType);

        result = this.Coerce(binder.Name, raw, targetType);

        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        // Writes are local to this view; the underlying (potentially cached) ContentFragment is never mutated.
        this.dictionary[binder.Name] = value!;

        return true;
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

    private static object? CoerceString(string value, Type targetType)
    {
        if (targetType == typeof(bool))
        {
            return bool.TryParse(value, out bool boolResult) ? boolResult : Default(targetType);
        }

        if (targetType.IsEnum)
        {
            return Enum.TryParse(targetType, value, ignoreCase: true, out object? enumResult) ? enumResult : Default(targetType);
        }

        if (targetType == typeof(DateTime))
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime invariantDate))
            {
                return invariantDate;
            }

            return DateTime.TryParse(value, out DateTime currentCultureDate) ? currentCultureDate : Default(targetType);
        }

        if (targetType == typeof(DateTimeOffset))
        {
            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset invariantDate))
            {
                return invariantDate;
            }

            return DateTimeOffset.TryParse(value, out DateTimeOffset currentCultureDate) ? currentCultureDate : Default(targetType);
        }

        if (targetType.IsPrimitive || targetType == typeof(decimal))
        {
            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
            {
                return Default(targetType);
            }
        }

        return value;
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

        if (value is string stringValue)
        {
            return CoerceString(stringValue, effective);
        }

        if (value is List<object> list)
        {
            return this.CoerceList(memberName, list, GetEnumerableElementType(effective));
        }

        try
        {
            return Convert.ChangeType(value, effective, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            return value;
        }
    }

    private object CoerceList(string memberName, List<object> list, Type? elementType)
    {
        if (list.Count != 0 && list[0] is Dictionary<object, object>)
        {
            IConverter<Dictionary<object, object>>? converter = this.serviceProvider.GetContent<IConverter<Dictionary<object, object>>>(memberName.AsConverter());

            if (converter is null)
            {
                throw new InvalidOperationException($"No converter for content type '{memberName.AsConverter()}' is registered with the container.");
            }

            IList converted = elementType is null
                ? new List<object>()
                : (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;

            foreach (object item in list)
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
            return list.ConvertAll(x => x?.ToString());
        }

        if (elementType is not null)
        {
            IList typed = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;

            foreach (object item in list)
            {
                typed.Add(this.Coerce(memberName, item, elementType));
            }

            return typed;
        }

        return list;
    }
}