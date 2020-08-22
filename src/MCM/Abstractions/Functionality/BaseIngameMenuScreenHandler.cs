﻿using Bannerlord.ButterLib;
using Bannerlord.ButterLib.Common.Extensions;

using MCM.Extensions;

using System;

using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;

namespace MCM.Abstractions.Functionality
{
    public abstract class BaseIngameMenuScreenHandler : IDependency
    {
        private static BaseIngameMenuScreenHandler? _instance;
        public static BaseIngameMenuScreenHandler Instance => _instance ??=
            ButterLibSubModule.Instance.GetServiceProvider().GetRequiredService<BaseIngameMenuScreenHandler, IngameMenuScreenHandlerWrapper>();
            //DI.GetImplementation<BaseIngameMenuScreenHandler, IngameMenuScreenHandlerWrapper>()!;

        public abstract void AddScreen(string internalName, int index, Func<ScreenBase?> screenFactory, TextObject text);
        public abstract void RemoveScreen(string internalName);
    }
}