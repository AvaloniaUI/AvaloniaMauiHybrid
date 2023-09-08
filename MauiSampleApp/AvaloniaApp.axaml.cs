using Avalonia.Markup.Xaml;

namespace MauiSample
{
    public class AvaloniaApp : Avalonia.Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();
        }
    }
}