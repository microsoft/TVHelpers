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
using Windows.UI.Xaml;

namespace MediaAppSample.UI.Triggers
{
    /// <summary>
    /// Trigger to indicate when a window is displayed on a Windows Mobile continuum screen.
    /// </summary>
    public class ContinuumMobileExecutingTrigger : StateTriggerBase
    {
        public ContinuumMobileExecutingTrigger()
        {
            Window.Current.SizeChanged += (sender, args) => this.UpdateTrigger();
            this.UpdateTrigger();
        }

        private void UpdateTrigger()
        {
            if (Platform.Current.IsMobileContinuumDesktop)
                this.SetActive(true);
            else
                this.SetActive(false);
        }
    }
}
