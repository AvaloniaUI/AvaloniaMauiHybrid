namespace Avalonia.Maui
{
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
#endif
            var lifetime = new MauiApplicationLifetime();

            avaloniaBuilder.SetupWithLifetime(lifetime);

            return builder;
        }
    }
}
