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

using MediaAppSample.Core;
using MediaAppSample.Core.Models;
using MediaAppSample.Core.Services;
using MediaAppSample.UI.Views;
using System;
using Windows.ApplicationModel.Activation;
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
        /// Handles protocol activation i.e. MediaAppSample:4
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
                case "showTitleDetails":
                    {
                        // Access the value of the {destination} phrase in the voice command
                        string spokenTitle = info.GetSemanticInterpretation("Titles");
                        this.Details(spokenTitle);
                        return true;
                    }

                case "playTitle":
                    {
                        // Access the value of the {destination} phrase in the voice command
                        string spokenTitle = info.GetSemanticInterpretation("Titles");
                        this.Media(spokenTitle);
                        return true;
                    }

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
                        case "tvseriesmodel":
                        case "tvepisodemodel":
                        case "trailermodel":
                        case "moviemodel":
                        case "contentitembase":
                            this.Details(dic["ID"]);
                            return true;
                    }
                    throw new NotImplementedException(string.Format("No action implemented for model type ", dic["model"]));
                }
                else
                {
                    this.Details(arguments);
                    return true;

                    //// When a proper querystring isn't passed in as arguments
                    //int id = int.MinValue;
                    //if (int.TryParse(arguments, out id))
                    //{
                    //    //this.Details(id);
                    //    return true;
                    //}
                    //else
                    //{
                    //    Platform.Current.Logger.Log(LogLevels.Warning, "Unknown arguments passed into application '{0}'", arguments);
                    //    return false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not parse argument '{0}' passed into app.", arguments);
                return false;
            }
        }

        #endregion Handle Activation

        #region Navigation Methods

        private void Home()
        {
            this.Home(null);
        }

        public override void Home(object parameter = null)
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

        public override void NavigateTo(object model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model is QueueModel)
                this.Details((model as QueueModel).Item);
            else if (model is ContentItemBase)
                this.Details(model);
            else
                throw new NotImplementedException("Navigation not implemented for type " + model.GetType().Name);
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

        public override void Welcome(object parameter = null)
        {
            this.ParentFrame.Navigate(typeof(WelcomeView), this.SerializeParameter(parameter));
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
            if (parameter is QueueModel)
                parameter = (parameter as QueueModel).Item;
            if (parameter is ContentItemBase)
                parameter = (parameter as ContentItemBase).ID;

            if (this.IsChildFramePresent)
                this.Frame.Navigate(typeof(DetailsView), this.SerializeParameter(parameter));
            else
                this.Home(new NavigationRequest(typeof(DetailsView), parameter));
        }

        public override void Queue()
        {
            this.Frame.Navigate(typeof(QueueView));
        }

        public override void Movies()
        {
            this.Frame.Navigate(typeof(GalleryView), ItemTypes.Movie.ToString());
        }

        public override void TV()
        {
            this.Frame.Navigate(typeof(GalleryView), ItemTypes.TvSeries.ToString());
        }

        public override void Media(object parameter)
        {
            if (parameter is QueueModel)
                parameter = (parameter as QueueModel).Item;
            if (parameter is ContentItemBase)
                parameter = (parameter as ContentItemBase).ID;

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