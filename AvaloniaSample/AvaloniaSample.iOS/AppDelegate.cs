using Avalonia;
using Foundation;
using Avalonia.iOS;
using Avalonia.Maui;
using AVFoundation;
using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;
using UIKit;

namespace AvaloniaSample.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaAppDelegate<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        // See https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/mediaelement#bypassing-the-ios-silent-switch
        AVAudioSession.SharedInstance().SetActive(true);
        AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);

        return base.CustomizeAppBuilder(builder)
            .UseMaui<AvaloniaSample.Maui.MauiApplication>(this, b => b
                .ConfigureSyncfusionCore()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement());
    }
}
