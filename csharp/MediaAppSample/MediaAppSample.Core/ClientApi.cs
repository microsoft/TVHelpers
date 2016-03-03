using MediaAppSample.Core.Models;
using MediaAppSample.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace MediaAppSample.Core
{
    /// <summary>
    /// Client SDK to access remote data.
    /// </summary>
    public sealed class ClientApi : ClientApiBase
    {
        #region Variables

        private const string BASE_ADDRESS = "https://localhost:59992";
        private const string URL_ACCOUNT_SIGNIN = "/auth/signin";
        private const string URL_ACCOUNT_SIGNUP = "/auth/signup";
        private const string URL_ACCOUNT_FORGOT = "/auth/forgot";

        #endregion

        #region Constructors

        public ClientApi() : base(BASE_ADDRESS)
        {
            //// Instantiate constructor and all headers
            //this.Client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
            //this.Client.DefaultRequestHeaders.Add("Accept-Language", "en-us");
            //this.Client.DefaultRequestHeaders.Add("Accept", "*/*");
            //this.Client.DefaultRequestHeaders.Add("User-Agent", string.Format("MediaAppSample/{0}", Platform.Current.AppInfo.VersionNumber));
            //this.Client.DefaultRequestHeaders.Add("Authorization", "Basic YzExNGEzM2U4YjNhNDdmY2E3NzBhYmJiMGNlOWE0YjE6NDFjOTcxYTU3NzlhNGZhMGI4NGZmN2EzNTA4NTQ5M2U=");
        }

        #endregion

        #region Methods

        #region Authentication/Registration

        /// <summary>
        /// Performs authentication for the application.
        /// </summary>
        /// <param name="vm">Sign in view model instance that contains the user's login information.</param>
        /// <returns>Login reponse with authorization details.</returns>
        internal async Task<UserResponse> AuthenticateAsync(AccountSignInViewModel vm, CancellationToken? ct = null)
        {
            var response = new UserResponse() { AccessToken = "1234567890", RefreshToken = "abcdefghijklmnop", ID = vm.Username, Email = vm.Username, FirstName = "John", LastName = "Doe" };
            await Task.Delay(2000, ct.HasValue ? ct.Value : CancellationToken.None);
            return response;

            //this.Client.DefaultRequestHeaders.Add("Authorization", "Basic YzExNGEzM2U4YjNhNDdmY2E3NzBhYmJiMGNlOWE0YjE6NDFjOTcxYTU3NzlhNGZhMGI4NGZmN2EzNTA4NTQ5M2U=");

            //var dic = new Dictionary<string, string>();
            //dic.Add("grant_type", "password");
            //dic.Add("username", vm.Username);
            //dic.Add("password", vm.Password);
            //dic.Add("scope", "streaming");
            //var contents = new HttpFormUrlEncodedContent(dic);

            //HttpStringContent content = new HttpStringContent(message.Stringify(), UnicodeEncoding.Utf8, "application/json");

            //return await this.PostAsync<UserResponse, HttpFormUrlEncodedContent>(URL_ACCOUNT_SIGNIN, contents, SerializerTypes.Json);
        }

        /// <summary>
        /// Performs account creation for the application.
        /// </summary>
        /// <param name="vm">Sign up view model instance containing all the user's registration information.</param>
        /// <returns>Login response and authorization information if the account creation process was successful.</returns>
        internal async Task<UserResponse> RegisterAsync(AccountSignUpViewModel vm, CancellationToken? ct = null)
        {
            var response = new UserResponse() { AccessToken = "0987654321", RefreshToken = "qrstuvwxwyz", ID = vm.Username, Email = vm.Username, FirstName = vm.FirstName, LastName = vm.LastName };
            await Task.Delay(2000, ct.HasValue ? ct.Value : CancellationToken.None);
            return response;

            //var dic = new Dictionary<string, string>();
            //dic.Add("grant_type", "password");
            //dic.Add("username", vm.Username);
            //dic.Add("password", vm.Password1);
            //dic.Add("scope", "streaming");
            //var contents = new HttpFormUrlEncodedContent(dic);

            //HttpStringContent content = new HttpStringContent(message.Stringify(), UnicodeEncoding.Utf8, "application/json");

            //return await this.PostAsync<UserResponse, HttpFormUrlEncodedContent>(URL_ACCOUNT_SIGNUP, contents);
        }

        /// <summary>
        /// Requests forgotten account information when a user cannot rememeber their authentication details.
        /// </summary>
        /// <param name="vm">Account forgot view model instance contain partial account details.</param>
        /// <returns>Response information indicating whether the call was successful or not.</returns>
        internal async Task<ForgotPasswordResponse> ForgotPasswordAsync(AccountForgotViewModel vm, CancellationToken? ct = null)
        {
            var response = new ForgotPasswordResponse() { IsValid = true, Message = "Your password has been sent to your e-mail!" };
            await Task.Delay(2000, ct.HasValue ? ct.Value : CancellationToken.None);
            return response;
        }

        #endregion

        #region Microsoft Account

        /// <summary>
        /// Authenticates a user account returned from the Web Account Manager service.
        /// </summary>
        /// <param name="wi">Web account info object instance representing an authenticated WAM user.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response object from the server.</returns>
        internal async Task<UserResponse> AuthenticateAsync(Services.WebAccountManager.WebAccountInfo wi, CancellationToken? ct = null)
        {
            // This logic below should be server side. Token should be used to retrieve MSA and then check to see if MediaAppSample account exists else register new account.

            switch (wi.Type)
            {
                case Services.WebAccountManager.WebAccountTypes.Microsoft:
                    {
                        // Retrieve MSA profile data
                        MicrosoftAccountDetails msa = null;
                        using (var api = new MicrosoftApi())
                        {
                            msa = await api.GetUserProfile(wi.Token, ct);
                        }

                        if (msa == null)
                            throw new Exception("Could not retrieve Microsoft account profile data!");

                        var response = await this.IsMicrosoftAccountRegistered(msa.id, ct);
                        if (response != null)
                        {
                            // User account exists, return response
                            return response;
                        }
                        else
                        {
                            // No account exists, use MSA profile to register user
                            AccountSignUpViewModel vm = new AccountSignUpViewModel();

                            // Set all the MSA data to the ViewModel
                            vm.Populate(msa);

                            // Call the registration API to create a new account and return
                            return await this.RegisterAsync(vm);
                        }
                    }

                default:
                    throw new NotImplementedException(wi.Type.ToString());
            }
        }

        /// <summary>
        /// Checks to see if a MSA account ID is an existing user of this app service or not.
        /// </summary>
        /// <param name="id">Unique MSA account ID.</param>
        /// <param name="ct">Cancelation token.</param>
        /// <returns>Response object from the server.</returns>
        private Task<UserResponse> IsMicrosoftAccountRegistered(string id, CancellationToken? ct = null)
        {
            // TODO server side logic to check if MSA user is existing or not as a user of this application. Returns "false" in this sample.
            return Task.FromResult<UserResponse>(null);
        }

        #endregion

        #region Sample Data

        /// <summary>
        /// Gets sample data.
        /// </summary>
        /// <param name="ct">Cancelation token.</param>
        /// <returns>List of items.</returns>
        public Task<IEnumerable<ItemModel>> GetItems(CancellationToken? ct)
        {
            var list = new List<ItemModel>();
            list.Add(new ItemModel() { ID = 1, LineOne = "Runtime One", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu", Latitude = 40, Longitude = -87 });
            list.Add(new ItemModel() { ID = 2, LineOne = "Runtime Two", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus", Latitude = 1, Longitude = 10 });
            list.Add(new ItemModel() { ID = 3, LineOne = "Runtime Three", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent", Latitude = 2, Longitude = 20 });
            list.Add(new ItemModel() { ID = 4, LineOne = "Runtime Four", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos", Latitude = 3, Longitude = 30 });
            list.Add(new ItemModel() { ID = 5, LineOne = "Runtime Five", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur", Latitude = 4, Longitude = 40 });
            list.Add(new ItemModel() { ID = 6, LineOne = "Runtime Six", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent", Latitude = 5, Longitude = 50 });
            list.Add(new ItemModel() { ID = 7, LineOne = "Runtime Seven", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat", Latitude = 6, Longitude = 60 });
            list.Add(new ItemModel() { ID = 8, LineOne = "Runtime Eight", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum", Latitude = 7, Longitude = 70 });
            list.Add(new ItemModel() { ID = 9, LineOne = "Runtime Nine", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu", Latitude = 8, Longitude = 80 });
            list.Add(new ItemModel() { ID = 10, LineOne = "Runtime Ten", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus", Latitude = 9, Longitude = 90 });
            list.Add(new ItemModel() { ID = 11, LineOne = "Runtime Eleven", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent", Latitude = 10, Longitude = 100 });
            list.Add(new ItemModel() { ID = 12, LineOne = "Runtime Twelve", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos", Latitude = 11, Longitude = 110 });
            list.Add(new ItemModel() { ID = 13, LineOne = "Runtime Thirteen", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur", Latitude = 12, Longitude = 120 });
            list.Add(new ItemModel() { ID = 14, LineOne = "Runtime Fourteen", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent", Latitude = 13, Longitude = 130 });
            list.Add(new ItemModel() { ID = 15, LineOne = "Runtime Fifteen", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat", Latitude = 14, Longitude = 140 });
            list.Add(new ItemModel() { ID = 16, LineOne = "Runtime Sixteen", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum", Latitude = 15, Longitude = 150 });
            if (ct.HasValue) ct.Value.ThrowIfCancellationRequested();
            return Task.FromResult<IEnumerable<ItemModel>>(list);
        }

        /// <summary>
        /// Gets a sample data item by ID.
        /// </summary>
        /// <param name="id">ID to retrieve.</param>
        /// <param name="ct">Cancelation token.</param>
        /// <returns>ItemModel instance matching the specified ID.</returns>
        public async Task<ItemModel> GetItemByID(int id, CancellationToken? ct)
        {
            var items = await this.GetItems(ct);
            if (ct.HasValue) ct.Value.ThrowIfCancellationRequested();
            await Task.Delay(3000, ct.HasValue ? ct.Value : CancellationToken.None);
            if (ct.HasValue) ct.Value.ThrowIfCancellationRequested();
            return items.FirstOrDefault(f => f.ID == id);
        }

        /// <summary>
        /// Gets sample data matching the search text provided.
        /// </summary>
        /// <param name="searchText">Text to search.</param>
        /// <param name="ct">Cancelation token.</param>
        /// <returns>List of items matching the search text.</returns>
        public async Task<IEnumerable<ItemModel>> SearchItems(string searchText, CancellationToken? ct)
        {
            var items = await this.GetItems(ct);
            var results = new List<ItemModel>();

            foreach (var item in items)
            {
                if(ct.HasValue) ct.Value.ThrowIfCancellationRequested();

                if (item.ID.ToString().IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                    || item.LineOne.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                    || item.LineTwo.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                    || item.LineThree.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    results.Add(item);
                }
            }

            return results.ToArray();
        }

        #endregion

        #endregion
    }
}
