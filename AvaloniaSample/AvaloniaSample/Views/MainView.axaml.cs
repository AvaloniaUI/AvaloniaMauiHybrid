using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Maui.Controls;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.ProgressBar;

namespace AvaloniaSample.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void MediaElement_OnPositionChanged(object? sender, MediaPositionChangedEventArgs e)
    {
        var mediaElement = (MediaElement)sender!;
        var progressBar = (SfCircularProgressBar)this.Get<MauiControlHost>("ProgressBarHost").Content!;
        progressBar.IsIndeterminate = false;
        progressBar.Minimum = 0;
        progressBar.Maximum = 1;
        progressBar.Progress = e.Position / mediaElement.Duration;
    }
}