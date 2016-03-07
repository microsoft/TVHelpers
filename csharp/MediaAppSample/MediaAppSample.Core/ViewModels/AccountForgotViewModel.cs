using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.Core.ViewModels
{
    public partial class AccountForgotViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return Strings.Account.ViewTitleForgotPassword; }
        }

        /// <summary>
        /// Command used to submit the form.
        /// </summary>
        public CommandBase SubmitCommand { get; private set; }

        private bool _IsSubmitEnabled;
        /// <summary>
        /// Gets whether or not the form is in a valid state and can be submitted.
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

        private string _Username;
        /// <summary>
        /// Gets or sets the username to check to see if the account exists and send credentials to.
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

        #endregion

        #region Constructors

        public AccountForgotViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.SubmitCommand = new GenericCommand<IModel>("AccountForgotViewModel-SubmitCommand", async () => await this.SubmitAsync(), () => this.IsSubmitEnabled);

            // Properties to preserve during tombstoning
            this.PreservePropertyState(() => this.Username);
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
        /// Checks to see if the form data is valid.
        /// </summary>
        private void CheckIfValid()
        {
            this.IsSubmitEnabled = !string.IsNullOrWhiteSpace(this.Username);
        }

        /// <summary>
        /// Submits the form to the account forgotten service.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        private async Task SubmitAsync()
        {
            try
            {
                this.IsSubmitEnabled = false;
                this.ShowBusyStatus(Strings.Account.TextValidatingUsername, true);

                using (var api = new ClientApi())
                {
                    var response = await api.ForgotPasswordAsync(this);

                    this.ClearStatus();

                    await this.ShowMessageBoxAsync(response.Message, this.Title);
                    if (response?.IsValid == true)
                        Platform.Current.Navigation.GoBack();
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

        #endregion
    }

    public partial class AccountForgotViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public AccountForgotViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class AccountForgotViewModel : MediaAppSample.Core.ViewModels.AccountForgotViewModel
    {
        public AccountForgotViewModel()
        {
            this.Username = "TestUsername1";
        }
    }
}