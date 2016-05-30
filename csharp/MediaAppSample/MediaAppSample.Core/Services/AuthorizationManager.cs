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

using MediaAppSample.Core.Models;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the cryptography provider of the platform currently executing.
        /// </summary>
        public AuthorizationManager AuthManager
        {
            get { return this.GetService<AuthorizationManager>(); }
            protected set { this.SetService<AuthorizationManager>(value); }
        }
    }

    public sealed class AuthorizationManager : ServiceBase, IServiceSignout
    {
        #region Variables
        
        private const string CREDENTIAL_USER_KEYNAME = "MediaAppSampleUser";
        private const string CREDENTIAL_ACCESSTOKEN_KEYNAME = "MediaAppSampleAccessToken";
        private const string CREDENTIAL_REFRESHTOKEN_KEYNAME = "MediaAppSampleAccessToken";

        #endregion

        #region Events

        public event EventHandler<bool> UserAuthenticatedStatusChanged;

        #endregion

        #region Properties

        private string _AccessToken;
        public string AccessToken
        {
            get { return _AccessToken; }
            private set { this.SetProperty(ref _AccessToken, value); }
        }

        private string _RefreshToken;
        public string RefreshToken
        {
            get { return _RefreshToken; }
            private set { this.SetProperty(ref _RefreshToken, value); }
        }

        private UserResponse _User;
        public UserResponse User
        {
            get { return _User; }
            private set { this.SetProperty(ref _User, value); }
        }

        #endregion

        #region Constructors

        internal AuthorizationManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Indicates whether or not a user is authenticated into this app.
        /// </summary>
        /// <returns>True if the user is authenticated else false.</returns>
        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(this.User?.AccessToken);
        }

        /// <summary>
        /// Notify any subscribers that the user authentication status has changed.
        /// </summary>
        private void NotifyUserAuthenticated()
        {
            this.UserAuthenticatedStatusChanged?.Invoke(null, this.IsAuthenticated());
        }

        /// <summary>
        /// Initialization logic which is called on launch of this application.
        /// </summary>
        protected internal override void Initialize()
        {
            // Retrieve the access token from the credential locker
            string access_token_value = null;
            if (Platform.Current.Storage.LoadCredential(CREDENTIAL_USER_KEYNAME, CREDENTIAL_ACCESSTOKEN_KEYNAME, ref access_token_value))
                this.AccessToken = access_token_value;

            // Retrieve the refresh token from the credential locker
            string refresh_token_value = null;
            if (Platform.Current.Storage.LoadCredential(CREDENTIAL_USER_KEYNAME, CREDENTIAL_REFRESHTOKEN_KEYNAME, ref refresh_token_value))
                this.RefreshToken = refresh_token_value;

            // Retrieve the user profile data from settings
            this.User = Platform.Current.Storage.LoadSetting<UserResponse>(CREDENTIAL_USER_KEYNAME, ApplicationData.Current.RoamingSettings, SerializerTypes.Json);
            if (this.User != null)
            {
                this.User.AccessToken = this.AccessToken;
                this.User.RefreshToken = this.RefreshToken;
            }
            
            // Notify any subscribers that authentication status has changed
            this.NotifyUserAuthenticated();

            base.Initialize();
        }

        /// <summary>
        /// Sets the current user of the app.
        /// </summary>
        /// <param name="response"></param>
        internal void SetUser(UserResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            if (string.IsNullOrEmpty(response.AccessToken))
                throw new ArgumentException("Access token cannot be blank");
            if (string.IsNullOrEmpty(response.RefreshToken))
                throw new ArgumentException("Refresh token cannot be blank");

            // Store user data
            Platform.Current.Storage.SaveSetting(CREDENTIAL_USER_KEYNAME, response, ApplicationData.Current.RoamingSettings, SerializerTypes.Json);
            Platform.Current.Storage.SaveCredential(CREDENTIAL_USER_KEYNAME, CREDENTIAL_ACCESSTOKEN_KEYNAME, response.AccessToken);
            Platform.Current.Storage.SaveCredential(CREDENTIAL_USER_KEYNAME, CREDENTIAL_REFRESHTOKEN_KEYNAME, response.RefreshToken);

            // Set properties
            this.User = response;
            this.AccessToken = response.AccessToken;
            this.RefreshToken = response.RefreshToken;

            // Notify any subscribers that authentication status has changed
            this.NotifyUserAuthenticated();
        }

        /// <summary>
        /// Signs the user out of the application and removes and credential data from storage / credential locker.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public Task SignoutAsync()
        {
            Platform.Current.Storage.SaveSetting(CREDENTIAL_USER_KEYNAME, null, ApplicationData.Current.RoamingSettings, SerializerTypes.Json);
            Platform.Current.Storage.SaveCredential(CREDENTIAL_USER_KEYNAME, null, null);
            this.User = null;
            this.AccessToken = null;
            this.RefreshToken = null;
            this.NotifyUserAuthenticated();
            return Task.CompletedTask;
        }

        #endregion
    }
}