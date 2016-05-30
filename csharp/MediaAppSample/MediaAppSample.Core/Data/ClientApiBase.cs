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
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MediaAppSample.Core.Data
{
    /// <summary>
    /// Base class for any SDK client API implementation containing reusable logic for common call types, error handling, request retry attempts.
    /// </summary>
    public abstract class ClientApiBase : IDisposable
    {
        #region Variables

        protected HttpClient Client { get; private set; }

        protected Uri BaseUri { get; private set; }
        
        private const int E_WINHTTP_TIMEOUT = unchecked((int)0x80072ee2);
        private const int E_WINHTTP_NAME_NOT_RESOLVED = unchecked((int)0x80072ee7);
        private const int E_WINHTTP_CANNOT_CONNECT = unchecked((int)0x80072efd);
        private const int E_WINHTTP_CONNECTION_ERROR = unchecked((int)0x80072efe);

        #endregion

        #region Constructors

        public ClientApiBase(string baseURL)
        {
            if (string.IsNullOrEmpty(baseURL))
                throw new ArgumentNullException(nameof(baseURL));

            this.BaseUri = new Uri(baseURL);
            this.Client = new HttpClient();
        }

        public void Dispose()
        {
            this.Client.Dispose();
            this.Client = null;
        }

        #endregion

        #region Methods

        #region Get

        /// <summary>
        /// Gets data from the specified URL.
        /// </summary>
        /// <typeparam name="T">Type for the strongly typed class representing data returned from the URL.</typeparam>
        /// <param name="url">URL to retrieve data from.</param>
        /// <param name="serializerType">Specifies how the data should be deserialized.</param>
        /// <param name="retryCount">Number of retry attempts if a call fails. Default is zero.</param>
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        /// <summary>
        protected async Task<T> GetAsync<T>(string url, CancellationToken? ct = null, SerializerTypes serializerType = SerializerTypes.Default)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var response = await this.Client.GetAsync(new Uri(this.BaseUri, url)).AsTask(ct.HasValue ? ct.Value : CancellationToken.None);
            this.Log(response);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return Serializer.Deserialize<T>(data, serializerType);
        }

        #endregion

        #region Post

        /// <summary>
        /// Posts data to the specified URL.
        /// </summary>
        /// <typeparam name="T">Type for the strongly typed class representing data returned from the URL.</typeparam>
        /// <param name="url">URL to retrieve data from.</param>
        /// <param name="contents">Any content that should be passed into the post.</param>
        /// <param name="serializerType">Specifies how the data should be deserialized.</param>
        /// <param name="retryCount">Number of retry attempts if a call fails. Default is zero.</param>
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        /// <summary>
        protected async Task<T> PostAsync<T, R>(string url, R contents = default(R), CancellationToken? ct = null, SerializerTypes serializerType = SerializerTypes.Default) where R : IHttpContent
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var response = await this.Client.PostAsync(new Uri(this.BaseUri, url), contents).AsTask(ct.HasValue ? ct.Value : CancellationToken.None);
            this.Log(response);
            response.EnsureSuccessStatusCode();
            var data = await response.Content?.ReadAsStringAsync();
            return Serializer.Deserialize<T>(data, serializerType);
        }

        //public async Task<JsonValue> PostAsync(string relativeUri)
        //{
        //    HttpClient httpClient = new HttpClient();
        //    HttpResponseMessage httpResponse = null;
        //    try
        //    {
        //        httpResponse = await httpClient.PostAsync(new Uri(this.BaseUri, relativeUri), content);
        //    }
        //    catch (Exception ex)
        //    {
        //        switch (ex.HResult)
        //        {
        //            case E_WINHTTP_TIMEOUT:
        //            // The connection to the server timed out.
        //            case E_WINHTTP_NAME_NOT_RESOLVED:
        //            case E_WINHTTP_CANNOT_CONNECT:
        //            case E_WINHTTP_CONNECTION_ERROR:
        //            // Unable to connect to the server. Check that you have Internet access.
        //            default:
        //                // "Unexpected error connecting to server: ex.Message
        //                return null;
        //        }
        //    }

        //    // We assume that if the server responds at all, it responds with valid JSON.
        //    return JsonValue.Parse(await httpResponse.Content.ReadAsStringAsync());
        //}

        #endregion

        #region Logging

        /// <summary>
        /// Logs HttpRequest information to the application logger.
        /// </summary>
        /// <param name="request">Request to log.</param>
        private void Log(HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (Platform.Current.Logger.CurrentLevel > LogLevels.Debug)
                return;

            Platform.Current.Logger.Log(LogLevels.Debug,
                Environment.NewLine + "---------------------------------" + Environment.NewLine +
                "WEB REQUEST to {0}" + Environment.NewLine +
                "-Method: {1}" + Environment.NewLine +
                "-Headers: {2}" + Environment.NewLine +
                "-Contents: " + Environment.NewLine + "{3}" + Environment.NewLine +
                "---------------------------------",
                request.RequestUri.OriginalString,
                request.Method.Method,
                request.Headers?.ToString(),
                request.Content?.ReadAsStringAsync().AsTask().Result
                );
        }

        /// <summary>
        /// Logs the HttpResponse object to the application logger.
        /// </summary>
        /// <param name="response">Response to log.</param>
        private void Log(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (Platform.Current.Logger.CurrentLevel > LogLevels.Debug)
                return;

            this.Log(response.RequestMessage);
            Platform.Current.Logger.Log(LogLevels.Debug,
                Environment.NewLine + "---------------------------------" + Environment.NewLine +
                "WEB RESPONSE to {0}" + Environment.NewLine +
                "-HttpStatus: {1}" + Environment.NewLine +
                "-Reason Phrase: {2}" + Environment.NewLine +
                "-ContentLength: {3:0.00 KB}" + Environment.NewLine +
                "-Contents: " + Environment.NewLine + "{4}" + Environment.NewLine +
                "---------------------------------",
                response.RequestMessage.RequestUri.OriginalString,
                string.Format("{0} {1}", (int)response.StatusCode, response.StatusCode.ToString()),
                response.ReasonPhrase,
                Convert.ToDecimal(Convert.ToDouble(response.Content.Headers.ContentLength) / 1024),
                response.Content?.ReadAsStringAsync().AsTask().Result
                );
        }

        #endregion

        #endregion
    }
}
