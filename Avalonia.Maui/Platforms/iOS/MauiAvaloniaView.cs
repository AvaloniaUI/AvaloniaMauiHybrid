using Avalonia.Maui.Controls;

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
            Content = _mauiView.Content as Avalonia.Controls.Control;
        }
    }
}
