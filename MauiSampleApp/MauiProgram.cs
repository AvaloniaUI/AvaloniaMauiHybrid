using Avalonia;
using Avalonia.Maui;
using Avalonia.Maui.Controls;
using Avalonia.Maui.Handlers;
using MauiSample;

namespace MauiSampleApp
{
    public static class MauiProgram
    {
        public static Microsoft.Maui.Hosting.MauiApp CreateMauiApp()
        {
            var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
            
            builder.UseMauiApp<MauiApp>();
            
            builder.UseAvalonia<AvaloniaApp>(appBuilder =>
            {
            });
            
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
            
            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID || IOS || WINDOWS
                handlers.AddHandler(typeof(AvaloniaView), typeof(AvaloniaViewHandler));
#endif
            });

            return builder.Build();
        }
    }
}