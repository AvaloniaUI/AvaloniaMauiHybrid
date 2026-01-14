using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Avalonia.Maui;
using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;

namespace AvaloniaSample.Android;

[Activity(
    Label = "AvaloniaSample.Android",
    Theme = "@style/Maui.SplashTheme",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return builder.UseMaui<AvaloniaSample.Maui.MauiApplication>(this, b => b
                .ConfigureSyncfusionCore()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement());
    }
}
