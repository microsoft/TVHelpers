using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    /// <summary>
    /// User control used in the ShellView page's SplitView control's pane.  Each instance represents a navigation button for the main navigation menu.
    /// </summary>
    public sealed class SplitViewButton : RadioButton
    {
        public SplitViewButton()
        {
            this.DefaultStyleKey = typeof(SplitViewButton);
        }

        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(Symbol), typeof(SplitViewButton), new PropertyMetadata(Symbol.Accept));
        public Symbol Symbol
        {
            get { return (Symbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }
    }
}
