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

using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public class SectionReadOnlyList : ContentControl
    {
        private static ResourceLoader resourceLoader = new ResourceLoader("MediaAppSample.Core/Resources");

        public SectionReadOnlyList()
        {
            this.DefaultStyleKey = typeof(SectionReadOnlyList);

        }

        public object Items
        {
            get { return (object)GetValue(ItemsProperty); }
            set {
                SetValue(FilteredItemsProperty, value);
                SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(object), typeof(SectionReadOnlyList), new PropertyMetadata(null));

        public static readonly DependencyProperty FilteredItemsProperty =
            DependencyProperty.Register("FilteredItems", typeof(object), typeof(SectionReadOnlyList), new PropertyMetadata(null));

        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(SectionReadOnlyList), new PropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(SectionReadOnlyList), new PropertyMetadata(null));


        public string SeeMoreText
        {
            get { return (string)GetValue(SeeMoreTextProperty); }
            set { SetValue(SeeMoreTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeeMoreText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeeMoreTextProperty =
            DependencyProperty.Register("SeeMoreText", typeof(string), typeof(SectionReadOnlyList), new PropertyMetadata(resourceLoader.GetString("TextSeeMore")));



        public ICommand SeeMoreCommand
        {
            get { return (ICommand)GetValue(SeeMoreCommandProperty); }
            set { SetValue(SeeMoreCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeeMoreCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeeMoreCommandProperty =
            DependencyProperty.Register("SeeMoreCommand", typeof(ICommand), typeof(SectionReadOnlyList), new PropertyMetadata(null));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(SectionReadOnlyList), new PropertyMetadata(450));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(SectionReadOnlyList), new PropertyMetadata(300));

        public static readonly DependencyProperty IsFilteredProperty =
            DependencyProperty.Register("IsFiltered", typeof(bool), typeof(SectionReadOnlyList), new PropertyMetadata(false));

        public int TakeN
        {
            get { return (int)GetValue(TakeNProperty); }
            set
            {
                SetValue(TakeNProperty, value);
                var items = (object)GetValue(ItemsProperty);
                if (items != null && items is IEnumerable<object> && value > 0 && value < (items as IEnumerable<object>).Count())
                {
                    SetValue(FilteredItemsProperty, (items as IEnumerable<object>).Take(value));
                    SetValue(IsFilteredProperty, true);
                }
                else
                {
                    SetValue(FilteredItemsProperty, items);
                    SetValue(IsFilteredProperty, false);
                }
            }
        }
        
        public static readonly DependencyProperty TakeNProperty =
            DependencyProperty.Register("TakeN", typeof(int), typeof(SectionReadOnlyList), new PropertyMetadata(-1));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SectionReadOnlyList), new PropertyMetadata(string.Empty));
    }
}