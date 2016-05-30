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

using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using Microsoft.Xaml.Interactivity;
using System;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Behaviors
{
    /// <summary>
    /// Enables a pivot control that is bound to a CollectionViewModelBase to call update the current view model of the collection as each pivot chanage happens. This in 
    /// turn also fires the LoadState (OnNavigatedTo) event only when the pivot is loaded into view.
    /// </summary>
    public class PivotCollectionViewModelBehavior : Behavior<Pivot>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PivotItemLoading += AssociatedObject_PivotItemLoading;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PivotItemLoading -= AssociatedObject_PivotItemLoading;
            base.OnDetaching();
        }

        private async void AssociatedObject_PivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            var parentVM = AssociatedObject.DataContext as CollectionViewModelBase;
            if (parentVM != null)
            {
                if (args.Item.DataContext is ViewModelBase)
                {
                    var vm = args.Item.DataContext as ViewModelBase;
                    try
                    {
                        await parentVM.SetCurrentAsync(vm);
                    }
                    catch (Exception ex)
                    {
                        Platform.Current.Logger.LogError(ex, "Error while loading child ViewModel of type {1} in CollectionViewModel type {0}", parentVM.GetType().Name, vm.GetType().Name);
                    }
                }
            }
        }
    }
}
