using Microsoft.Maui;

namespace Avalonia.Maui.Handlers;

internal class EmbeddedWindowHandler : IElementHandler
{
    public object? PlatformView { get; set; }
    public IElement? VirtualView { get; set; }
    public IMauiContext? MauiContext { get; set; }

    public void DisconnectHandler() { }
    public void Invoke(string command, object? args = null)
    {
        // Required to prevent exception when hot reload is invoked
        if(args is RetrievePlatformValueRequest<float> request)
        {
            request.TrySetResult(1f);
        }
    }
    public void SetMauiContext(IMauiContext mauiContext) => MauiContext = mauiContext;
    public void SetVirtualView(IElement view) => VirtualView = view;
    public void UpdateValue(string property) { }
}