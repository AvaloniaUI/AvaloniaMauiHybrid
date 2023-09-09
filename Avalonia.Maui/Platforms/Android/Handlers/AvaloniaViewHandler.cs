using Avalonia.Layout;
using Avalonia.Maui.Platforms.Android.Handlers;
using Microsoft.Maui.Handlers;

namespace Avalonia.Maui.Handlers
{
    public partial class AvaloniaViewHandler : ViewHandler<Avalonia.Maui.Controls.VirtualAvaloniaView, AvaloniaPlatformView>
    {
        protected override AvaloniaPlatformView CreatePlatformView()
        {
            return new AvaloniaPlatformView(Context, VirtualView);
        }

        protected override void ConnectHandler(AvaloniaPlatformView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.Content = VirtualView.Content;
        }

        protected override void DisconnectHandler(AvaloniaPlatformView platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        public static void MapContent(AvaloniaViewHandler handler, Controls.VirtualAvaloniaView view)
        {
            handler.PlatformView?.UpdateContent();
        }

        public override Microsoft.Maui.Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (VirtualView.Content is Layoutable control)
            {
                control.Measure(new Size(widthConstraint, heightConstraint));

                base.GetDesiredSize(control.DesiredSize.Width, control.DesiredSize.Height);

                return new Microsoft.Maui.Graphics.Size(control.DesiredSize.Width, control.DesiredSize.Height);
            }
            else
            {
                return base.GetDesiredSize(widthConstraint, heightConstraint);
            }
        }
    }
}
