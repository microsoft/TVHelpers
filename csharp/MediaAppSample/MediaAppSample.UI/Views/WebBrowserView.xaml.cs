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
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class WebBrowserViewBase : ViewBase<WebBrowserViewModel>
    {
    }

    public sealed partial class WebBrowserView : WebBrowserViewBase
    {
        public WebBrowserView()
        {
            InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
            {
                WebBrowserViewModel vm;
                if (e.NavigationEventArgs.Parameter is WebBrowserViewModel)
                    vm = e.NavigationEventArgs.Parameter as WebBrowserViewModel;
                else
                    vm = new WebBrowserViewModel();
                this.SetViewModel(vm);
            }

            return base.OnLoadStateAsync(e);
        }
    }
}
