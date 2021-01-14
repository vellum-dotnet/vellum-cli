// <copyright file="DynamicContentFragment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions
{
    using System.Collections.Generic;
    using System.Dynamic;
    using Vellum.Abstractions.Content;

    public class DynamicContentFragment : DynamicObject
    {
        // The inner dictionary.
        private readonly Dictionary<string, dynamic> dictionary;
        private readonly ContentFragment contentFragment;

        public DynamicContentFragment(ContentFragment contentFragment)
        {
            this.contentFragment = contentFragment;
            this.dictionary = contentFragment.MetaData;
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
}