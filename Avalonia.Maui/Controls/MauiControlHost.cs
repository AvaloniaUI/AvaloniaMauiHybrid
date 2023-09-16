using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;

namespace Avalonia.Maui.Controls;

public class MauiControlHost : NativeControlHost
{
    private ContentPage? _contentPage;

    public static readonly DirectProperty<MauiControlHost, ContentPage?> ContentPageProperty =
        AvaloniaProperty.RegisterDirect<MauiControlHost, ContentPage?>(nameof(ContentPage), o => o.ContentPage,
            (o, v) => o.ContentPage = v);

    [Content] // Invalidate native control on this property changed.
    public ContentPage? ContentPage
    {
        get => _contentPage;
        set => SetAndRaise(ContentPageProperty, ref _contentPage, value);
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (ContentPage is null)
        {
            return base.CreateNativeControlCore(parent);
        }

        var app = Microsoft.Maui.Controls.Application.Current!;
#if ANDROID
        var services = app.Handler!.MauiContext!.Services;
        var mauiContext = new MauiContext(services, services.GetRequiredService<global::Android.App.Activity>());
#else
        var mauiContext = app.Handler!.MauiContext!;
#endif

        ContentPage.Parent = app.Windows[0];

        var native = ContentPage.ToPlatform(mauiContext);

#if ANDROID
        return new Android.AndroidViewControlHandle(native);
#elif IOS
        return new iOS.UIViewControlHandle(native);
#else
        return base.CreateNativeControlCore(parent);
#endif
    }
}