using Avalonia;
using Avalonia.Maui;
using Avalonia.Maui.Controls;
using Avalonia.Maui.Handlers;
using MauiSample;
using System.Collections.Immutable;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/2021/maui", "Microsoft.Maui.Controls", AssemblyName = "Microsoft.Maui.Controls")]

namespace MauiSampleApp
{

    public static class MauiProgram
    {
        private static object DoNotOptimiceAway = ImmutableArray.Create(
        
            typeof(Microsoft.Maui.Controls.Shell),
            typeof(Microsoft.Maui.Controls.ContentPage),
            typeof(Microsoft.Maui.Controls.Application),
            typeof(Microsoft.Maui.Controls.ResourceDictionary)
        );


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
                handlers.AddHandler(typeof(VirtualAvaloniaView), typeof(AvaloniaViewHandler));
#endif
            });

            return builder.Build();
        }
    }
}