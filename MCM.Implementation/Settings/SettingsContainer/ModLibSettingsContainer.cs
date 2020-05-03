﻿using HarmonyLib;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Settings.Definitions;
using MCM.Abstractions.Settings;
using MCM.Abstractions.Settings.SettingsContainer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MCM.Implementation.Settings.SettingsContainer
{
    [Version("e1.0.0",  1)]
    [Version("e1.0.1",  1)]
    [Version("e1.0.2",  1)]
    [Version("e1.0.3",  1)]
    [Version("e1.0.4",  1)]
    [Version("e1.0.5",  1)]
    [Version("e1.0.6",  1)]
    [Version("e1.0.7",  1)]
    [Version("e1.0.8",  1)]
    [Version("e1.0.9",  1)]
    [Version("e1.0.10", 1)]
    [Version("e1.0.11", 1)]
    [Version("e1.1.0",  1)]
    [Version("e1.2.0",  1)]
    [Version("e1.2.1",  1)]
    [Version("e1.3.0",  1)]
    internal sealed class ModLibSettingsContainer : IModLibSettingsContainer
    {
        private Dictionary<string, ModLibSettingsWrapper> LoadedModLibSettings { get; } = new Dictionary<string, ModLibSettingsWrapper>();
        private Type? ModLibSettingsDatabase { get; }

        public List<SettingsDefinition> CreateModSettingsDefinitions
        {
            get
            {
                if (ModLibSettingsDatabase == null)
                    return new List<SettingsDefinition>();

                ReloadAll();

                return LoadedModLibSettings.Keys
                    .Select(id => new SettingsDefinition(id))
                    .OrderByDescending(a => a.ModName)
                    .ToList();
            }
        }

        public ModLibSettingsContainer()
        {
            ModLibSettingsDatabase = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(a => a.FullName == "ModLib.SettingsDatabase");
        }

        public SettingsBase? GetSettings(string id)
        {
            if (ModLibSettingsDatabase == null)
                return null;

            ReloadAll();
            return LoadedModLibSettings.TryGetValue(id, out var settings) ? settings : null;
        }

        public bool OverrideSettings(SettingsBase settings)
        {
            if (ModLibSettingsDatabase == null)
                return false;

            if (settings is ModLibSettingsWrapper settingsWrapper)
            {
                var overrideSettingsWithIDMethod = AccessTools.Method(ModLibSettingsDatabase, "OverrideSettingsWithID");

                return (bool) overrideSettingsWithIDMethod.Invoke(null, new object[] { settingsWrapper.Object, settingsWrapper.Id });
            }

            return false;
        }

        public bool RegisterSettings(SettingsBase settingsClass) => settingsClass is ModLibSettingsWrapper;

        public SettingsBase? ResetSettings(string id)
        {
            if (ModLibSettingsDatabase == null)
                return null;

            ReloadAll();

            var resetSettingsInstanceMethod = AccessTools.Method(ModLibSettingsDatabase, "ResetSettingsInstance");

            var settingsWrapper = LoadedModLibSettings[id];
            resetSettingsInstanceMethod.Invoke(null, new object[] { settingsWrapper.Object });

            Reload(id);
            return LoadedModLibSettings[id];
        }

        public void SaveSettings(SettingsBase settings)
        {
            if (ModLibSettingsDatabase == null)
                return;

            if (settings is ModLibSettingsWrapper settingsWrapper)
            {
                var saveSettingsMethod = AccessTools.Method(ModLibSettingsDatabase, "SaveSettings");

                saveSettingsMethod.Invoke(null, new object[] { settingsWrapper.Object });
            }
        }

        private void ReloadAll()
        {
            var saveSettingsMethod = AccessTools.Property(ModLibSettingsDatabase, "AllSettingsDict");
            var dict = (IDictionary) saveSettingsMethod.GetValue(null);
            foreach (var settings in dict.Values)
            {
                var settingsType = settings.GetType();
                var idProperty = AccessTools.Property(settingsType, "ID") ?? AccessTools.Property(settingsType, "Id");
                var id = (string) idProperty.GetValue(settings);

                var settWrapper = new ModLibSettingsWrapper(settings);

                if (!LoadedModLibSettings.ContainsKey(id))
                    LoadedModLibSettings.Add(id, settWrapper);
                else
                    LoadedModLibSettings[id] = settWrapper;
            }
        }
        private void Reload(string id)
        {
            var saveSettingsMethod = AccessTools.Property(ModLibSettingsDatabase, "AllSettingsDict");
            var dict = (IDictionary) saveSettingsMethod.GetValue(null);

            if (dict.Contains(id))
                LoadedModLibSettings[id] = new ModLibSettingsWrapper(dict[id]);
        }
    }
}