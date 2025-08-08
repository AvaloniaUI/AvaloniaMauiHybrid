#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Controls.ApplicationLifetimes;
using HarmonyLib;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Reflection;

namespace Avalonia.Maui.Platforms.Windows
{
    [HarmonyPatch]
    public static class HandlePatch
    {
        static MethodBase TargetMethod()
        {
            // Fully-qualified name of the internal static class
            var type = AccessTools.TypeByName("Microsoft.Maui.ApplicationModel.WindowStateManagerExtensions");
            if (type == null)
                throw new Exception("Could not find WindowStateManagerExtensions type");

            return AccessTools.Method(
                type,
                "GetActiveWindowHandle",
                new Type[] { typeof(IWindowStateManager), typeof(bool) }
            );
        }

        static bool Prefix(IWindowStateManager manager, bool throwOnNull, ref IntPtr __result)
        {
            __result = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow.TryGetPlatformHandle().Handle;
            return false;
        }

        public static void PatchAll()
        {
            var harmony = new HarmonyLib.Harmony("com.essentials.patch");
            harmony.PatchAll();
        }
    }
}
#endif
