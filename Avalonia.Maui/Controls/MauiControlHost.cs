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
    private View? _content;
    private ContentPage? _page;

    public static readonly DirectProperty<MauiControlHost, View?> ContentProperty =
        AvaloniaProperty.RegisterDirect<MauiControlHost, View?>(nameof(ContentPage), o => o.Content,
            (o, v) => o.Content = v);

    [Content]
    public View? Content
    {
        get => _content;
        set => SetAndRaise(ContentProperty, ref _content, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            if (_page is not null)
            {
                _page.Content = change.GetNewValue<View?>();
            }
        }
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _page = new ContentPage
        {
            Content = Content
        };

        var app = Microsoft.Maui.Controls.Application.Current!;
#if ANDROID
        var services = app.Handler!.MauiContext!.Services;
        var mauiContext = new MauiContext(services, services.GetRequiredService<global::Android.App.Activity>());
#else
        var mauiContext = app.Handler!.MauiContext!;
#endif
        
        _page.Parent = app.Windows[0];

        var native = _page.ToPlatform(mauiContext);

#if ANDROID
        return new Android.AndroidViewControlHandle(native);
#elif IOS
        return new iOS.UIViewControlHandle(native);
#else
        return base.CreateNativeControlCore(parent);
#endif
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        if (_page is not null)
        {
            _page.Content = null;
            _page.Parent = null;
            _page = null;
        }

        base.DestroyNativeControlCore(control);
    }
}