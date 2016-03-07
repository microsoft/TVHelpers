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
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return Strings.Account.ViewTitleSignIn; }
        }

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