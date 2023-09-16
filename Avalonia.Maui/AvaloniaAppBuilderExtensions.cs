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
using UIKit;
#endif

namespace Avalonia.Maui;

public static class AvaloniaAppBuilderExtensions
{
#if ANDROID
    public static AppBuilder WithMaui<TMauiApplication>(this AppBuilder appBuilder, global::Android.App.Activity activity, Action<MauiAppBuilder>? configure  = null)
        where TMauiApplication : Microsoft.Maui.Controls.Application
#else
    public static AppBuilder WithMaui<TMauiApplication>(this AppBuilder appBuilder, Action<MauiAppBuilder>? configure  = null)
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
	                .AddSingleton(UIApplication.SharedApplication.Delegate)
	                .AddSingleton<UIWindow>(_ => UIApplication.SharedApplication.Delegate.GetWindow())
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
	    var activity = mauiApp.Services.GetRequiredService <global::Android.App.Activity>();
	    var scope = mauiApp.Services.CreateScope();
	    var platformApplication = activity.Application!;
	    var services = scope.ServiceProvider;
	    var rootContext = new MauiContext(scope.ServiceProvider, activity);

	    Microsoft.Maui.ApplicationModel.Platform.Init(activity, null);
#else
		var rootContext = new MauiContext(mauiApp.Services);
	    var services = mauiApp.Services;
#if IOS
	    var platformApplication = UIApplication.SharedApplication.Delegate;

		Microsoft.Maui.ApplicationModel.Platform.Init(() => platformApplication.GetWindow().RootViewController!);
#else
	    var platformApplication = new object();
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
		    var virtualWindow = CreateVirtualWindow(app);
		    windows.Add(virtualWindow);
	    }
    }
    
    private static Window CreateVirtualWindow(Microsoft.Maui.Controls.Application app)
    {
#if ANDROID
    	var services = app.Handler!.MauiContext!.Services;
    	var context = new MauiContext(services, services.GetRequiredService<global::Android.App.Activity>());
#else
	    var context = app.Handler.MauiContext;
#endif

	    // Create an Application Main Page and initialize a Handler with the Maui Context
	    var page = new ContentPage();
	    app.MainPage = page;
	    _ = page.ToPlatform(context);

	    // Create a Maui Window and initialize a Handler shim. This will expose the actual Application Window
	    var virtualWindow = new MauiWindow();
	    virtualWindow.Handler = new EmbeddedWindowHandler
	    {
#if IOS
		    PlatformView = context.Services.GetRequiredService<UIWindow>(),
#elif ANDROID
    		PlatformView = context.Services.GetRequiredService<global::Android.App.Activity>(),
#elif WINDOWS
    		PlatformView = context.Services.GetRequiredService<Microsoft.UI.Xaml.Window>(),
#endif
		    VirtualView = virtualWindow,
		    MauiContext = context
	    };
	    virtualWindow.Page = page;
	    return virtualWindow;
    }
    
    private record EmbeddingApplication(IServiceProvider Services, IApplication Application) : IPlatformApplication;
}