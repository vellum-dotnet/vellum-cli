// <copyright file="DynamicContentFragment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Vellum.Abstractions.Content;
using Vellum.Abstractions.Content.ContentFactories;
using Vellum.Abstractions.Content.Converters;

namespace Vellum.Abstractions;

public class DynamicContentFragment : DynamicObject
{
    private readonly ContentFragment contentFragment;
    private readonly Dictionary<string, dynamic> dictionary;
    private readonly IServiceProvider serviceProvider;

    public DynamicContentFragment(ContentFragment contentFragment, IServiceProvider serviceProvider)
    {
        this.contentFragment = contentFragment;
        this.serviceProvider = serviceProvider;
        this.dictionary = new Dictionary<string, dynamic>(contentFragment.MetaData, StringComparer.InvariantCultureIgnoreCase);
    }

    // This property returns the number of elements
    // in the inner dictionary.
    public int Count
    {
        get
        {
            return this.dictionary.Count;
        }
    }

    // If you try to get a value of a property
    // not defined in the class, this method is called.
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        if (!this.dictionary.TryGetValue(binder.Name, out result))
        {
            result = typeof(ContentFragment).GetProperty(binder.Name)?.GetValue(this.contentFragment, null);
            return true;
        }

        if (result is List<object> list)
        {
            if (list.Count() != 0 && list[0] is Dictionary<object, object>)
            {
                List<object> converted = new();
                foreach (object item in list)
                {
                    if (item is Dictionary<object, object> entry)
                    {
                        IConverter<Dictionary<object, object>> converter = this.serviceProvider.GetContent<IConverter<Dictionary<object, object>>>(binder.Name.AsConverter());

                        if (converter is null)
                        {
                            throw new InvalidOperationException($"No converter for content type '{binder.Name.AsConverter()}' is registered with the container.");
                        }

                        converted.Add(converter.Convert(entry));
                    }
                }

                result = converted;

                return true;
            }

            result = list.ConvertAll(x => x.ToString());
        }

        if (bool.TryParse(result.ToString(), out bool resultAsBool))
        {
            result = resultAsBool;
        }

        return true;
    }

    // If you try to set a value of a property that is
    // not defined in the class, this method is called.
    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        // Converting the property name to lowercase
        // so that property names become case-insensitive.
        this.dictionary[binder.Name] = value;

        // You can always add a value to a dictionary,
        // so this method always returns true.
        return true;
    }
}