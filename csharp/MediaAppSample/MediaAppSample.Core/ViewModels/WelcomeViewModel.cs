using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
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

        public override string Title
        {
            get { return Strings.Resources.ViewTitleWelcome; }
        }

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
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion Constructors

        #region Methods

        public override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New)
            {
            }

            return base.OnLoadStateAsync(e, isFirstRun);
        }

        private async Task LaunchWebAccountManager()
        {
            await Platform.Current.WebAccountManager.SignoutAsync();
            Platform.Current.WebAccountManager.Show(this.WAM_Success, this.WAM_Failed);
        }

        private async void WAM_Success(Services.WebAccountManager.WebAccountProviderInfo pi, Services.WebAccountManager.WebAccountInfo wad, WebTokenRequestResult result)
        {
            try
            {
                this.ShowBusyStatus(Strings.Account.TextAuthenticating, true);

                _cts = new CancellationTokenSource();
                var response = await DataSource.Current.AuthenticateAsync(wad, _cts.Token);
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

        private async void WAM_Failed(Services.WebAccountManager.WebAccountProviderInfo pi, WebTokenRequestResult result)
        {
            try
            {
                Platform.Current.Logger.LogError(result?.ResponseError.ToException(), "WAM failed to retrieve user account token.");
                await this.ShowMessageBoxAsync("Could not use your Microsoft Account profile to register an account.");
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to perform work during WAM failure");
            }
        }

        public override void Dispose()
        {
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