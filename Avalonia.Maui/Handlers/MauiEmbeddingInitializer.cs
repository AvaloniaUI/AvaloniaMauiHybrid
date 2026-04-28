using System;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

namespace Avalonia.Maui.Handlers;

internal class MauiEmbeddingInitializer : IMauiInitializeService
{
    private readonly Application _app;

    public MauiEmbeddingInitializer(Application app)
    {
        _app = app;
    }

    public void Initialize(IServiceProvider services)
    {
        var iApp = services.GetRequiredService<IApplication>();
        if (iApp is Microsoft.Maui.Controls.Application mauiApp)
        {
            mauiApp.MainPage = new Page();

            _app.ActualThemeVariantChanged += (_, _) => SetAppTheme();
            SetAppTheme();

            void SetAppTheme()
            {
                mauiApp.UserAppTheme = (PlatformThemeVariant?)_app.ActualThemeVariant == PlatformThemeVariant.Dark
                    ? AppTheme.Dark
                    : AppTheme.Light;
            }
        }
    }
}