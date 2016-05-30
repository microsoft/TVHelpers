using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public class Rating : Control
    {
        public Rating()
        {
            this.DefaultStyleKey = typeof(Rating);
            this.SizeChanged += Rating_SizeChanged;
        }

        private void Rating_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update(this);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Rating), new PropertyMetadata(0, new PropertyChangedCallback(ValueChanged)));



        private static void ValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                var rc = sender as Rating;
                if(rc != null)
                    Update(rc);
            }
        }

        private static void Update(Rating rc)
        {
            var spOutline = rc?.GetDescendantByName("spOutline") as StackPanel;
            var spSolid = rc?.GetDescendantByName("spSolid") as StackPanel;

            if (spOutline != null && spSolid != null)
            {
                if (rc.Value <= rc.MaxValue)
                    spSolid.Width = (rc.Value / rc.MaxValue) * spOutline.ActualWidth;
                else
                    spSolid.Width = spOutline.ActualWidth;
            }
        }

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(Rating), new PropertyMetadata(5.0, new PropertyChangedCallback(ValueChanged)));
    }
}
