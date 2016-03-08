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
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public sealed partial class MetadataControl : UserControl
    {
        public MainViewModel MainVM => Platform.Current.ViewModel;

        public DetailsViewModel VM => this.DataContext as DetailsViewModel;

        /// <summary>
        /// Note that we use a UserControl for simplicity. Optimally this should be written as a Control with a ControlTemplate
        /// to take advantage of performance improvements.
        /// The only reason that we are using a UserControl is to workaround the fact that the Hub control uses DataTemplates to contain content
        /// and DataTemplates do not support VisualStateManager events.
        /// </summary>
        public MetadataControl()
        {
            this.InitializeComponent();
        }
    }
}
