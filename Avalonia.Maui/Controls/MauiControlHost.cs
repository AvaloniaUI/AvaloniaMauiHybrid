using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Runtime.InteropServices;
using ContentView = Microsoft.Maui.Controls.ContentView;

namespace Avalonia.Maui.Controls;

public class MauiControlHost : NativeControlHost
{
#if WINDOWS10_0_19041_0_OR_GREATER
    const int GWL_STYLE = -16;
    const uint WS_CAPTION = 0x00C00000;      // WS_BORDER | WS_DLGFRAME
    const uint WS_THICKFRAME = 0x00040000;

     [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    public static void RemoveBorder(IntPtr hwnd)
    {
        uint style = GetWindowLong(hwnd, GWL_STYLE);
        style &= ~WS_CAPTION;
        style &= ~WS_THICKFRAME;
        SetWindowLong(hwnd, GWL_STYLE, style);
    }

#endif

    private View? _content;
    private ContentView? _page;

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
#if ANDROID || IOS ||WINDOWS10_0_19041_0_OR_GREATER
        var app = Microsoft.Maui.Controls.Application.Current!;
#if ANDROID
        var services = app.Handler!.MauiContext!.Services;
        var mauiContext = new MauiContext(services, services.GetRequiredService<global::Android.App.Activity>());
#else
        var mauiContext = app.Handler!.MauiContext!;
#endif

        var pageHandler = new ContentViewHandler
        {
#if IOS
            ViewController = ((iOS.UIViewControlHandle)parent).View.Window.RootViewController,
#endif
        };
        pageHandler.SetMauiContext(mauiContext);

        _page = new ContentView
        {
            Handler = pageHandler,
            Parent = app.Windows[0],
            Content = Content
        };

        var native = _page.ToPlatform(mauiContext);

#if ANDROID
        return new Android.AndroidViewControlHandle(native);
#elif IOS
        return new iOS.UIViewControlHandle(native);
#elif WINDOWS10_0_19041_0_OR_GREATER

        var handle = MainThread.InvokeOnMainThreadAsync(() =>
        {
            var window = new MauiWinUIWindow();
            window.Content = native;
            RemoveBorder(window.WindowHandle);

            if (window.Content is Microsoft.UI.Xaml.FrameworkElement rootElement)
            {
                // Apply theme
                bool isDark = (PlatformThemeVariant?)Avalonia.Application.Current!.ActualThemeVariant == PlatformThemeVariant.Dark;
                rootElement.RequestedTheme = isDark ? Microsoft.UI.Xaml.ElementTheme.Dark
                    : Microsoft.UI.Xaml.ElementTheme.Light;
            }

            return window.WindowHandle;

        }).Result;

        return new PlatformHandle(handle, "HWMD");
#endif
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