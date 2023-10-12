using System;
using System.Collections.Generic;
using Avalonia.Maui.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Embedding;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;
using MauiWindow = Microsoft.Maui.Controls.Window;
#if IOS
using PlatformWindow = UIKit.UIWindow;
using UIKit;
#elif ANDROID
using PlatformWindow = global::Android.Content.Context;
#else
using PlatformWindow = System.Object;
#endif

namespace Avalonia.Maui;

public static class AvaloniaAppBuilderExtensions
{
#if ANDROID
    public static AppBuilder UseMaui<TMauiApplication>(this AppBuilder appBuilder, global::Android.App.Activity activity, Action<MauiAppBuilder>? configure = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#elif IOS
    public static AppBuilder UseMaui<TMauiApplication>(this AppBuilder appBuilder, IUIApplicationDelegate applicationDelegate, Action<MauiAppBuilder>? configure  = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#else
    public static AppBuilder UseMaui<TMauiApplication>(this AppBuilder appBuilder, Action<MauiAppBuilder>? configure  = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#endif
    {
        return appBuilder
            .AfterSetup(appBuilder =>
            {
                var builder = MauiApp.CreateBuilder()
                    .UseMauiEmbedding<TMauiApplication>();

                builder.Services.AddSingleton(appBuilder.Instance!)
#if ANDROID
                    .AddSingleton(activity.Application!)
                    .AddSingleton<global::Android.Content.Context>(activity)
                    .AddSingleton(activity)
#elif IOS
	                .AddSingleton(applicationDelegate ?? UIApplication.SharedApplication.Delegate)
	                .AddSingleton<UIWindow>(static p => p.GetService<IUIApplicationDelegate>()!.GetWindow())
#endif
                    .AddSingleton<IMauiInitializeService, MauiEmbeddingInitializer>();

                configure?.Invoke(builder);

                var mauiApp = builder.Build();
                InitializeMauiEmbeddingApp(mauiApp);
            });
    }

    private static void InitializeMauiEmbeddingApp(this MauiApp mauiApp)
    {
        var iApp = mauiApp.Services.GetRequiredService<IApplication>();

#if ANDROID
        var window = mauiApp.Services.GetRequiredService<global::Android.App.Activity>();
        var scope = mauiApp.Services.CreateScope();
        var platformApplication = window.Application!;
        var services = scope.ServiceProvider;
        var rootContext = new MauiContext(scope.ServiceProvider, window);

        Microsoft.Maui.ApplicationModel.Platform.Init(window, null);
#else
		var rootContext = new MauiContext(mauiApp.Services);
	    var services = mauiApp.Services;
#if IOS
	    var platformApplication = mauiApp.Services.GetRequiredService<IUIApplicationDelegate>();
	    var window = platformApplication.GetWindow();
	    if (window == null)
	    {   // hack for older Avalonia versions.
		    window = new UIWindow();
		    platformApplication.SetWindow(window);
	    }

		Microsoft.Maui.ApplicationModel.Platform.Init(() => platformApplication.GetWindow().RootViewController!);
#else
	    var platformApplication = new object();
	    var window = new object();
#endif
#endif

        var scopedServices = rootContext.Services.GetServices<IMauiInitializeScopedService>();
        foreach (var service in scopedServices)
        {
            service.Initialize(rootContext.Services);
        }

        platformApplication.SetApplicationHandler(iApp, rootContext);
        IPlatformApplication.Current = new EmbeddingApplication(services, iApp);

        if (iApp is Microsoft.Maui.Controls.Application { Handler.MauiContext: not null } app
            && iApp.Windows is List<MauiWindow> windows)
        {
            var virtualWindow = CreateVirtualWindow(app, window);
            windows.Add(virtualWindow);
        }
    }

    private static Window CreateVirtualWindow(Microsoft.Maui.Controls.Application app, PlatformWindow? window)
    {
#if ANDROID
        var services = app.Handler!.MauiContext!.Services;
        var context = new MauiContext(services, services.GetRequiredService<global::Android.App.Activity>());
#else
	    var context = app.Handler.MauiContext;
#endif

        // Create a Maui Window and initialize a Handler shim. This will expose the actual Application Window
        var virtualWindow = new MauiWindow();
        virtualWindow.Handler = new EmbeddedWindowHandler
        {
            PlatformView = window,
            VirtualView = virtualWindow,
            MauiContext = context
        };
        // This ContentPage is necessary only to fool the Xaml Hot Reload.
        virtualWindow.Page = new ContentPage();
        return virtualWindow;
    }

    private record EmbeddingApplication(IServiceProvider Services, IApplication Application) : IPlatformApplication;
}