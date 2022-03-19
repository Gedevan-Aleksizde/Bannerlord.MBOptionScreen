﻿using BUTR.DependencyInjection;
using BUTR.DependencyInjection.Logger;

using HarmonyLib.BUTR.Extensions;

using MCM.Abstractions;
using MCM.Abstractions.Settings.Base;
using MCM.Abstractions.Settings.Base.PerCampaign;
using MCM.Abstractions.Settings.Containers;
using MCM.Abstractions.Settings.Formats;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace MCM.Implementation.Settings.Containers.PerCampaign
{
    internal sealed class MCMPerCampaignSettingsContainer : BaseSettingsContainer<PerCampaignSettings>, IMCMPerCampaignSettingsContainer
    {
        private readonly IBUTRLogger _logger;

        /// <inheritdoc/>
        protected override string RootFolder { get; }

        public MCMPerCampaignSettingsContainer(IBUTRLogger<MCMPerCampaignSettingsContainer> logger)
        {
            _logger = logger;
            RootFolder = Path.Combine(base.RootFolder, "PerCampaign");
        }

        /// <inheritdoc/>
        protected override void RegisterSettings(PerCampaignSettings? perCampaignSettings)
        {
            if (perCampaignSettings is null)
                return;

            var directoryPath = Path.Combine(RootFolder, Campaign.Current.UniqueGameId, perCampaignSettings.FolderName, perCampaignSettings.SubFolder);
            var settingsFormats = GenericServiceProvider.GetService<IEnumerable<ISettingsFormat>>() ?? Enumerable.Empty<ISettingsFormat>();
            var settingsFormat = settingsFormats.FirstOrDefault(x => x.FormatTypes.Any(y => y == perCampaignSettings.FormatType));
            settingsFormat?.Load(perCampaignSettings, directoryPath, perCampaignSettings.Id);
        }

        /// <inheritdoc/>
        public override bool SaveSettings(BaseSettings settings)
        {
            if (settings is not PerCampaignSettings campaignSettings)
                return false;

            var directoryPath = Path.Combine(RootFolder, Campaign.Current.UniqueGameId, campaignSettings.FolderName, campaignSettings.SubFolder);
            var settingsFormats = GenericServiceProvider.GetService<IEnumerable<ISettingsFormat>>() ?? Enumerable.Empty<ISettingsFormat>();
            var settingsFormat = settingsFormats.FirstOrDefault(x => x.FormatTypes.Any(y => y == campaignSettings.FormatType));
            settingsFormat?.Save(campaignSettings, directoryPath, campaignSettings.Id);

            return true;
        }

        public void OnGameStarted(Game game)
        {
            LoadedSettings.Clear();

            LoadSettings();
        }

        private void LoadSettings()
        {
            IEnumerable<PerCampaignSettings> GetPerCampaignSettings()
            {
                foreach (var assembly in AccessTools2.AllAssemblies().Where(a => !a.IsDynamic))
                {
                    IEnumerable<PerCampaignSettings> settings;
                    try
                    {
                        settings = AccessTools2.GetTypesFromAssemblyIfValid(assembly)
                            .Where(t => t.IsClass && !t.IsAbstract)
                            .Where(t => t.GetConstructor(Type.EmptyTypes) is not null)
                            .Where(t => typeof(PerCampaignSettings).IsAssignableFrom(t))
                            .Where(t => !typeof(EmptyPerCampaignSettings).IsAssignableFrom(t))
                            .Where(t => !typeof(IWrapper).IsAssignableFrom(t))
                            .Select(t => Activator.CreateInstance(t) as PerCampaignSettings)
                            .OfType<PerCampaignSettings>()
                            .ToList();
                    }
                    catch (TypeLoadException ex)
                    {
                        settings = Array.Empty<PerCampaignSettings>();
                        _logger.LogError(ex, "Error while handling assembly {Assembly}!", assembly);
                    }

                    foreach (var setting in settings)
                    {
                        yield return setting;
                    }
                }
            }

            foreach (var setting in GetPerCampaignSettings())
                RegisterSettings(setting);
        }

        public void OnGameEnded(Game game)
        {
            LoadedSettings.Clear();
        }
    }
}