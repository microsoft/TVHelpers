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
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class ShellViewBase : ViewBase<ShellViewModel>
    {
    }
    
    public sealed partial class ShellView : ShellViewBase
    {
        #region Constructors

        public ShellView()
        {
            this.InitializeComponent();

            SuspensionManager.RegisterFrame(bodyFrame, "ShellBodyFrame");

            this.KeyDown += ShellView_KeyDown;

            this.Loaded += (sender, e) =>
            {
                bodyFrame.Navigated += BodyFrame_Navigated;
                this.UpdateSelectedMenuItem();
            };

            this.Unloaded += (sender, e) => 
            {
                bodyFrame.Navigated -= BodyFrame_Navigated;
            };
        }

        #endregion

        #region Methods

        protected override void OnApplicationResuming()
        {
            // Set the child frame to the navigation service so that it can appropriately perform navigation of pages to the desired frame.
            this.Frame.SetChildFrame(bodyFrame);

            base.OnApplicationResuming();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the child frame to the navigation service so that it can appropriately perform navigation of pages to the desired frame.
            this.Frame.SetChildFrame(bodyFrame);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Remove the bodyFrame as the childFrame
            this.Frame.SetChildFrame(null);
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new ShellViewModel());

            return base.OnLoadStateAsync(e);
        }

        /// <summary>
        /// Update the selected item in the shell navigation menu based on watching the body frame navigated event.
        /// </summary>
        private void UpdateSelectedMenuItem()
        {
            var view = bodyFrame.Content;
            if (view is SettingsView)
                btnSettings.IsChecked = true;
            else if (view is SearchView)
                btnSearch.IsChecked = true;
            else if (view is QueueView)
                btnQueue.IsChecked = true;
            else if (view is GalleryView)
            {
                //btnMovies.IsChecked = true;
            }
            else if (view is MainView || view is DetailsView || view is MediaView)
                btnHome.IsChecked = true;
        }

        #endregion

        #region Event Handlers

        private async void BodyFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the selected item when a page navigation occurs in the body frame
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.ViewModel.IsMenuOpen = false;
                this.UpdateSelectedMenuItem();
            });
        }

        /// <summary>
        /// Default keyboard focus movement for any unhandled keyboarding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShellView_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            FocusNavigationDirection direction = FocusNavigationDirection.None;
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.GamepadDPadLeft:
                case Windows.System.VirtualKey.GamepadLeftThumbstickLeft:
                case Windows.System.VirtualKey.NavigationLeft:
                    direction = FocusNavigationDirection.Left;
                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.GamepadDPadRight:
                case Windows.System.VirtualKey.GamepadLeftThumbstickRight:
                case Windows.System.VirtualKey.NavigationRight:
                    direction = FocusNavigationDirection.Right;
                    break;

                case Windows.System.VirtualKey.Up:
                case Windows.System.VirtualKey.GamepadDPadUp:
                case Windows.System.VirtualKey.GamepadLeftThumbstickUp:
                case Windows.System.VirtualKey.NavigationUp:
                    direction = FocusNavigationDirection.Up;
                    break;

                case Windows.System.VirtualKey.Down:
                case Windows.System.VirtualKey.GamepadDPadDown:
                case Windows.System.VirtualKey.GamepadLeftThumbstickDown:
                case Windows.System.VirtualKey.NavigationDown:
                    direction = FocusNavigationDirection.Down;
                    break;
            }

            if (direction != FocusNavigationDirection.None)
            {
                var control = FocusManager.FindNextFocusableElement(direction) as Control;
                if (control != null)
                {
                    control.Focus(FocusState.Programmatic);
                    e.Handled = true;
                }
            }
        }

        #endregion
    }
}
