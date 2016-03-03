using MediaAppSample.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ApplicationSettings;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the app info service of the platform currently executing.
        /// </summary>
        public WebAccountManager WebAccountManager
        {
            get { return this.GetAdapter<WebAccountManager>(); }
            set { this.Register<WebAccountManager>(value); }
        }
    }

    public sealed class WebAccountManager : ServiceBase, IServiceSignout
    {
        #region Enums

        public enum WebAccountTypes
        {
            Microsoft,
            AzureActiveDirectory,
            Facebook
        }

        #endregion

        #region Properties & Variables

        private static List<WebAccountProviderInfo> _providers = new List<WebAccountProviderInfo>();
        
        private bool ShowingUI { get; set; }

        private WebAccountManangerSuccessHandler _successHandler;
        private WebAccountManangerFailedHandler _failedHandler;

        public delegate void WebAccountManangerSuccessHandler(WebAccountProviderInfo pi, WebAccountInfo wad, WebTokenRequestResult result);
        public delegate void WebAccountManangerFailedHandler(WebAccountProviderInfo pi, WebTokenRequestResult result);

        #endregion

        #region Configuration

        static WebAccountManager()
        {
            // Create entries for all supported web account providers here

            // Microsoft Account Provider
            _providers.Add(new  WebAccountProviderInfo()
            {
                WebAccountType = WebAccountTypes.Microsoft,
                ProviderID = "https://login.microsoft.com",
                ClientID = "none",
                Scope = "wl.basic, wl.phone_numbers, wl.emails, wl.postal_addresses, wl.birthday",
                Authority = "consumers",
                Actions = SupportedWebAccountActions.Manage | SupportedWebAccountActions.Remove
            });

            //// Azure Active Directory (AAD) Account Provider
            //var providerAAD = new WebAccountProviderInfo()
            //{
            //    WebAccountType = WebAccountTypes.AzureActiveDirectory,
            //    ProviderID = "https://login.microsoft.com",
            //    ClientID = "{Your AAD ID here}",
            //    Scope = "service::wl.basic::DELEGATION",
            //    Authority = "organizations"
            //};
            //providerAAD.RequestProperties.Add("resource", "https://graph.windows.net");
            //_providers.Add(providerAAD);

            //// Facebook Account Provider
            //var providerFB = new WebAccountProviderInfo()
            //{
            //    WebAccountType = WebAccountTypes.Facebook,
            //    ProviderID = "https://www.facebook.com",
            //    ClientID = "{Your FB client ID from their dev portal here}",
            //    Scope = "public_profile",
            //    Authority = null,
            //    Actions = SupportedWebAccountActions.Manage | SupportedWebAccountActions.Remove
            //};
            //providerFB.RequestProperties.Add("redirect_uri", "msft-dc9fb650-0192-404e-82d8-f204246816ee://success");
            //_providers.Add(providerFB);
        }

        #endregion

        #region Constructors

        internal WebAccountManager()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays the Web Account Manager for users to chose an account to authenticate with.
        /// </summary>
        /// <param name="successHandler">Call back for when an account is successfully choosen by the user.</param>
        /// <param name="failedHandler">Call back for when an error or cancelled UI occurs by the user.</param>
        public void Show(WebAccountManangerSuccessHandler successHandler, WebAccountManangerFailedHandler failedHandler)
        {
            if (this.ShowingUI == false)
                AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += OnAccountCommandsRequested;
            this.ShowingUI = true;
            _successHandler = successHandler;
            _failedHandler = failedHandler;
            AccountsSettingsPane.Show();
        }

        /// <summary>
        /// Signs the current user out of all web accounts.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public async Task SignoutAsync()
        {
            foreach (var pi in _providers)
                await this.SignoutAsync(pi);
        }

        #endregion

        #region Private Methods

        #region Handlers

        private async void OnAccountCommandsRequested(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs e)
        {
            // In order to make async calls within this callback, the deferral object is needed
            AccountsSettingsPaneEventDeferral deferral = e.GetDeferral();

            try
            {
                foreach (var pi in _providers)
                {
                    // This scenario only lets the user have one account at a time.
                    // If there already is an account, we do not include a provider in the list
                    // This will prevent the add account button from showing up.
                    if (this.HasWebAccountInfo(pi.WebAccountType))
                    {
                        WebAccount account = await this.GetWebAccount(pi.WebAccountType);

                        if (account != null)
                        {
                            WebAccountCommand command = new WebAccountCommand(account, OnWebAccountRequested, pi.Actions);
                            e.WebAccountCommands.Add(command);
                        }
                    }
                    else
                    {
                        var provider = await this.GetProvider(pi.ProviderID, pi.Authority);
                        if (provider != null)
                        {
                            WebAccountProviderCommand providerCommand = new WebAccountProviderCommand(provider, OnWebAccountProviderRequested);
                            e.WebAccountProviderCommands.Add(providerCommand);
                        }
                    }
                }

                e.HeaderText = "Sign-In / Sign-Up with an account that you can associate with MediaAppSample.";

                // You can add links such as privacy policy, help, general account settings
                e.Commands.Add(new SettingsCommand("privacypolicy", Strings.Resources.ViewTitlePrivacyPolicy, (c) => { CommandsManager.Instance.NavigateToPrivacyPolicyCommand.Execute(null); this.Cleanup(); }));
                e.Commands.Add(new SettingsCommand("tos", Strings.Resources.ViewTitleTermsOfService, (c) => { CommandsManager.Instance.NavigateToTermsOfServiceCommand.Execute(null); this.Cleanup(); }));
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to display the web account manager UI.");
                throw ex;
            }
            finally
            {
                deferral.Complete();
            }
        }

        private async void OnWebAccountProviderRequested(WebAccountProviderCommand cmd)
        {
            // Retrieve the provider info instance for the requestd provider
            var pi = Platform.Current.WebAccountManager.GetProviderInfo(cmd.WebAccountProvider.Id);
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            try
            {
                WebTokenRequest request = new WebTokenRequest(cmd.WebAccountProvider, pi.Scope, pi.ClientID);

                // Add any properties for this request from the provider info
                if (pi.RequestProperties != null)
                    foreach (var prop in pi.RequestProperties)
                        request.Properties.Add(prop);

                // If the user selected a specific account, RequestTokenAsync will return a token for that account.
                // The user may be prompted for credentials or to authorize using that account with your app
                // If the user selected a provider, the user will be prompted for credentials to login to a new account
                WebTokenRequestResult result = await WebAuthenticationCoreManager.RequestTokenAsync(request);

                if (result.ResponseStatus == WebTokenRequestStatus.Success)
                {
                    var webTokenResponse = result.ResponseData[0];
                    var webAccount = webTokenResponse.WebAccount;

                    // Wrapper the web account data
                    WebAccountInfo wi = new WebAccountInfo();
                    wi.Type = pi.WebAccountType;
                    wi.ProviderID = webAccount.WebAccountProvider.Id;
                    wi.AccountID = !string.IsNullOrEmpty(webAccount.Id) ? webAccount.Id : webAccount.UserName;
                    wi.Authority = webAccount.WebAccountProvider.Authority != null ? webAccount.WebAccountProvider.Authority : "";
                    wi.Token = webTokenResponse.Token;
                    this.SaveWebAccountInfo(wi);

                    Platform.Current.Logger.Log(LogLevels.Information, string.Format("Web Token request successful for AccountID: {0}", wi.AccountID));

                    // Success Callback
                    _successHandler(pi, wi, result);
                }
                else
                {
                    Platform.Current.Logger.Log(LogLevels.Information, "Web Token request error: " + result.ResponseStatus + " Code: " + result.ResponseError.ErrorMessage);

                    // Failed Callback
                    _failedHandler(pi, result);
                }

            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Web Token request failed");
                _failedHandler(pi, null);
            }
            finally
            {
                this.Cleanup();
            }
        }

        private async void OnWebAccountRequested(WebAccountCommand cmd, WebAccountInvokedArgs args)
        {
            try
            {
                // Handle the different WebAccount actions...

                if (args.Action == WebAccountAction.Remove)
                {
                    // Signs the provider out.
                    Platform.Current.Logger.Log(LogLevels.Information, "Web Account Manager - Remove account called");
                    await this.SignoutAsync(cmd.WebAccount.WebAccountProvider.Id);
                }
                else if (args.Action == WebAccountAction.Manage)
                {
                    // Display user management UI for this account
                    Platform.Current.Logger.Log(LogLevels.Information, "Web Account Manager - Manage account called");
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Web Token request failed");
            }
            finally
            {
                this.Cleanup();
            }
        }

        /// <summary>
        /// Clean up after displaying the web account manager.
        /// </summary>
        private void Cleanup()
        {
            if (this.ShowingUI)
                AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= OnAccountCommandsRequested;
            this.ShowingUI = false;
            _successHandler = null;
            _failedHandler = null;
        }

        #endregion

        #region Retrieve

        /// <summary>
        /// Gets a web account by type.
        /// </summary>
        /// <param name="type">Type enum of the web account.</param>
        /// <returns>WebAccount instance if found else null.</returns>
        private async Task<WebAccount> GetWebAccount(WebAccountTypes type)
        {
            var details = this.GetWebAccountInfo(type);

            WebAccount account = null;
            if (details != null)
            {
                WebAccountProvider provider = await GetProvider(details.ProviderID, details.Authority);
                if (provider != null)
                    account = await WebAuthenticationCoreManager.FindAccountAsync(provider, details.AccountID);

                // The account has been deleted if FindAccountAsync returns null
                if (account == null)
                    this.DeleteUsersWebAccountDetails(type);
            }
            return account;
        }

        /// <summary>
        /// Gets a provider by ID.
        /// </summary>
        /// <param name="providerID">ID of the provider.</param>
        /// <param name="authority">Authority string of the provider.</param>
        /// <returns>WebAccountProvider instance if found else null.</returns>
        private async Task<WebAccountProvider> GetProvider(string providerID, string authority)
        {
            WebAccountProvider provider = null;
            try
            {
                if (string.IsNullOrWhiteSpace(authority))
                    provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(providerID);
                else
                    provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(providerID, authority);
            }
            catch
            {
            }
            return provider;
        }

        /// <summary>
        /// Gets a provider info by ID.
        /// </summary>
        /// <param name="providerID">ID of the provider.</param>
        /// <returns>Provider information else null.</returns>
        private WebAccountProviderInfo GetProviderInfo(string providerID)
        {
            return _providers.FirstOrDefault(p => p.ProviderID.Equals(providerID, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion

        #region Load/Save User WebAccountDetails

        /// <summary>
        /// Retrieves an authenticated account details from storage.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>WebAccountInfo instance if found else null.</returns>
        private WebAccountInfo GetWebAccountInfo(WebAccountTypes type)
        {
            return Platform.Current.Storage.LoadSetting<WebAccountInfo>("WAM_" + type.ToString(), ApplicationData.Current.RoamingSettings, SerializerTypes.Json);
        }

        /// <summary>
        /// Indicates whether or not an account is stored in storage.
        /// </summary>
        /// <param name="type">Enum type of the web account to check if stored info exists or not.</param>
        /// <returns>True if stored info exists else false.</returns>
        private bool HasWebAccountInfo(WebAccountTypes type)
        {
            return Platform.Current.Storage.ContainsSetting("WAM_" + type.ToString(), ApplicationData.Current.RoamingSettings);
        }

        /// <summary>
        /// Saves an authenticated account to storage.
        /// </summary>
        /// <param name="wi">Object containing the web account info.</param>
        private void SaveWebAccountInfo(WebAccountInfo wi)
        {
            if(wi != null)
                Platform.Current.Storage.SaveSetting("WAM_" + wi.Type.ToString(), wi, ApplicationData.Current.RoamingSettings, SerializerTypes.Json);
        }

        /// <summary>
        /// Deletes an authenticated account from storage.
        /// </summary>
        /// <param name="type">Enum type representing the web account to delete.</param>
        private void DeleteUsersWebAccountDetails(WebAccountTypes type)
        {
            Platform.Current.Storage.SaveSetting("WAM_" + type.ToString(), null, ApplicationData.Current.RoamingSettings, SerializerTypes.Json);
        }

        #endregion

        #region Remove Accounts

        /// <summary>
        /// Signs a user out of the specified provider.
        /// </summary>
        /// <param name="providerID">ID of the provider.</param>
        /// <returns>Awaitable task is returned.</returns>
        private async Task SignoutAsync(string providerID)
        {
            if (string.IsNullOrEmpty(providerID))
                throw new ArgumentNullException(nameof(providerID));

            var pi = this.GetProviderInfo(providerID);
            if (pi != null)
                await this.SignoutAsync(pi);
        }

        /// <summary>
        /// Signs a user out of the specified provider.
        /// </summary>
        /// <param name="pi">Provider information to sign out from.</param>
        /// <returns>Awaitable task is returned.</returns>
        private async Task SignoutAsync(WebAccountProviderInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            WebAccount account = await GetWebAccount(pi.WebAccountType);
            // Check if the account has been deleted already by Token Broker
            if (account != null)
                await account.SignOutAsync();
            account = null;
            this.DeleteUsersWebAccountDetails(pi.WebAccountType);
        }

        #endregion

        #endregion

        #region Classes

        public class WebAccountInfo
        {
            public WebAccountTypes Type { get; set; }
            public string AccountID { get; set; }
            public string ProviderID { get; set; }
            public string Authority { get; set; }
            public string Token { get; set; }
        }

        public class WebAccountProviderInfo
        {
            public WebAccountProviderInfo()
            {
                this.RequestProperties = new Dictionary<string, string>();
            }

            public WebAccountTypes WebAccountType { get; set; }
            public string ProviderID { get; set; }
            public string ClientID { get; set; }
            public string Scope { get; set; }
            public string Authority { get; set; }
            public Dictionary<string, string> RequestProperties { get; set; }
            public SupportedWebAccountActions Actions { get; set; }
        }

        #endregion
    }
}

#region Extensions

public static class WebProviderErrorExtensions
{
    /// <summary>
    /// Converts a WebProviderError into an Exception.
    /// </summary>
    /// <param name="element"></param>
    /// <returns>Exception instance.</returns>
    public static Exception ToException(this WebProviderError element)
    {
        return new Exception(string.Format("WebProviderError Exception {0}: {1}", element.ErrorCode, element.ErrorMessage));
    }
}

#endregion
