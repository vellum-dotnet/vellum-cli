// <copyright file="ISettingsManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Abstractions.Configuration
{
    using System;

    public interface ISettingsManager<T>
        where T : class
    {
        T LoadSettings();

        void SaveSettings(T settings);
    }
}