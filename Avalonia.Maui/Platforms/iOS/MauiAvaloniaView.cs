using Avalonia.Maui.Controls;
using AvaloniaControl = Avalonia.Controls.Control;

namespace Avalonia.Maui.Platforms.iOS
{
    public class MauiAvaloniaView : Avalonia.iOS.AvaloniaView
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
