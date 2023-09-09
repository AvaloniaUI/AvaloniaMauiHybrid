namespace Avalonia.Maui.Controls
{
    /// <summary>
    /// This is the control you place in your maui xaml.
    /// Create a visual tree with avalonia controls in it's content.
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class VirtualAvaloniaView : View
    {
        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create(nameof(Content), typeof(object), typeof(VirtualAvaloniaView), true);

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}
