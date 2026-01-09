using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace AvaloniaSample.Android;

[Activity(
    Label = "AvaloniaSample.Android",
    Theme = "@style/Maui.SplashTheme",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}
