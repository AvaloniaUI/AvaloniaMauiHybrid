using Microsoft.Maui.Dispatching;
using System;

namespace Avalonia.Maui.Platforms.Windows
{
    internal class AppDispatcher : IDispatcher
    {
        public bool IsDispatchRequired => Avalonia.Threading.Dispatcher.UIThread.CheckAccess();

        public IDispatcherTimer CreateTimer()
        {
            throw new NotImplementedException();
        }

        public bool Dispatch(Action action)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(action);
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            throw new NotImplementedException();
        }
    }
}
