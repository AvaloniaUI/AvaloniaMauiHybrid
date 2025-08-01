using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Maui.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;
using MauiWindow = Microsoft.Maui.Controls.Window;
#if IOS
using PlatformWindow = UIKit.UIWindow;
using UIKit;
#elif ANDROID
using PlatformWindow = global::Android.Content.Context;
#elif WINDOWS10_0_19041_0_OR_GREATER
using PlatformWindow = Microsoft.UI.Xaml.Window;
#else
using PlatformWindow = System.Object;
#endif

namespace Avalonia.Maui;

public static class AvaloniaAppBuilderExtensions
{
#if WINDOWS10_0_19041_0_OR_GREATER
    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
#endif

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
#if WINDOWS10_0_19041_0_OR_GREATER
        Avalonia.Maui.Platforms.Windows.App.Start();
#endif

        return appBuilder
            .AfterSetup(appBuilder =>
            {
                var builder = MauiApp.CreateBuilder()
                    .UseMauiApp<TMauiApplication>();

                builder.Services.AddSingleton(appBuilder.Instance!)
#if ANDROID
                    .AddSingleton(activity.Application!)
                    .AddSingleton<global::Android.Content.Context>(activity)
                    .AddSingleton(activity)
#elif IOS
	                .AddSingleton(applicationDelegate ?? UIApplication.SharedApplication.Delegate)
	                .AddSingleton<UIWindow>(static p => p.GetService<IUIApplicationDelegate>()!.GetWindow())
#elif WINDOWS10_0_19041_0_OR_GREATER
                    .AddKeyedSingleton<IDispatcher, Platforms.Windows.AppDispatcher>(typeof(IApplication))
                    .AddSingleton(Microsoft.UI.Xaml.Application.Current)
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
#elif WINDOWS10_0_19041_0_OR_GREATER
        var platformApplication = mauiApp.Services.GetRequiredService<Microsoft.UI.Xaml.Application>();
        Microsoft.UI.Dispatching.DispatcherQueueController.CreateOnCurrentThread();
        Microsoft.UI.Xaml.Hosting.WindowsXamlManager.InitializeForCurrentThread();
        var window = new Microsoft.UI.Xaml.Window();
        Microsoft.Maui.ApplicationModel.Platform.OnPlatformWindowInitialized(window);
        if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime destop)
        {
            destop.ShutdownRequested += (_, _) => Environment.Exit(0);
            destop.Startup += (_, _) =>
            {
                var handle = window.GetWindowHandle();
                var hwnd = destop.MainWindow!.TryGetPlatformHandle()!.Handle;
                SetParent(handle, hwnd);
            };
        }
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