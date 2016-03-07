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
            if (model is ItemModel)
            {
                var m = model as ItemModel;
                requestData.Properties.Title = m.LineOne;
                requestData.Properties.Description = m.LineTwo;
                var args = Platform.Current.GenerateModelArguments(model);
                requestData.Properties.ContentSourceApplicationLink = new Uri(Platform.Current.AppInfo.GetDeepLink(args), UriKind.Absolute);
                string body = m.LineOne + Environment.NewLine + m.LineTwo + Environment.NewLine + m.LineThree + Environment.NewLine + m.LineFour;
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
