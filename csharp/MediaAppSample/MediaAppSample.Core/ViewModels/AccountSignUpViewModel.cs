using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.Authentication.Web.Core;

namespace MediaAppSample.Core.ViewModels
{
    public partial class AccountSignUpViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return Strings.Account.ViewTitleSignUp; }
        }

        /// <summary>
        /// Command used to submit form.
        /// </summary>
        public CommandBase SubmitCommand { get; private set; }

        private bool _IsSubmitEnabled;
        /// <summary>
        /// Gets a boolean indicating whether or not the form is valid.
        /// </summary>
        public bool IsSubmitEnabled
        {
            get { return _IsSubmitEnabled; }
            set
            {
                if (this.SetProperty(ref _IsSubmitEnabled, value))
                    this.SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private CommandBase _LaunchWebAccountManagerCommand = null;
        /// <summary>
        /// Command to access Web Account Manager
        /// </summary>
        public CommandBase LaunchWebAccountManagerCommand
        {
            get { return _LaunchWebAccountManagerCommand ?? (_LaunchWebAccountManagerCommand = new GenericCommand("LaunchWebAccountManagerCommand", async () => await this.LaunchWebAccountManager())); }
        }

        private string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set { this.SetProperty(ref _FirstName, value); }
        }

        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set { this.SetProperty(ref _LastName, value); }
        }

        private string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                if (this.SetProperty(ref _Username, value))
                    this.CheckIfValid();
            }
        }

        private string _Password1;
        public string Password1
        {
            get { return _Password1; }
            set
            {
                if (this.SetProperty(ref _Password1, value))
                    this.CheckIfValid();
            }
        }

        private string _Password2;
        public string Password2
        {
            get { return _Password2; }
            set
            {
                if (this.SetProperty(ref _Password2, value))
                    this.CheckIfValid();
            }
        }

        private DateTime _DOB = DateTime.Today.AddYears(-18);
        public DateTime DOB
        {
            get { return _DOB; }
            set { this.SetProperty(ref _DOB, value); }
        }
        
        private string _Address1;
        public string Address1
        {
            get { return _Address1; }
            set { this.SetProperty(ref _Address1, value); }
        }

        private string _Address2;
        public string Address2
        {
            get { return _Address2; }
            set { this.SetProperty(ref _Address2, value); }
        }

        private string _City;
        public string City
        {
            get { return _City; }
            set { this.SetProperty(ref _City, value); }
        }

        private string _State;
        public string State
        {
            get { return _State; }
            set { this.SetProperty(ref _State, value); }
        }

        private string _PostalCode;
        public string PostalCode
        {
            get { return _PostalCode; }
            set { this.SetProperty(ref _PostalCode, value); }
        }

        #endregion

        #region Constructors

        public AccountSignUpViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.SubmitCommand = new GenericCommand<IModel>("AccountSignUpViewModel-SubmitCommand", async () => await this.SubmitAsync(), () => this.IsSubmitEnabled);

            // Preserves valids for the following fields when the app tombstones
            this.PreservePropertyState(() => this.Username);
            this.PreservePropertyState(() => this.FirstName);
            this.PreservePropertyState(() => this.LastName);
            this.PreservePropertyState(() => this.Address1);
            this.PreservePropertyState(() => this.Address2);
            this.PreservePropertyState(() => this.City);
            this.PreservePropertyState(() => this.State);
            this.PreservePropertyState(() => this.PostalCode);
            this.PreservePropertyState(() => this.Password1);
            this.PreservePropertyState(() => this.Password2);
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

        internal void Populate(MicrosoftAccountDetails msa)
        {
            if (msa == null)
                return;

            this.FirstName = msa.first_name;
            this.LastName = msa.last_name;
            this.Username = msa.emails.account;
            this.Address1 = msa.addresses?.personal?.street;
            this.Address2 = msa.addresses?.personal?.street_2?.ToString();
            this.City = msa.addresses?.personal?.city;
            this.State = msa.addresses?.personal?.state;
            this.PostalCode = msa.addresses?.personal?.postal_code;
            this.DOB = new DateTime(msa.birth_year, msa.birth_month, msa.birth_day);
        }

        private void CheckIfValid()
        {
            this.IsSubmitEnabled =
                !string.IsNullOrEmpty(this.FirstName)
                && !string.IsNullOrEmpty(this.LastName)
                && !string.IsNullOrWhiteSpace(this.Username)
                && !string.IsNullOrWhiteSpace(this.Password1)
                && this.Password1 == this.Password2;
        }

        private async Task SubmitAsync()
        {
            try
            {
                this.IsSubmitEnabled = false;
                this.ShowBusyStatus(Strings.Account.TextCreatingAccount, true);

                using (var api = new ClientApi())
                {
                    string userMessage = null;
                    var response = await api.RegisterAsync(this);

                    if (response?.AccessToken != null)
                        Platform.Current.AuthManager.SetUser(response);
                    else
                        userMessage = Strings.Account.TextAuthenticationFailed;

                    this.ClearStatus();

                    if (this.IsUserAuthenticated)
                        Platform.Current.Navigation.Home();
                    else
                        await this.ShowMessageBoxAsync(userMessage, this.Title);
                }
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

        private async Task LaunchWebAccountManager()
        {
            await Platform.Current.WebAccountManager.SignoutAsync();
            Platform.Current.WebAccountManager.Show(this.WAM_Success, this.WAM_Failed);
        }

        private CancellationTokenSource _cts;
        private async void WAM_Success(Services.WebAccountManager.WebAccountProviderInfo pi, Services.WebAccountManager.WebAccountInfo wad, WebTokenRequestResult result)
        {
            try
            {
                this.ShowBusyStatus(string.Format(Strings.Account.TextWebAccountManagerRetrievingProfile, pi.WebAccountType), true);

                await Platform.Current.WebAccountManager.SignoutAsync();

                _cts = new CancellationTokenSource();
                using (var api = new MicrosoftApi())
                {
                    var msa = await api.GetUserProfile(wad.Token, _cts.Token);
                    this.Populate(msa);
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to perform work during WAM success");
            }
            finally
            {
                this.Dispose();
                this.ClearStatus();
            }
        }

        private async void WAM_Failed(Services.WebAccountManager.WebAccountProviderInfo pi, WebTokenRequestResult result)
        {
            try
            {
                Platform.Current.Logger.LogError(result?.ResponseError.ToException(), "WAM failed to retrieve user account token.");
                await this.ShowMessageBoxAsync("Could not retrieve your Microsoft Account profile to pre-fill your account registration.");
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to perform work during WAM failure");
            }
        }

        #endregion
    }

    public partial class AccountSignUpViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public AccountSignUpViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class AccountSignUpViewModel : MediaAppSample.Core.ViewModels.AccountSignUpViewModel
    {
        public AccountSignUpViewModel()
        {
        }
    }
}