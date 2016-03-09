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

namespace MediaAppSample.UI.Controls
{
    public abstract class HeroControlBase : ViewControlBase<MainViewModel>
    {
    }

    /// <summary>
    /// Note that we use a UserControl for simplicity. Optimally this should be written as a Control with a ControlTemplate
    /// to take advantage of performance improvements.
    /// The only reason that we are using a UserControl is to workaround the fact that the Hub control uses DataTemplates to contain content
    /// and DataTemplates do not support VisualStateManager events.
    /// </summary>
    public sealed partial class HeroControl : HeroControlBase
    {
        public HeroControl()
        {
            this.InitializeComponent();
        }
    }
}
