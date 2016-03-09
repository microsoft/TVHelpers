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

using MediaAppSample.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public abstract class CarouselControlBase : ViewControlBase<MainViewModel>
    {
    }

    public sealed partial class CarouselControl : CarouselControlBase
    {
        public CarouselControl()
        {
            this.InitializeComponent();
            flipView.DataContextChanged += FlipView_DataContextChanged;
            GotFocus += CarouselControl_GotFocus;
        }

        private void FlipView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            CheckRadio(0);
        }

        private int _firsttime = 0;

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var view = sender as FlipView;
            if (view == null) return;
            var index = view.SelectedIndex;

            // workaround for issue with ContainerFromIndex
            if (_firsttime == 0)
            {
                _firsttime++;
                DelayTimer.Start(1000, (t, u) =>
                {
                    var b = Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        FlipView_SelectionChanged(sender, e);
                    });
                });
                return;
            }

            CheckRadio(index);
            AnimateTitle(index);
        }

        private void radioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            flipView.SelectedIndex = itemsControl.IndexFromContainer((sender as RadioButton).GetFirstAncestorByType<ContentPresenter>(null));
        }

        private void CheckRadio(int index)
        {
            var radioButton = ((itemsControl.ContainerFromIndex(index) as ContentPresenter).GetDescendantByName("radioButton") as RadioButton);
            if (radioButton == null) return;
            radioButton.IsChecked = true;
        }

        private void AnimateTitle(int index)
        {
            if (index == 0) return;
            var fvic = (flipView.ContainerFromIndex(index) as FlipViewItem).GetDescendantByName("flipViewItemControl") as FlipViewItemControl;
            var button = fvic?.GetDescendantByName("button") as Button;
            if (button == null) return;
            button.Focus(FocusState.Keyboard);
            VisualStateManager.GoToState(fvic, "FlipViewSelectionChanged", true);
        }

        private void CarouselControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var view = sender as CarouselControl;
            if (view == null) return;
            var index = view.flipView.SelectedIndex;
            if (index != 0) return;
            var fvic = (view.flipView.ContainerFromIndex(index) as FlipViewItem).GetDescendantByName("flipViewItemControl") as FlipViewItemControl;
            var button = fvic?.GetDescendantByName("button") as Button;
            if (button == null) return;
            button.Focus(FocusState.Keyboard);
            VisualStateManager.GoToState(fvic, "FlipViewSelectionChanged", true);
        }

    }
}
