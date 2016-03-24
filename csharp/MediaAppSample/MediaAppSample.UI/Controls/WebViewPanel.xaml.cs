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
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public abstract class WebViewPanelBase : ViewControlBase<WebBrowserViewModel>
    {
    }

    public sealed partial class WebViewPanel : WebViewPanelBase
    {
        public WebViewPanel()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) => this.SetCurrentViewModel(this.DataContext as WebBrowserViewModel);
        }

        #region Methods

        /// <summary>
        /// Configures a WebBrowserViewModel instance to get notified of WebView control events.
        /// </summary>
        /// <param name="vm"></param>
        private void SetCurrentViewModel(WebBrowserViewModel vm)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.GoHomeRequested -= ViewModel_GoHomeRequested;
                this.ViewModel.RefreshRequested -= ViewModel_RefreshRequested;
                this.ViewModel.GoForwardRequested -= ViewModel_GoForwardRequested;
                this.ViewModel.GoBackwardsRequested -= ViewModel_GoBackwardsRequested;
                this.ViewModel.NavigateToRequested -= ViewModel_NavigateToRequested;
                this.ViewModel.BrowserInstance = null;

                webView.NavigationStarting -= webView_Navigating;
                webView.NavigationCompleted -= webView_Navigated;

                while (webView.CanGoBack)
                    webView.GoBack();
            }

            this.ViewModel = vm;

            if (this.ViewModel != null)
            {
                webView.NavigationStarting += webView_Navigating;
                webView.NavigationCompleted += webView_Navigated;

                this.ViewModel.BrowserInstance = webView;
                this.ViewModel.SetBrowserFunctions(() => webView.CanGoBack, () => webView.CanGoForward);
                this.ViewModel.GoHomeRequested += ViewModel_GoHomeRequested;
                this.ViewModel.RefreshRequested += ViewModel_RefreshRequested;
                this.ViewModel.GoForwardRequested += ViewModel_GoForwardRequested;
                this.ViewModel.GoBackwardsRequested += ViewModel_GoBackwardsRequested;
                this.ViewModel.NavigateToRequested += ViewModel_NavigateToRequested;

                this.ViewModel.InitialNavigation();
            }
        }

        #endregion

        #region Events

        private void webView_Navigating(WebView webView, WebViewNavigationStartingEventArgs e)
        {
            if (this.ViewModel != null)
                e.Cancel = this.ViewModel.Navigating(e.Uri);
        }

        private void webView_Navigated(WebView webView, WebViewNavigationCompletedEventArgs e)
        {
            if (this.ViewModel != null)
            {
                if (e.IsSuccess)
                    this.ViewModel.Navigated(e.Uri, webView.DocumentTitle);
                else
                    this.ViewModel.NavigationFailed(e.Uri, new Exception(e.WebErrorStatus.ToString()), webView.DocumentTitle);
            }
        }

        private void ViewModel_NavigateToRequested(object sender, string e)
        {
            var webView = sender as WebView;
            if (webView != null)
                webView.Navigate(new Uri(e, UriKind.Absolute));
        }

        private void ViewModel_RefreshRequested(object sender, EventArgs e)
        {
            var webView = sender as WebView;
            if (webView != null)
                webView.Refresh();
        }

        private void ViewModel_GoHomeRequested(object sender, EventArgs e)
        {
            var webView = sender as WebView;
            if (webView != null)
                while (webView.CanGoBack)
                    webView.GoBack();
        }

        private void ViewModel_GoBackwardsRequested(object sender, EventArgs e)
        {
            var webView = sender as WebView;
            if (webView != null && webView.CanGoBack)
                webView.GoBack();
        }

        private void ViewModel_GoForwardRequested(object sender, EventArgs e)
        {
            var webView = sender as WebView;
            if (webView != null && webView.CanGoForward)
                webView.GoForward();
        }

        #endregion
    }
}
