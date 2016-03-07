using Microsoft.Xaml.Interactivity;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MediaAppSample.UI.Behaviors
{
    /// <summary>
    /// Enables a control to execute its associated command when an enter key press is detected within the control.
    /// </summary>
    public class EnterKeyBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            AssociatedObject.KeyUp += AssociatedObject_KeyUp;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
            base.OnDetaching();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EnterKeyBehavior), null);

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EnterKeyBehavior), null);

        private void AssociatedObject_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (this.Command != null && this.Command.CanExecute(this.CommandParameter))
                    this.Command.Execute(this.CommandParameter);
                else
                    FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
            }
        }
    }
}
