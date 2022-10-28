﻿using BUTR.DependencyInjection;

using MCM.Abstractions.Base;
using MCM.Abstractions.Properties;

using System.Collections.Generic;
using System.Linq;

namespace MCM.Abstractions
{
    public static class BaseSettingsExtensions
    {
        public static List<SettingsPropertyGroupDefinition> GetSettingPropertyGroups(this BaseSettings settings) => settings
            .GetUnsortedSettingPropertyGroups()
            .SortDefault()
            .ToList();

        public static IEnumerable<SettingsPropertyGroupDefinition> GetUnsortedSettingPropertyGroups(this BaseSettings settings)
        {
            if (settings is IFluentSettings fluentSettings)
                return fluentSettings.SettingPropertyGroups;

            var discoverers = GenericServiceProvider.GetService<IEnumerable<ISettingsPropertyDiscoverer>>() ?? Enumerable.Empty<ISettingsPropertyDiscoverer>();
            var discoverer = discoverers.FirstOrDefault(x => x.DiscoveryTypes.Any(y => y == settings.DiscoveryType));
            return SettingsUtils.GetSettingsPropertyGroups(settings.SubGroupDelimiter, discoverer?.GetProperties(settings) ?? Enumerable.Empty<ISettingsPropertyDefinition>());
        }

        public static IEnumerable<ISettingsPropertyDefinition> GetAllSettingPropertyDefinitions(this BaseSettings settings) =>
            settings.GetSettingPropertyGroups().SelectMany(SettingsUtils.GetAllSettingPropertyDefinitions);
        public static IEnumerable<SettingsPropertyGroupDefinition> GetAllSettingPropertyGroupDefinitions(this BaseSettings settings) =>
            settings.GetSettingPropertyGroups().SelectMany(SettingsUtils.GetAllSettingPropertyGroupDefinitions);

        public static IEnumerable<ISettingsPreset> GetExternalPresets(this BaseSettings settings) =>
            GenericServiceProvider.GetService<BaseSettingsProvider>()?.GetPresets(settings.Id) ?? Enumerable.Empty<ISettingsPreset>();
    }
}