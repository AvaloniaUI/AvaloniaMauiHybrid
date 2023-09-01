using Avalonia.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace Avalonia.Maui.Handlers
{
    public partial class AvaloniaViewHandler : ViewHandler<AvaloniaView, Microsoft.UI.Xaml.Controls.Frame>
    {
        protected override Microsoft.UI.Xaml.Controls.Frame CreatePlatformView()
        {
            return new Microsoft.UI.Xaml.Controls.Frame();
        }

        public static void MapContent(AvaloniaViewHandler handler, AvaloniaView view)
        {
            //handler.PlatformView?.UpdateContent();
        }
    }
}
