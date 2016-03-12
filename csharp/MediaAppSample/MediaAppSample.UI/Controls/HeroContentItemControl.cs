using MediaAppSample.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public sealed class HeroContentItemControl : Control
    {
        public ContentItemBase Item
        {
            get { return (ContentItemBase)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(ContentItemBase), typeof(HeroContentItemControl), new PropertyMetadata(null));



        public HeroContentItemControl()
        {
            this.DefaultStyleKey = typeof(HeroContentItemControl);
        }
    }
}
