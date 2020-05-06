﻿using MCM.Abstractions.Settings.Definitions;
using MCM.Utils;

using System.Collections.Generic;

namespace MCM.Abstractions.Settings.SettingsProvider
{
    public abstract class BaseSettingsProvider
    {
        private static BaseSettingsProvider? _instance;
        public static BaseSettingsProvider Instance => _instance
            ?? (_instance = DI.GetImplementation<BaseSettingsProvider, SettingsProviderWrapper>(ApplicationVersionUtils.GameVersion()));

        public abstract IEnumerable<SettingsDefinition> CreateModSettingsDefinitions { get; }
        public abstract BaseSettings? GetSettings(string id);
        public abstract void SaveSettings(BaseSettings settings);
        public abstract void ResetSettings(BaseSettings settings);
        public abstract void OverrideSettings(BaseSettings settings);
    }
}