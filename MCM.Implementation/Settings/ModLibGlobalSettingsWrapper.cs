﻿using HarmonyLib;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Settings.Definitions;
using MCM.Utils;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MCM.Abstractions.Settings
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
    public class ModLibGlobalSettingsWrapper : BaseGlobalSettingsWrapper
    {
        private PropertyInfo? IDProperty { get; }
        private PropertyInfo? ModuleFolderNameProperty { get; }
        private PropertyInfo? ModNameProperty { get; }
        private PropertyInfo? SubFolderProperty { get; }

        public override string Id => IDProperty?.GetValue(Object) as string ?? "ERROR";
        public override string ModuleFolderName // TODO: ModLib throws for some reason
        {
            get
            {
                try { return ModuleFolderNameProperty?.GetValue(Object) as string ?? ""; }
                catch (TargetInvocationException) { return ""; }
            }
        }
        public override string DisplayName => ModNameProperty?.GetValue(Object) as string ?? "ERROR";
        public override int UIVersion => 1;
        public override string SubFolder => SubFolderProperty?.GetValue(Object) as string ?? "";
        protected override char SubGroupDelimiter => '/';
        public override string Format => "json";
        public override event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

        public ModLibGlobalSettingsWrapper(object @object) : base(@object)
        {
            var type = @object.GetType();
            IDProperty = AccessTools.Property(type, "ID");
            ModuleFolderNameProperty = AccessTools.Property(type, nameof(ModuleFolderName));
            ModNameProperty = AccessTools.Property(type, "ModName");
            SubFolderProperty = AccessTools.Property(type, nameof(SubFolder));

            IsCorrect = IDProperty != null && ModuleFolderNameProperty != null &&
                        ModNameProperty != null && SubFolderProperty != null;
        }

        internal void UpdateReference(object @object)
        {
            Object = @object;
        }

        protected override void OnPropertyChanged(string? propertyName = null) { }

        public override List<SettingsPropertyGroupDefinition> GetSettingPropertyGroups() => GetWrappedSettingPropertyGroups();

        private List<SettingsPropertyGroupDefinition> GetWrappedSettingPropertyGroups() => GetUnsortedSettingPropertyGroups()
            .OrderByDescending(x => x.GroupName == SettingsPropertyGroupDefinition.DefaultGroupName)
            .ThenByDescending(x => x.Order)
            .ThenByDescending(x => x, new AlphanumComparatorFast())
            .ToList();

        protected override IEnumerable<SettingsPropertyGroupDefinition> GetUnsortedSettingPropertyGroups()
        {
            var groups = new List<SettingsPropertyGroupDefinition>();
            foreach (var settingProp in SettingsUtils.GetProperties(Object, Id))
            {
                //Find the group that the setting property should belong to. This is the default group if no group is specifically set with the SettingPropertyGroup attribute.
                var group = GetGroupFor(settingProp, groups);
                group.Add(settingProp);
            }
            return groups;
        }
    }
}