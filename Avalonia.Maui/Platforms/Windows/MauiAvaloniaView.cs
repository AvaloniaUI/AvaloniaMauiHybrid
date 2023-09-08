using Avalonia.Input.TextInput;
using Avalonia.Maui.Controls;

namespace Avalonia.Maui.Platforms.Windows
{
    public class MauiAvaloniaView : Frame
    {
        readonly AvaloniaView _mauiView;

        public MauiAvaloniaView(AvaloniaView mauiView)
        {
            _mauiView = mauiView;
        }

        public void UpdateContent()
        {
            //Content = _mauiView.Content as Avalonia.Controls.Control;
        }

        // Avalonia.Input.TextInput.ITextInputMethodImpl
        public void SetClient(TextInputMethodClient? client)
        {

        }

        public void SetCursorRect(Rect rect)
        {

        }

        public void SetOptions(TextInputOptions options)
        {

        }

        public void Reset()
        {

        }
    }
}
