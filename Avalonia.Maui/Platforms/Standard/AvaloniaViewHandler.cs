#if !ANDROID && !IOS && !WINDOWS && !MACOS && !MACCATALYST
using Avalonia.Maui.Controls;
using Avalonia.Maui.Platforms.Standard;
using Microsoft.Maui.Handlers;

namespace Avalonia.Maui.Handlers
{
    public partial class AvaloniaViewHandler : ViewHandler<AvaloniaView, MauiAvaloniaView>
    {
        protected override MauiAvaloniaView CreatePlatformView()
        {
            return new MauiAvaloniaView(VirtualView);
        }

        protected override void ConnectHandler(MauiAvaloniaView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.Content = VirtualView.Content as MauiAvaloniaView;
        }
        
        public static void MapContent(AvaloniaViewHandler handler, AvaloniaView view)
        {
            handler.PlatformView?.UpdateContent();
        }
    }
}
#endif