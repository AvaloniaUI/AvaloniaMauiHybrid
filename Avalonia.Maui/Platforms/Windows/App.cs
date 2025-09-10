#if WINDOWS10_0_19041_0_OR_GREATER
using Microsoft.UI.Dispatching;
using System.Runtime.InteropServices;
using System.Threading;
using WinRT;

namespace Avalonia.Maui.Platforms.Windows
{
    public partial class App : Microsoft.UI.Xaml.Application
    {
        [DllImport("Microsoft.ui.xaml.dll")]
        private static extern void XamlCheckProcessRequirements();

        public static void Start()
        {
            AppActionsPatch.Patch();

            var thread = new Thread(() =>
            {
                XamlCheckProcessRequirements();

                ComWrappersSupport.InitializeComWrappers();
                Start((p) =>
                {
                    SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread()));
                    _ = new App();
                });
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private App()
        {

        }
    }
}
#endif
