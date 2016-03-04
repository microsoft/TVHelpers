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
            return await this.GetAsync<MicrosoftAccountDetails>(url, SerializerTypes.Json, ct);
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
