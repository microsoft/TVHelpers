using MediaAppSample.Core.Models;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public class ContentItemListControl : ContentControl
    {
        public ContentItemListControl()
        {
            this.DefaultStyleKey = typeof(ContentItemListControl);
        }
        
        public ContentItemList Items
        {
            get { return (ContentItemList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ContentItemList), typeof(ContentItemListControl), new PropertyMetadata(null));
        
        public ICommand ItemCommand
        {
            get { return (ICommand)GetValue(ItemCommandProperty); }
            set { SetValue(ItemCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemCommandProperty =
            DependencyProperty.Register("ItemCommand", typeof(ICommand), typeof(ContentItemListControl), new PropertyMetadata(null));

        
        public string SeeMoreText
        {
            get { return (string)GetValue(SeeMoreTextProperty); }
            set { SetValue(SeeMoreTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeeMoreText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeeMoreTextProperty =
            DependencyProperty.Register("SeeMoreText", typeof(string), typeof(ContentItemListControl), new PropertyMetadata("See more"));



        public ICommand SeeMoreCommand
        {
            get { return (ICommand)GetValue(SeeMoreCommandProperty); }
            set { SetValue(SeeMoreCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeeMoreCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeeMoreCommandProperty =
            DependencyProperty.Register("SeeMoreCommand", typeof(ICommand), typeof(ContentItemListControl), new PropertyMetadata(null));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(ContentItemListControl), new PropertyMetadata(450));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(ContentItemListControl), new PropertyMetadata(300));



    }
}
