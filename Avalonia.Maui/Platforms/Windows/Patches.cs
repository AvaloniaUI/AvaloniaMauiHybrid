#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Controls.ApplicationLifetimes;
using HarmonyLib;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Authentication;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
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

    [HarmonyPatch]
    class WebAuthenticatorPatch
    {
        // This method tells Harmony which method to patch
        static MethodBase TargetMethod()
        {
            // Find the internal type by full name
            var type = AccessTools.TypeByName("Microsoft.Maui.Authentication.WebAuthenticatorImplementation");
            if (type == null)
                throw new Exception("Could not find WebAuthenticatorImplementation type");

            // Find the AuthenticateAsync method with WebAuthenticatorOptions parameter
            return AccessTools.Method(type, "AuthenticateAsync", new Type[] { typeof(WebAuthenticatorOptions) });
        }

        // Prefix runs before original method; returning false skips original
        static bool Prefix(object __instance, WebAuthenticatorOptions webAuthenticatorOptions, ref Task<WebAuthenticatorResult> __result)
        {
            if (webAuthenticatorOptions.CallbackUrl.Scheme == "http" || webAuthenticatorOptions.CallbackUrl.Scheme == "https")
            {
                __result = Task.Run(async () =>
                {
                    return await ProcessHttpScheme(webAuthenticatorOptions);
                });
                return false;
            }
            return true;
        }

        async static Task<WebAuthenticatorResult> ProcessHttpScheme(WebAuthenticatorOptions webAuthenticatorOptions)
        {
            using var listener = new HttpListener();

            listener.Prefixes.Add(webAuthenticatorOptions.CallbackUrl.OriginalString);
            listener.Start();

            await Launcher.OpenAsync(webAuthenticatorOptions.Url);

            var cancelToken = new CancellationTokenSource();
            var context = await listener.GetContextAsync().WaitAsync(TimeSpan.FromMinutes(1), cancelToken.Token);

            var response = context.Response;
            string responseString = "<html><head><style>h1{color:green;font-size:20px;}</style></head><body><h1>You can now close this window.</h1></body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();
            listener.Stop();

            if(webAuthenticatorOptions.ResponseDecoder is not null)
            {
                var dictionary = webAuthenticatorOptions.ResponseDecoder.DecodeResponse(context.Request.Url);
                return new WebAuthenticatorResult(dictionary);
            }
            
            return new WebAuthenticatorResult(context.Request.Url);

        }
    }
}
#endif