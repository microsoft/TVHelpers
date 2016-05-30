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
using MediaAppSample.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class AccountSignInViewModel : ViewModelBase
    {
        #region Properties
        
        /// <summary>
        /// Command used to submit the sign in form.
        /// </summary>
        public CommandBase SubmitCommand { get; private set; }

        private string _Username;

        /// <summary>
        /// Username entered by user.
        /// </summary>
        public string Username
        {
            get { return _Username; }
            set
            {
                if (this.SetProperty(ref _Username, value))
                    this.CheckIfValid();
            }
        }

        private string _Password;
        /// <summary>
        /// Password entered by user.
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set
            {
                if (this.SetProperty(ref _Password, value))
                    this.CheckIfValid();
            }
        }

        private bool _IsSubmitEnabled = false;
        /// <summary>
        /// Gets a boolean indicating whether or not the form has valid data to enable the submit button.
        /// </summary>
        public bool IsSubmitEnabled
        {
            get { return _IsSubmitEnabled; }
            private set
            {
                if (this.SetProperty(ref _IsSubmitEnabled, value))
                    this.SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Constructors

        public AccountSignInViewModel()
        {
            this.Title = Strings.Account.ViewTitleSignIn;

            if (DesignMode.DesignModeEnabled)
                return;

            this.SubmitCommand = new GenericCommand<IModel>("AccountSignInViewModel-SubmitCommand", async () => await this.SubmitAsync(), () => this.IsSubmitEnabled);

            // Properties to preserve during tombstoning
            this.PreservePropertyState(() => this.Username);
            this.PreservePropertyState(() => this.Password);
        }

        #endregion

        #region Methods

        protected override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
            }

            this.CheckIfValid();

            return base.OnLoadStateAsync(e, isFirstRun);
        }

        /// <summary>
        /// Submit the form to the authentication service.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        private async Task SubmitAsync()
        {
            try
            {
                this.IsSubmitEnabled = false;
                this.ShowBusyStatus(Strings.Account.TextAuthenticating, true);

                string userMessage = null;
                var response = await DataSource.Current.AuthenticateAsync(this, CancellationToken.None);

                // Ensure that there is a valid token returned
                if (response?.AccessToken != null)
                    Platform.Current.AuthManager.SetUser(response);
                else
                    userMessage = Strings.Account.TextAuthenticationFailed;

                this.ClearStatus();

                // Nav home if authenticated else display error message
                if (this.IsUserAuthenticated)
                    Platform.Current.Navigation.Home();
                else
                    await this.ShowMessageBoxAsync(userMessage, this.Title);
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex);
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
            }
            finally
            {
                this.CheckIfValid();
            }
        }

        /// <summary>
        /// Checks to see if the form is valid.
        /// </summary>
        private void CheckIfValid()
        {
            this.IsSubmitEnabled = !string.IsNullOrWhiteSpace(this.Username)
                && !string.IsNullOrWhiteSpace(this.Password)
                && this.Username.Length > 0
                && this.Password.Length > 0;
        }

        #endregion
    }

    public partial class AccountSignInViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public AccountSignInViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class AccountSignInViewModel : MediaAppSample.Core.ViewModels.AccountSignInViewModel
    {
        public AccountSignInViewModel()
        {
        }
    }
}