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

using MediaAppSample.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public sealed partial class Carousel : UserControl
    {
        private bool _isLoaded = false;

        public Carousel()
        {
            this.InitializeComponent();
            this.Loaded += Carousel_Loaded;
        }

        private void Carousel_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            this.Init();
            this.Loaded -= Carousel_Loaded;
        }

        private void Init()
        {
            if (_isLoaded == false)
                return;
            
            // Ensure the first item is selected in the control
            if (this.Data != null && this.Data.Count > 0)
            {
                var fv = this.GetDescendantByName("carousel") as FlipView;
                if (fv?.Items.Count > 0)
                    fv.SelectedIndex = 0;
            }
        }

        public ContentItemList Data
        {
            get { return (ContentItemList)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(ContentItemList), typeof(Carousel), new PropertyMetadata(null, new PropertyChangedCallback(DataChanged)));

        private static void DataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Update the control to select the first item in the list
            (sender as Carousel)?.Init();
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(Carousel), new PropertyMetadata(null));
    }
}
