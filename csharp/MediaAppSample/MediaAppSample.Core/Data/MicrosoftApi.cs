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
using System.Threading;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Data
{
    /// <summary>
    /// API access to Microsoft account data.
    /// </summary>
    public class MicrosoftApi : ClientApiBase
    {
        #region Variables

        private const string BASE_ADDRESS = "https://apis.live.net";
        private const string URL_PROFILE_DATA = "/v5.0/me?access_token={0}";

        #endregion

        #region Constructors

        public MicrosoftApi() : base(BASE_ADDRESS)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a user's Microsoft account profile data.
        /// </summary>
        /// <param name="token">Token to access the Microsoft account data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>User data else null if not found or invalid token.</returns>
        public async Task<MicrosoftAccountDetails> GetUserProfile(string token, CancellationToken? ct = null)
        {
            string url = string.Format(URL_PROFILE_DATA, token);
            return await this.GetAsync<MicrosoftAccountDetails>(url, ct, SerializerTypes.Json);
        }

        #endregion
    }
}

namespace MediaAppSample.Core.Models
{
    #region Classes

    public class MicrosoftAccountDetails
    {
        public string id { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string link { get; set; }
        public int birth_day { get; set; }
        public int birth_month { get; set; }
        public int birth_year { get; set; }
        public object gender { get; set; }
        public MicrosoftEmails emails { get; set; }
        public MicrosoftAddresses addresses { get; set; }
        public MicrosoftPhones phones { get; set; }
        public string locale { get; set; }
        public string updated_time { get; set; }
    }

    public class MicrosoftEmails
    {
        public string preferred { get; set; }
        public string account { get; set; }
        public string personal { get; set; }
        public string business { get; set; }
    }

    public class MicrosoftPersonal
    {
        public string street { get; set; }
        public object street_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string region { get; set; }
    }

    public class MicrosoftBusiness
    {
        public string street { get; set; }
        public object street_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string region { get; set; }
    }

    public class MicrosoftAddresses
    {
        public MicrosoftPersonal personal { get; set; }
        public MicrosoftBusiness business { get; set; }
    }

    public class MicrosoftPhones
    {
        public object personal { get; set; }
        public string business { get; set; }
        public string mobile { get; set; }
    }

    #endregion
}
