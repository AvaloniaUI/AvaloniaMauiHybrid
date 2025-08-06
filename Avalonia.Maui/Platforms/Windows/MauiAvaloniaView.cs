#if WINDOWS10_0_19041_0_OR_GREATER
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Maui.Controls;
using Avalonia.Platform;
using Avalonia.Skia.Helpers;
using Avalonia.Styling;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using WinRT.Interop;
using WPoint = Windows.Foundation.Point;
namespace Avalonia.Maui.Windows
{
    public partial class MauiAvaloniaView : Microsoft.UI.Xaml.Controls.ContentControl
    {
        [Serializable]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point minPosition;
            public System.Drawing.Point maxPosition;
            public System.Drawing.Rectangle normalPosition;
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        public const int SW_SHOWNORMAL = 1;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern uint GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);


        readonly AvaloniaView _mauiView;

        public MauiAvaloniaView(AvaloniaView mauiView)
        {
            _mauiView = mauiView;
        }

        public void UpdateContent()
        {
            var maui = Microsoft.Maui.Controls.Application.Current!.Windows.LastOrDefault();
            if (maui is null)
                return;

            var window = (Microsoft.UI.Xaml.Window)maui.Handler.PlatformView!;
            if (window is null)
                return;

            var clears = new List<Action>();
            EventHandler? destroying = null!;
            maui.Destroying += destroying = (s, e) =>
            {
                clears.ForEach(c => c());
                clears.Clear();
            };
            clears.Add(() => maui.Destroying -= destroying);

            var avaWindow = new Avalonia.Controls.Window()
            {
                Content = Content,
                MaxHeight = window.Bounds.Height,
                MaxWidth = window.Bounds.Width,
                SizeToContent = SizeToContent.WidthAndHeight,
                ExtendClientAreaToDecorationsHint = true,
                SystemDecorations = SystemDecorations.None,
                WindowState = WindowState.Normal
            };

            Application.Current!.RequestedThemeVariant =
                Microsoft.Maui.Controls.Application.Current.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark ?
                (ThemeVariant)PlatformThemeVariant.Dark
                : (ThemeVariant)PlatformThemeVariant.Light;

            avaWindow.Show();

            IntPtr hwnd = WindowNative.GetWindowHandle(window);
            var handle = avaWindow.TryGetPlatformHandle()!.Handle;

            if (avaWindow.DesiredSize.Height > window.Bounds.Height)
                avaWindow.Height = window.Bounds.Height;
            if (avaWindow.DesiredSize.Width > window.Bounds.Width)
                avaWindow.Width = window.Bounds.Width;

            this.Width = avaWindow.DesiredSize.Width;
            this.Height = avaWindow.DesiredSize.Height;

            var canvas = new SKXamlCanvas()
            {
                Height = avaWindow.Height,
                Width = avaWindow.Width,
            };
            EventHandler<SKPaintSurfaceEventArgs>? paintSurface = null!;
            canvas.PaintSurface += paintSurface = (s, e) =>
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
                DrawingContextHelper.RenderAsync(e.Surface.Canvas, avaWindow);
            };
            clears.Add(() => canvas.PaintSurface -= paintSurface);
            var wp = new WINDOWPLACEMENT
            {
                length = Marshal.SizeOf(typeof(WINDOWPLACEMENT)),
                showCmd = SW_SHOWNORMAL
            };

            bool update = true;
            PointerEventHandler? pointerEntered = null!;
            canvas.PointerEntered += (s, e) =>
            {
                Show();
                //if (update)
                    Update();
            };
            clears.Add(() => canvas.PointerEntered -= pointerEntered);

            SizeChangedEventHandler? sizeChanged = null!;
            this.SizeChanged += sizeChanged =(s, e) =>
            {
                update = true;

                if (Microsoft.Maui.Controls.Application.Current.Windows.Count == 0)
                    return;

                canvas.Width = avaWindow.Width = avaWindow.MaxWidth = e.NewSize.Width;
                canvas.Height = avaWindow.Height = avaWindow.MaxHeight = e.NewSize.Height;

                Update();
            };
            clears.Add(() => this.SizeChanged -= sizeChanged);

            EventHandler<PointerEventArgs>? pointerExited = null!;
            avaWindow.PointerExited += pointerExited  =(s, e) =>
            {
                update = false;
                Hide();
            };
            clears.Add(() => avaWindow.PointerExited -= pointerExited);

            var root = window.Content;
            if (root != null)
            {
                //Security to avoid Airspace issues
                PointerEventHandler? pointerMoved = null!;
                root.PointerMoved += pointerMoved =(s, e) =>
                {
                    // Get position relative to your control
                    var point = e.GetCurrentPoint(this);

                    // Check if pointer is outside the control bounds
                    if (point.Position.X < 0 || point.Position.Y < 0 ||
                        point.Position.X > this.ActualWidth ||
                        point.Position.Y > this.ActualHeight)
                    {
                        // Pointer is outside the control
                        Hide();
                    }
                };
                clears.Add(() => root.PointerMoved -= pointerMoved);
            }


            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    if (Microsoft.Maui.Controls.Application.Current.Windows.Count == 0)
                        continue;

                    if (canvas is not null && canvas.DispatcherQueue is not null)
                        canvas.Invalidate();
                    Thread.Sleep(100);
                }
            });

            SetParent(handle, hwnd);
            Update();
            ShowWindow(handle, 0);


            Content = canvas;

            void Show()
            {
                wp.showCmd = SW_SHOWNORMAL;
                var point = new System.Drawing.Point();
                ClientToScreen(handle, ref point);
                wp.normalPosition.X -= point.X;
                wp.normalPosition.Y -= point.Y;
                SetWindowPlacement(handle, ref wp);
            }

            void Hide()
            {
                wp.showCmd = 0;
                SetWindowPlacement(handle, ref wp);
            }

            void Update()
            {
                uint dpi = GetDpiForWindow(hwnd);
                double dpiScale = dpi / 96.0;

                var transform = this.TransformToVisual(null);
                var newPosition = transform.TransformPoint(new WPoint(0, 0));
                if (newPosition.X == 0 && newPosition.Y == 0)
                    return;
                
                int screenX = (int)(newPosition.X * dpiScale);
                int screenY = (int)(newPosition.Y * dpiScale);
                int width = (int)(this.ActualWidth * dpiScale);
                int height = (int)(this.ActualHeight * dpiScale);

                
                SetWindowPos(handle, IntPtr.Zero, screenX, screenY, width, height, 0x0000);

                GetWindowPlacement(handle, ref wp);

                Debug.WriteLine($"Window moved to {screenX}, {screenY} with size {width}x{height} at DPI {dpiScale}");
            }
        }
    }
}
#endif