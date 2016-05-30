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

using System;
using Newtonsoft.Json;

namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Base class for any response object returned by your client API.
    /// </summary>
    [JsonObject(Title = "RootObject")]
    public abstract class ResponseBase : ModelBase
    {
    }

    /// <summary>
    /// Response object from the forgotten password end point.
    /// </summary>
    public class ForgotPasswordResponse : ResponseBase
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Response object from the authentication end point.
    /// </summary>
    public class UserResponse : ResponseBase
    {
        private string _AccessToken;
        public string AccessToken
        {
            get { return _AccessToken; }
            set { this.SetProperty(ref _AccessToken, value); }
        }

        private string _RefreshToken;
        public string RefreshToken
        {
            get { return _RefreshToken; }
            set { this.SetProperty(ref _RefreshToken, value); }
        }

        private string _ID;
        public string ID
        {
            get { return _ID; }
            set { this.SetProperty(ref _ID, value); }
        }

        private string _Email;
        public string Email
        {
            get { return _Email; }
            set { this.SetProperty(ref _Email, value); }
        }

        private string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set { if (this.SetProperty(ref _FirstName, value)) this.NotifyPropertyChanged(() => this.DisplayName); }
        }

        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set { if (this.SetProperty(ref _LastName, value)) this.NotifyPropertyChanged(() => this.DisplayName); }
        }

        private string _ProfileImageURL;
        public string ProfileImageURL
        {
            get { return _ProfileImageURL; }
            set { this.SetProperty(ref _ProfileImageURL, value); }
        }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName).Trim();
            }
        }
    }
}
