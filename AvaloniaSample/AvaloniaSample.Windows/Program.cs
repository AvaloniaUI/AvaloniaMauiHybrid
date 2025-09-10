using Avalonia;
using Avalonia.Maui;
using AvaloniaSample;
using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;

namespace AvaloniaMauiSample.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseMaui<MauiApplication>(builder =>
            {
                builder.UseMauiCommunityToolkit();
                builder.UseMauiCommunityToolkitMediaElement();
                builder.ConfigureSyncfusionCore();
               
            })
            .UsePlatformDetect()
            .LogToTrace();

}

public class MauiApplication : Microsoft.Maui.Controls.Application
{
    
}
