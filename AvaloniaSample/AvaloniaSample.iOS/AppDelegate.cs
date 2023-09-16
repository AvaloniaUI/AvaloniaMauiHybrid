using Avalonia;
using Foundation;
using Avalonia.iOS;
using Avalonia.Maui;

namespace AvaloniaSample.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaAppDelegate<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithMaui<AvaloniaSample.Maui.MauiApplication>();
    }
}
