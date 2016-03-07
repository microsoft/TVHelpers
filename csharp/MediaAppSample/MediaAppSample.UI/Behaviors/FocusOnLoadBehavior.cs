using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Behaviors
{
    /// <summary>
    /// Focuses a control on the loaded event.  Useful for setting focus to the first textbox on a page, for example, immediately on load of the page.
    /// </summary>
    public class FocusOnLoadBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnDetaching();
        }
        
        private void AssociatedObject_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AssociatedObject.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }
    }
}
