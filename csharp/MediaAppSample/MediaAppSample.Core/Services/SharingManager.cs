// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the app info service of the platform currently executing.
        /// </summary>
        public SharingManager SharingManager
        {
            get { return this.GetService<SharingManager>(); }
            protected set { this.SetService<SharingManager>(value); }
        }
    }

    public sealed class SharingManager : ServiceBase
    {
        #region Properties

        private CommandBase _shareCommand = null;
        /// <summary>
        /// Command to navigate to the share functionality.
        /// </summary>
        public CommandBase ShareCommand
        {
            get { return _shareCommand ?? (_shareCommand = new ShareCommand()); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Share a model of data with Windows.
        /// </summary>
        /// <param name="model"></param>
        public void Share(IModel model)
        {
            Platform.Current.Analytics.Event("Share");

            DataTransferManager.GetForCurrentView().DataRequested += (sender, e) =>
            {
                try
                {
                    this.SetShareContent(e.Request, model ?? Platform.Current.ViewModel);
                }
                catch (Exception ex)
                {
                    Platform.Current.Logger.LogError(ex, "Error in OnDataRequested");
#if DEBUG
                    e.Request.FailWithDisplayText(ex.ToString());
#else
                    e.Request.FailWithDisplayText(Strings.Resources.TextErrorGeneric);
#endif
                }
            };
            DataTransferManager.ShowShareUI();
        }

        private void SetShareContent(DataRequest request, IModel model)
        {
            DataPackage requestData = request.Data;

            Platform.Current.Logger.Log(LogLevels.Information, "SetShareContent - Model: {0}", model?.GetType().Name);

            // Sharing is based on the model data that was passed in. Perform customized sharing based on the type of the model provided.
            if (model is ContentItemBase)
            {
                var m = model as ContentItemBase;
                requestData.Properties.Title = m.Title;
                requestData.Properties.Description = m.Description;
                var args = Platform.Current.GenerateModelArguments(model);
                requestData.Properties.ContentSourceApplicationLink = new Uri(Platform.Current.AppInfo.GetDeepLink(args), UriKind.Absolute);
                string body = m.Title + Environment.NewLine + m.Description;
                requestData.SetText(body);
            }
            else
            {
                requestData.Properties.Title = Core.Strings.Resources.ApplicationName;
                requestData.Properties.Description = Core.Strings.Resources.ApplicationDescription;
                requestData.Properties.ContentSourceApplicationLink = new Uri(Platform.Current.AppInfo.StoreURL, UriKind.Absolute);
                string body = string.Format(Core.Strings.Resources.ApplicationSharingBodyText, Core.Strings.Resources.ApplicationName, Platform.Current.AppInfo.StoreURL);
                requestData.SetText(body);
            }
        }

        #endregion Sharing
    }
}
