using System;
using Avalonia.Maui.Controls;
using Avalonia.Maui.Handlers;
using Microsoft.Maui.Hosting;

namespace Avalonia.Maui
{
    public static class MauiAppHostBuilderExtensions
    {
        public static MauiAppBuilder UseAvalonia<TApp>(this MauiAppBuilder builder, Action<AppBuilder>? customizeBuilder = null) where TApp : Application, new()
        {
            var avaloniaBuilder = AppBuilder.Configure<TApp>();
#if ANDROID
            avaloniaBuilder.UseAndroid();
#elif IOS
            avaloniaBuilder.UseiOS();
#elif WINDOWS10_0_19041_0_OR_GREATER
            avaloniaBuilder.UsePlatformDetect();
#endif

            customizeBuilder?.Invoke(avaloniaBuilder);

            avaloniaBuilder.SetupWithoutStarting();

            return builder
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID || IOS || WINDOWS10_0_19041_0_OR_GREATER
                    handlers.AddHandler(typeof(AvaloniaView), typeof(AvaloniaViewHandler));
#endif
                });;
        }
    }
}
