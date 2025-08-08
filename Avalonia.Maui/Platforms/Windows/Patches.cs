#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Controls.ApplicationLifetimes;
using HarmonyLib;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Avalonia.Maui.Platforms.Windows
{
    /// <summary>
    /// Determine which Essentials work in Windows Unpackaged apps
    /// https://github.com/dotnet/maui/issues/8552
    /// Essentials ContactsPicker is no longer supported on Windows
    /// https://github.com/dotnet/docs-maui/issues/1191
    /// </summary>
    public static class Patches
    {
        public static void PatchAll()
        {
            var harmony = new Harmony("com.essentials.patch");
            harmony.PatchAll();
        }
    }

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
    }

    [HarmonyPatch]
    public static class PhoneDialerPatch
    {
        static System.Reflection.MethodBase TargetMethod()
        {
            // Even if PhoneDialerImplementation is internal, Harmony can still grab it
            var type = AccessTools.TypeByName("Microsoft.Maui.ApplicationModel.Communication.PhoneDialerImplementation");
            if (type == null)
                throw new Exception("Could not find PhoneDialerImplementation type");

            return AccessTools.Method(type, "Open", new Type[] { typeof(string) });
        }

        // Prefix to fully replace the behavior
        static bool Prefix(object __instance, string number)
        {
            Console.WriteLine($"Harmony: Overriding PhoneDialerImplementation.Open with number = {number}");

            // You can re-use the original ValidateOpen method if you want
            var validateMethod = AccessTools.Method(__instance.GetType(), "ValidateOpen");
            validateMethod.Invoke(__instance, new object[] { number });

            // Your custom logic instead of the built-in dialing
            // Example: always use Launcher regardless of PhoneCallManager presence
            Task.Run(async () =>
            {
                await Launcher.OpenAsync($"tel:{number}");
            });

            return false; // false = skip original method completely
        }
    }
}
#endif