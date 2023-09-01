using Microsoft.UI.Xaml;

namespace MauiSampleApp.Platforms.Windows
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Microsoft.Maui.Hosting.MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
