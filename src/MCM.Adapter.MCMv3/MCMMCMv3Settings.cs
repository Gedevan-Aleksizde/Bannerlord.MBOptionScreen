﻿extern alias v4;

using System.Collections.Generic;

using TaleWorlds.Localization;

namespace MCM.Adapter.MCMv3
{
    internal sealed class MCMMCMv3Settings : v4::MCM.Abstractions.Settings.Base.Global.AttributeGlobalSettings<MCMMCMv3Settings>
    {
        public override string Id { get; } = "MCMMCMv3_v4";
        public override string DisplayName => new TextObject("{=MCMMCMv3Settings_Name}MCM MCMv3 Adapter {VERSION}", new Dictionary<string, TextObject>
        {
            { "VERSION", new TextObject(typeof(MCMMCMv3Settings).Assembly.GetName().Version?.ToString(3) ?? "ERROR") }
        }).ToString();
        public override string FolderName { get; } = "MCM";
        public override string FormatType { get; } = "none";
    }
}