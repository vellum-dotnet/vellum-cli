// <copyright file="DynamicDictionary.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Dynamic;

namespace Vellum.Abstractions;

public class DynamicDictionary : DynamicObject
{
    // The inner dictionary.
    private readonly Dictionary<string, dynamic> dictionary = [];

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
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        // Converting the property name to lowercase
        // so that property names become case-insensitive.
        string name = binder.Name.ToLower();

        // If the property name is found in a dictionary,
        // set the result parameter to the property value and return true.
        // Otherwise, return false.
        return this.dictionary.TryGetValue(name, out result);
    }

    // If you try to set a value of a property that is
    // not defined in the class, this method is called.
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (value is null)
        {
            return false;
        }

        // Converting the property name to lowercase
        // so that property names become case-insensitive.
        this.dictionary[binder.Name.ToLower()] = value;

        // You can always add a value to a dictionary,
        // so this method always returns true.
        return true;
    }
}