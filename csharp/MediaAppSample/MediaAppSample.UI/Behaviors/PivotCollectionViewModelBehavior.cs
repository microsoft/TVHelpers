//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

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
