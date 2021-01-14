// <copyright file="ContentFragmentTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using ImpromptuInterface;

    public class ContentFragmentTypeFactory<T>
        where T : class, IContent
    {
        public T Create(ContentFragment contentFragment)
        {
            var cf = new DynamicContentFragment(contentFragment);

            return cf.ActLike<T>();
        }
    }
}