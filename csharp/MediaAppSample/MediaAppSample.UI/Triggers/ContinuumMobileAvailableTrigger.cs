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
using System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace MediaAppSample.UI.Triggers
{
    /// <summary>
    /// Trigger for determinging when continuum on Windows Mobile is available for consumption.
    /// </summary>
    public class ContinuumMobileAvailableTrigger : StateTriggerBase
    {
        public ContinuumMobileAvailableTrigger()
        {
            ProjectionManager.ProjectionDisplayAvailableChanged += ProjectionManager_ProjectionDisplayAvailableChanged;
            this.UpdateTrigger();
        }

        private async void ProjectionManager_ProjectionDisplayAvailableChanged(object sender, object e)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.UpdateTrigger();
            });
        }

        private void UpdateTrigger()
        {
            this.SetActive(ProjectionManager.ProjectionDisplayAvailable && Platform.Current.IsMobile);
        }
    }
}
