﻿using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MCM.Implementation.v1
{
    public sealed class MBOptionScreenSubModulePatch1
    {
        public static IEnumerable<MethodBase> TargetMethods() => AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Where(assembly => Path.GetFileName(assembly.Location) == "MBOptionScreen.dll")
            .Select(assembly => assembly.GetType("MBOptionScreen.MBOptionScreenSubModule"))
            .Select(type => AccessTools.Method(type, "OnSubModuleLoad"))
            .Where(method => method != null);

        public static bool Prefix() => false;
    }

    public sealed class MBOptionScreenSubModulePatch2
    {
        public static IEnumerable<MethodBase> TargetMethods() => AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Where(assembly => Path.GetFileName(assembly.Location) == "MBOptionScreen.dll")
            .Select(assembly => assembly.GetType("MBOptionScreen.MBOptionScreenSubModule"))
            .Where(type => type != null)
            .Select(type => AccessTools.Method(type, "OnBeforeInitialModuleScreenSetAsRoot"))
            .Where(method => method != null);

        public static bool Prefix() => false;
    }
}