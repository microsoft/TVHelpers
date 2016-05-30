// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public class ContentItemListControl : ContentControl
    {
        private static readonly ResourceLoader resourceLoader = new ResourceLoader("MediaAppSample.Core/Resources");

        public ContentItemListControl()
        {
            this.DefaultStyleKey = typeof(ContentItemListControl);
        }
        
        public object Items
        {
            get { return (object)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(object), typeof(ContentItemListControl), new PropertyMetadata(null));
        
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
            DependencyProperty.Register("SeeMoreText", typeof(string), typeof(ContentItemListControl), new PropertyMetadata(resourceLoader.GetString("TextSeeMore"))); 
        

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


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ContentItemListControl), new PropertyMetadata(null));


        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(ContentItemListControl), new PropertyMetadata(null));

        public StyleSelector ItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { SetValue(ItemContainerStyleSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemContainerStyleSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register("ItemContainerStyleSelector", typeof(StyleSelector), typeof(ContentItemListControl), new PropertyMetadata(null));


    }
}
