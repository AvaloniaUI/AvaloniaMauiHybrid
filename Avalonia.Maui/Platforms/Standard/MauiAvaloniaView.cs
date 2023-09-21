#if !ANDROID && !IOS && !WINDOWS && !MACOS && !MACCATALYST
using Avalonia.Controls;
using Avalonia.Maui.Controls;
using AvaloniaControl = Avalonia.Controls.Control;

namespace Avalonia.Maui.Platforms.Standard
{
    public class MauiAvaloniaView : ContentControl
    {
        readonly AvaloniaView _mauiView;

        public MauiAvaloniaView(AvaloniaView mauiView)
        {
            _mauiView = mauiView;
        }

        public void UpdateContent()
        {
            Content = _mauiView.Content as AvaloniaControl;
        }
    }
}
#endif