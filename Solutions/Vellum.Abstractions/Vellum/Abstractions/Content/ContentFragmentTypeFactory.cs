// <copyright file="ContentFragmentTypeFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System;
    using ImpromptuInterface;

    public class ContentFragmentTypeFactory<T>
        where T : class, IContent
    {
        private readonly IServiceProvider serviceProvider;

        public ContentFragmentTypeFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public T Create(ContentFragment contentFragment)
        {
            var cf = new DynamicContentFragment(contentFragment, this.serviceProvider);

            return cf.ActLike<T>();
        }
  }
}