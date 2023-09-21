# AvaloniaMauiHybrid

This repository contains source code for the integration of Avalonia and MAUI frameworks.
Currently supported scenarios:
- Embedding Avalonia controls inside of the MAUI pages.
- Embedding MAUI controls inside of the Avalonia views.
- Calling MAUI Essentials APIs from Avalonia.

Supported OS: iOS and Android only.
- For Windows support see https://github.com/AvaloniaUI/AvaloniaMauiHybrid/issues/7 open issue.
- For macOS support see https://github.com/AvaloniaUI/AvaloniaMauiHybrid/issues/10 open issue.
- MAUI doesn't support Browser platform, so we can't do much here. For Blazor integration, see [Avalonia.Browser.Blazor](https://www.nuget.org/packages/Avalonia.Browser.Blazor).
- Avalonia 11.1 is going to have Tizen backend, so it will be possible in the future.

## Quick Start

### Embedding Avalonia controls inside of the MAUI app

1. Start with a normal MAUI project.
2. Install https://www.nuget.org/packages/Avalonia.Maui nuget package to your project.
3. You need to have both Avalonia and MAUI Application classes created. You can copy [AvaloniaApp.axaml](https://github.com/AvaloniaUI/AvaloniaMauiHybrid/blob/main/MauiSample/AvaloniaApp.axaml) + [AvaloniaApp.axaml.cs](https://github.com/AvaloniaUI/AvaloniaMauiHybrid/blob/main/MauiSample/AvaloniaApp.axaml.cs) or reuse your own application with properties.
4. Update your MAUI app builder to include `UseAvalonia` call:
   ```csharp
    builder
      .UseMauiApp<App>() // MAUI Application
      .UseAvalonia<AvaloniaApp>() // Avalonia Application
   ```
5. If you need to modify Avalonia application builder, you can pass a lambda to the `UseAvalonia` method.
6. Now, you can use Avalonia controls from the MAUI XAML:
   ```xaml
   <StackLayout Orientation="Horizontal"  HorizontalOptions="FillAndExpand" VerticalOptions="Start">
        <maui:AvaloniaView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand">
            <ava:Button Content="Avalonia Button"/>
        </maui:AvaloniaView>
        <Button Text="Maui Button"/>
    </StackLayout>
   ```
   Don't forget to add Avalonia xmlns namespaces to your MAUI XAML file `xmlns:maui="clr-namespace:Avalonia.Maui.Controls;assembly=Avalonia.Maui"` and `xmlns:ava="clr-namespace:Avalonia.Controls;assembly=Avalonia.Controls"`, if IDE didn't include it automatically.


### Embedding MAUI controls inside of the Avalonia app

1. Start with avalonia.xplat template (see https://github.com/AvaloniaUI/avalonia-dotnet-templates). We will only use Android, iOS and shared projects.
2. Install https://www.nuget.org/packages/Avalonia.Maui nuget package to your project.
3. You need to have both Avalonia and MAUI Application classes created. For MAUI Application, you need to inherit Microsoft.Maui.Controls.Application, for example - [MauiApplication.cs](https://github.com/AvaloniaUI/AvaloniaMauiHybrid/blob/main/AvaloniaSample/AvaloniaSample/Maui/MauiApplication.cs).
4. Add `<UseMaui>true</UseMaui>` to every project (shared and platform-specific) from where you will use MAUI APIs. For example, [here](https://github.com/AvaloniaUI/AvaloniaMauiHybrid/blob/main/AvaloniaSample/AvaloniaSample/AvaloniaSample.csproj#L4) and [here](https://github.com/AvaloniaUI/AvaloniaMauiHybrid/blob/main/AvaloniaSample/AvaloniaSample.iOS/AvaloniaSample.iOS.csproj#L4).
5. Update both `MainActivity` (Android project) and `AppDelegate` (iOS project) app builders to include `.UseMaui()`
   ```csharp
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
      return base.CustomizeAppBuilder(builder)
          .UseMaui<AvaloniaSample.Maui.MauiApplication>(this);
    }
   ```
6. If you need to modify MAUI application builder, you can pass a lambda to the `UseMaui` method. For example, [we enable third-party MAUI controls this way](https://github.com/AvaloniaUI/AvaloniaMauiHybrid/blob/main/AvaloniaSample/AvaloniaSample.iOS/AppDelegate.cs#L26-L28).
7. Not, you can use MAUI controls from the Avalonia XAML:
   ```xaml
    <UniformGrid Columns="2" Height="40">
      <Button Content="Avalonia Button" />
      <controls:MauiControlHost>
        <mauiControls:Button Text="MAUI Button" />
      </controls:MauiControlHost>
    </UniformGrid>
   ```
   Don't forget about xmlns namespaces here as well: `xmlns:controls="using:Avalonia.Maui.Controls"` and `xmlns:mauiControls="using:Microsoft.Maui.Controls"`


### Using MAUI Essentials inside of the Avalonia app

1. Follow the same steps as in [Embedding MAUI controls inside of the Avalonia app](#embedding-maui-controls-inside-of-the-avalonia-app) except the last one.
2. Now you can call Essentials API from Avalonia code:
```csharp
private async void Button_OnClick(object? sender, RoutedEventArgs e)
{
    var location = await Geolocation.GetLocationAsync();
    Console.WriteLine(location.ToString());
}
```
Don't forget about enabling specific permissions. In the case of Geolocation class, you can follow [this documentation](https://learn.microsoft.com/en-us/xamarin/essentials/geolocation?tabs=android) from Microsoft.
