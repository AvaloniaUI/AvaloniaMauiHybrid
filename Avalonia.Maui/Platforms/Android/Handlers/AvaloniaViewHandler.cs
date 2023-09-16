#if ANDROID
using Avalonia.Layout;
using Avalonia.Maui.Platforms.Android.Handlers;
using Microsoft.Maui.Handlers;
using AvaloniaView = Avalonia.Maui.Controls.AvaloniaView;

namespace Avalonia.Maui.Handlers
{
    public partial class AvaloniaViewHandler : ViewHandler<AvaloniaView, MauiAvaloniaView>
    {
        protected override MauiAvaloniaView CreatePlatformView()
        {
            return new MauiAvaloniaView(Context, VirtualView);
        }

        protected override void ConnectHandler(MauiAvaloniaView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.Content = VirtualView.Content;
        }

        protected override void DisconnectHandler(MauiAvaloniaView platformView)
        {
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }

        public static void MapContent(AvaloniaViewHandler handler, AvaloniaView view)
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
#endif