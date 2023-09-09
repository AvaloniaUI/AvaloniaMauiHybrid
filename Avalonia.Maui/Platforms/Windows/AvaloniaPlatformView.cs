using Avalonia.Controls;
using Avalonia.Controls.Embedding;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Controls.Platform;
using Avalonia.Input.Raw;
using Avalonia.Input;
using Avalonia.Input.TextInput;
using Avalonia.Maui.Controls;
using Avalonia.OpenGL.Egl;
using Avalonia.OpenGL.Surfaces;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using SkiaSharp;

namespace Avalonia.Maui.Platforms.Windows
{
    public class AvaloniaPlatformView : View, IAvaloniaPlatformView
    {
        private readonly SKBitmap _bitmap;
        readonly VirtualAvaloniaView _mauiView;

        private EmbeddableControlRoot _root;

        public AvaloniaPlatformView(VirtualAvaloniaView mauiView)
        {
            _bitmap = new SKBitmap(100, 100);


            _mauiView = mauiView;

            _view = new ViewImpl(this);
            AddView(_view.View);

            
            _root = new EmbeddableControlRoot(new TopLevelImpl());
            _root.Prepare();

            //this.SetBackgroundColor(global::Android.Graphics.Color.Transparent);
            //OnConfigurationChanged();

        }

        public object? Content
        {
            get { return _root.Content; }
            set { _root.Content = value; }
        }

        public void UpdateContent()
        {
            Content = _mauiView.Content as Avalonia.Controls.Control;
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

        public class TopLevelImpl : ITopLevelImpl
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public object? TryGetFeature(Type featureType)
            {
                throw new NotImplementedException();
            }

            void IDisposable.Dispose()
            {
                throw new NotImplementedException();
            }

            object? IOptionalFeatureProvider.TryGetFeature(Type featureType)
            {
                throw new NotImplementedException();
            }
        }
    }
}
