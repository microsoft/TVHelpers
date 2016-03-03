using MediaAppSample.Core;
using MediaAppSample.Core.Models;
using MediaAppSample.Core.Services;
using MediaAppSample.Core.ViewModels;
using MediaAppSample.UI.Views;
using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Services
{
    /// <summary>
    /// NavigationManager instance with implementations specific to this application.
    /// </summary>
    public sealed partial class NavigationManager : NavigationManagerBase
    {
        #region Handle Activation Methods

        /// <summary>
        /// Handles actions from primary and secondary tiles and jump lists.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnActivation(LaunchActivatedEventArgs e)
        {
            var handled = this.HandleArgumentsActivation(e.Arguments);

            if (handled == false && Platform.Current.InitializationMode == InitializationModes.Restore)
                handled = true;

            return handled;
        }

        /// <summary>
        /// Handles activations from toasts activations.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnActivation(ToastNotificationActivatedEventArgs e)
        {
            return this.HandleArgumentsActivation(e.Argument);
        }

        /// <summary>
        /// Handles protocol activation i.e. contoso:4
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnActivation(ProtocolActivatedEventArgs e)
        {
            return this.HandleArgumentsActivation(e.Uri.PathAndQuery);
        }

        /// <summary>
        /// Handles voice commands from Cortana integration
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnActivation(VoiceCommandActivatedEventArgs e)
        {
            var info = new VoiceCommandInfo(e.Result);
            switch (info.VoiceCommandName)
            {
                case "showByName":
                    // Access the value of the {destination} phrase in the voice command
                    string itemName = info.GetSemanticInterpretation("ItemName");
                    this.Details(itemName);
                    return true;

                default:
                    // If we can't determine what page to launch, go to the default entry point.
                    return false;
            }
        }

        private bool HandleArgumentsActivation(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
                return false;

            Platform.Current.Logger.Log(LogLevels.Information, "HandleArgumentsActivation: {0}", arguments);

            try
            {
                var dic = GeneralFunctions.ParseQuerystring(arguments);

                if (dic.ContainsKey("model"))
                {
                    switch (dic["model"].ToLower())
                    {
                        case "itemmodel":
                            this.Details(dic["ID"]);
                            return true;

                    }
                    throw new NotImplementedException(string.Format("No action implemented for model type ", dic["model"]));
                }
                else
                {
                    // When a proper querystring isn't passed in as arguments
                    int id = int.MinValue;
                    if (int.TryParse(arguments, out id))
                    {
                        this.Details(id);
                        return true;
                    }
                    else
                    {
                        Platform.Current.Logger.Log(LogLevels.Warning, "Unknown arguments passed into application '{0}'", arguments);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not parse argument '{0}' passed into app.", arguments);
                return false;
            }
        }

        #endregion Handle Activation

        #region Sharing Methods

        protected override void SetShareContent(DataRequest request, IModel model)
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

        #region Navigation Methods

        private void Home()
        {
            this.Home(null);
        }

        public override void Home(object parameter = null)
        {
            if(Platform.Current.AuthManager.IsAuthenticated() == false)
            {
                this.ParentFrame.Navigate(typeof(WelcomeView), this.SerializeParameter(parameter));
                this.ClearBackstack();
            }
            else
            {
                // User is authenticated
                if (this.ParentFrame.Content == null || !(this.ParentFrame.Content is ShellView))
                {
                    NavigationRequest navParam = parameter as NavigationRequest ?? new NavigationRequest(typeof(MainView), parameter);
                    this.ParentFrame.Navigate(typeof(ShellView), this.SerializeParameter(navParam));
                    this.ClearBackstack();
                }
                else
                {
                    this.Frame.Navigate(typeof(MainView), this.SerializeParameter(parameter));
                }
            }
        }

        public override void NavigateTo(IModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model is ItemModel || model is ItemViewModel)
                this.Details(model);
            else
                throw new NotImplementedException("Navigation not implemented for type " + model.GetType().Name);
        }

        protected override void NavigateToSecondaryWindow(NavigationRequest request)
        {
            this.ParentFrame.Navigate(typeof(SecondaryWindowView), this.SerializeParameter(request));
        }

        protected override void WebView(object parameter)
        {
            this.Frame.Navigate(typeof(WebBrowserView), this.SerializeParameter(parameter));
        }

        public override void Settings(object parameter = null)
        {
            this.Frame.Navigate(typeof(SettingsView), this.SerializeParameter(parameter));
        }

        public override void Search(object parameter)
        {
            bool removePrevious = false;

            // If the current page is a search page, remove that page before displaying a new search results page
            if (this.Frame.CurrentSourcePageType == typeof(SearchView))
                removePrevious = true;

            this.Frame.Navigate(typeof(SearchView), this.SerializeParameter(parameter));

            if (removePrevious)
                this.RemovePreviousPage();
        }

        public override void AccountSignin(object parameter = null)
        {
            this.ParentFrame.Navigate(typeof(AccountSignInView), this.SerializeParameter(parameter));
        }

        public override void AccountSignup(object parameter = null)
        {
            this.ParentFrame.Navigate(typeof(AccountSignUpView), this.SerializeParameter(parameter));
        }

        public override void AccountForgot(object parameter = null)
        {
            this.ParentFrame.Navigate(typeof(AccountForgotView), this.SerializeParameter(parameter));
        }

        public override void Details(object parameter)
        {
            if (this.IsChildFramePresent)
                this.Frame.Navigate(typeof(ItemView), this.SerializeParameter(parameter));
            else
                this.Home(new NavigationRequest(typeof(ItemView), parameter));
        }

        public override void Queue()
        {
            this.Frame.Navigate(typeof(QueueView));
        }

        public override void Gallery(object parameter)
        {
            this.Frame.Navigate(typeof(GalleryView), this.SerializeParameter(parameter));
        }

        public override void Media(object parameter)
        {
            if (this.IsChildFramePresent)
                this.Frame.Navigate(typeof(MediaView), this.SerializeParameter(parameter));
            else
                this.Home(new NavigationRequest(typeof(MediaView), parameter));
        }

        #endregion

        #region Frame Customizations

        protected override Frame CreateFrame()
        {
            return new ApplicationFrame();
        }

        #endregion
    }
}