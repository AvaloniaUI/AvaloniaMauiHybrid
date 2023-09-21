using System;
using Microsoft.Maui.Hosting;

namespace Avalonia.Maui
{
    public static class MauiAppHostBuilderExtensions
    {
        public static MauiAppBuilder UseAvalonia<TApp>(this MauiAppBuilder builder, Action<AppBuilder> customizeBuilder) where TApp : Application, new()
        {
            var avaloniaBuilder = AppBuilder.Configure<TApp>();
            customizeBuilder(avaloniaBuilder);
#if ANDROID
            avaloniaBuilder.UseAndroid();
#elif IOS
            avaloniaBuilder.UseiOS();
#endif

            avaloniaBuilder.SetupWithoutStarting();
            return builder;
        }
    }
}
