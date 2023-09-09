using Avalonia.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace Avalonia.Maui.Handlers
{
    public partial class AvaloniaViewHandler
    {
        public static IPropertyMapper<VirtualAvaloniaView, AvaloniaViewHandler> PropertyMapper = new PropertyMapper<VirtualAvaloniaView, AvaloniaViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(VirtualAvaloniaView.Content)] = MapContent
        };

        public AvaloniaViewHandler() : base(PropertyMapper)
        {
        }
    }
}
