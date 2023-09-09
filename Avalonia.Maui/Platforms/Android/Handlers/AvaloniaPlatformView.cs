using Android.Content;
using Avalonia.Maui.Controls;
using Color = Android.Graphics.Color;

namespace Avalonia.Maui.Platforms.Android.Handlers
{
    public class AvaloniaPlatformView : Avalonia.Android.AvaloniaView, IAvaloniaPlatformView
    {
        readonly VirtualAvaloniaView _mauiView;

        public AvaloniaPlatformView(Context context, VirtualAvaloniaView mauiView) : base(context)
        {
            _mauiView = mauiView;

            SetBackgroundColor(Color.Transparent);
        }

        public void UpdateContent()
        {
            Content = _mauiView.Content;
        }
    }
}
