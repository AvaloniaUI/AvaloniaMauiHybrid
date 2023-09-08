using Avalonia.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace Avalonia.Maui.Handlers
{
    public partial class AvaloniaViewHandler : ViewHandler<AvaloniaView, Microsoft.UI.Xaml.Controls.Frame>
    {
        private Microsoft.UI.Xaml.Controls.Frame _control;

        protected override Microsoft.UI.Xaml.Controls.Frame CreatePlatformView()
        {
            return _control = new Microsoft.UI.Xaml.Controls.Frame();
        }

        protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.Frame platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(Microsoft.UI.Xaml.Controls.Frame platformView)
        {
            base.DisconnectHandler(platformView);
        }

        public static void MapContent(AvaloniaViewHandler handler, AvaloniaView view)
        {
            //handler.PlatformView?.UpdateContent();
        }

        public override Microsoft.Maui.Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }
    }
}
