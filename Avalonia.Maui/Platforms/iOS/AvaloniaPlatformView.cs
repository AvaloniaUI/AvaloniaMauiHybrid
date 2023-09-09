using Avalonia.Maui.Controls;

namespace Avalonia.Maui.Platforms.iOS
{
    public class AvaloniaPlatformView : Avalonia.iOS.AvaloniaView, IAvaloniaPlatformView
    {
        readonly VirtualAvaloniaView _mauiView;

        public AvaloniaPlatformView(VirtualAvaloniaView mauiView)
        {
            _mauiView = mauiView;
        }

        public void UpdateContent()
        {
            Content = _mauiView.Content as Avalonia.Controls.Control;
        }
    }
}
