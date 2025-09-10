#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Maui.Platforms.Windows
{
    internal static class AppActionsPatch
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;   // custom identifier
            public int cbData;      // size of data in bytes
            public IntPtr lpData;   // pointer to data
        }

        static string AppActionPrefix = "XE_APP_ACTIONS-";

        public static void Patch()
        {
            var proc = Process.GetCurrentProcess();
            //get all other (possible) running instances
            Process[] processes = Process.GetProcessesByName(proc.ProcessName);

            if (processes.Length > 1)
            {
                //iterate through all running target applications      
                foreach (Process p in processes)
                {
                    if (p.Id != proc.Id)
                    {
                        var args = Environment.GetCommandLineArgs();
                        var arg = args.FirstOrDefault(a => a.StartsWith(AppActionPrefix));
                        if (arg != null)
                        {
                            SendMessage(p.MainWindowHandle, arg);
                        }                        
                    }
                }
            }

        }

        private static void SendMessage(IntPtr handle, string args)
        {
            IntPtr lpData = Marshal.StringToHGlobalUni(args);

            var cds = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero, // you can put your message type here
                cbData = (args.Length) * 2, // Unicode bytes, + null terminator
                lpData = lpData
            };

            SendMessage(handle, WM_COPYDATA, IntPtr.Zero, ref cds);

            Marshal.FreeHGlobal(lpData);

            Environment.Exit(0);
        }

        public static void Register()
        {
            if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Startup += (_, _) =>
                {
                    Avalonia.Controls.Win32Properties.AddWndProcHookCallback(desktop.MainWindow!, WndProc);
                };
            }
        }
        
        private async static Task Decode(IntPtr msg)
        {
            var cds = Marshal.PtrToStructure<COPYDATASTRUCT>(msg);
            string received = Marshal.PtrToStringUni(cds.lpData, cds.cbData / 2);
            var args = Decode(received);
            var actions = await AppActions.GetAsync();
            var action = actions.FirstOrDefault(a => a.Id == args);
            if (action != null)
            {
                RaiseOnAction(action);
            }
        }

        private static string Decode(string args)
        {
            return Encoding.Default.GetString(Convert.FromBase64String(args.Substring(AppActionPrefix.Length)));
        }

        private static IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_COPYDATA)
            {
                MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Decode(lParam);
                });
                handled = true;
            }

            return IntPtr.Zero;
        }

        private static void RaiseOnAction(AppAction action)
        {
            var eventField = AppActions.Current.GetType().GetField("AppActionActivated", BindingFlags.Instance | BindingFlags.NonPublic);
            if (eventField != null)
            {
                var eventDelegate = eventField.GetValue(AppActions.Current) as MulticastDelegate;
                eventDelegate?.DynamicInvoke(AppActions.Current, new AppActionEventArgs(action));
            }
        }
    }
}
#endif