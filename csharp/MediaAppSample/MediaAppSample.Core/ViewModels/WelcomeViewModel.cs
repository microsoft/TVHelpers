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
using MediaAppSample.Core.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Authentication.Web.Core;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.Core.ViewModels
{
    public partial class WelcomeViewModel : ViewModelBase
    {
        #region Variables

        private CancellationTokenSource _cts;

        #endregion

        #region Properties

        private CommandBase _LaunchWebAccountManagerCommand = null;
        /// <summary>
        /// Command to access Web Account Manager
        /// </summary>
        public CommandBase LaunchWebAccountManagerCommand
        {
            get { return _LaunchWebAccountManagerCommand ?? (_LaunchWebAccountManagerCommand = new GenericCommand("LaunchWebAccountManagerCommand", async () => await this.LaunchWebAccountManager())); }
        }

        #endregion Properties

        #region Constructors

        public WelcomeViewModel()
        {
            this.Title = Strings.Resources.ViewTitleWelcome;

            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion Constructors

        #region Methods

        protected override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New)
            {
            }

            return base.OnLoadStateAsync(e, isFirstRun);
        }
        
        /// <summary>
        /// Launches the WebAccountManager (WAM)
        /// </summary>
        /// <returns></returns>
        private async Task LaunchWebAccountManager()
        {
            await Platform.Current.WebAccountManager.SignoutAsync();
            Platform.Current.WebAccountManager.Show(this.WAM_Success, this.WAM_Failed);
        }

        /// <summary>
        /// Flow to perform on successful pick of an account from the WAM popup
        /// </summary>
        /// <param name="pi">Details of the WAM provider choosen</param>
        /// <param name="info">Details of the WAM authenticated account</param>
        /// <param name="result">WebTokenRequestResult instance containing token info.</param>
        private async void WAM_Success(Services.WebAccountManager.WebAccountProviderInfo pi, Services.WebAccountManager.WebAccountInfo info, WebTokenRequestResult result)
        {
            try
            {
                this.ShowBusyStatus(Strings.Account.TextAuthenticating, true);

                // Create an account with the API
                _cts = new CancellationTokenSource();

                var response = await DataSource.Current.AuthenticateAsync(info, _cts.Token);

                // Authenticate the user into the app
                Platform.Current.AuthManager.SetUser(response);

                Platform.Current.Navigation.Home(this.ViewParameter);
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to perform work during WAM success");
            }
            finally
            {
                this.Dispose();
                this.ClearStatus();
            }
        }

        /// <summary>
        /// Flow to perform on any failures from the WAM popup
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="result"></param>
        private async void WAM_Failed(Services.WebAccountManager.WebAccountProviderInfo pi, WebTokenRequestResult result)
        {
            try
            {
                // Failure with WAM
                Platform.Current.Logger.LogError(result?.ResponseError.ToException(), "WAM failed to retrieve user account token.");
                await this.ShowMessageBoxAsync(string.Format(Strings.Account.TextWebAccountManagerRegisterAccountFailure, pi.WebAccountType));
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to perform work during WAM failure");
            }
        }

        public override void Dispose()
        {
            // Terminate any open tasks
            if (_cts?.IsCancellationRequested == false)
                _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            base.Dispose();
        }

        #endregion Methods
    }

    public partial class WelcomeViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public WelcomeViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class WelcomeViewModel : MediaAppSample.Core.ViewModels.WelcomeViewModel
    {
        public WelcomeViewModel()
        {
        }
    }
}