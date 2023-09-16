using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}