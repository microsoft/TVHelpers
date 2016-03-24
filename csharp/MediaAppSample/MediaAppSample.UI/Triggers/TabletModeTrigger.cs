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

using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace MediaAppSample.UI.Triggers
{
    /// <summary>
    /// Trigger to determine when a user enters tablet mode.
    /// </summary>
    public class TabletModeTrigger : StateTriggerBase
    {
        public TabletModeTrigger()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        ~TabletModeTrigger()
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            bool isTabletMode = UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch;
            this.SetActive(isTabletMode);
        }
    }
}
