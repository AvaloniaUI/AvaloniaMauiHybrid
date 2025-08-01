#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Maui.Controls;

namespace Avalonia.Maui.Windows
{
    public partial class MauiAvaloniaView : Microsoft.UI.Xaml.Controls.ContentControl
    {
        readonly AvaloniaView _mauiView;

        public MauiAvaloniaView(AvaloniaView mauiView)
        {
            _mauiView = mauiView;
        }
     
        public void UpdateContent()
        {
            Content = Content;
        }
    }
}
#endif