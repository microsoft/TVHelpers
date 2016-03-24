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
using System.Threading.Tasks;

namespace MediaAppSample.UI.Views
{
    public abstract class QueueViewBase : ViewBase<QueueViewModel>
    {
    }

    public sealed partial class QueueView : QueueViewBase
    {
        public QueueView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (this.ViewModel == null)
                this.SetViewModel(Platform.Current.ViewModel.QueueViewModel);

            await base.OnLoadStateAsync(e);
        }
    }
}
