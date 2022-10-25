using Android.Content;
using Android.Graphics;
using Avalonia.Maui.Controls;
using Color = Android.Graphics.Color;

namespace Avalonia.Maui.Platforms.Android.Handlers
{
    public class MauiAvaloniaView : Avalonia.Android.AvaloniaView
    {
        private AvaloniaView _mauiView;

        public MauiAvaloniaView(Context context, AvaloniaView mauiView) : base(context)
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
