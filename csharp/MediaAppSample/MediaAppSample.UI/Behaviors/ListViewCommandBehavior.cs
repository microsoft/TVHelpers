using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Behaviors
{
    /// <summary>
    /// Creates an attached property for all ListViewBase controls allowing binding  a command object to it's ItemClick event.
    /// </summary>
    public static class ListViewCommandBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(ListViewCommandBehavior), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (ListViewBase)d;
            if (control != null)
            {
                // Remove the old click handler if there was a previous command
                if (e.OldValue != null)
                    control.ItemClick -= OnItemClick;

                if(e.NewValue != null)
                    control.ItemClick += OnItemClick;
            }
        }

        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var control = (ListViewBase)sender;
            if (control != null)
            {
                var command = GetCommand(control);

                if (command != null && command.CanExecute(e.ClickedItem))
                    command.Execute(e.ClickedItem);
            }
        }
    }
}
