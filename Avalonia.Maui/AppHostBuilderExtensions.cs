namespace Avalonia.Maui
{
    using Avalonia;

    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder UseAvalonia<TApp>(this MauiAppBuilder builder, Action<AppBuilder> customizeBuilder) where TApp : Application, new()
        {
            var avaloniaBuilder = AppBuilder.Configure<TApp>();
            customizeBuilder(avaloniaBuilder);
#if ANDROID
            avaloniaBuilder.UseAndroid();
#elif IOS
            avaloniaBuilder.UseiOS();
#elif WINDOWS
            avaloniaBuilder.UseWin32();
            avaloniaBuilder.UseSkia();
#else
#error missing use for platform
#endif
            var lifetime = new MauiApplicationLifetime();

            avaloniaBuilder.SetupWithLifetime(lifetime);

            return builder;
        }
    }
}
