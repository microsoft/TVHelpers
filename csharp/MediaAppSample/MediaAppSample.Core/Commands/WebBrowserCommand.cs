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

namespace MediaAppSample.Core.Commands
{
    /// <summary>
    /// Command for navigating to an internal web browser to display a webpage within the app.
    /// </summary>
    public sealed class WebViewCommand : GenericCommand<string>
    {
        #region Constructors

        /// <summary>
        /// Create an instance of the command for internal webpage browsing.
        /// </summary>
        public WebViewCommand()
            : base("WebViewCommand", Platform.Current.Navigation.NavigateToWebView, (address) => { return address is string && !string.IsNullOrWhiteSpace(address.ToString()); })
        {
        }

        #endregion
    }

    /// <summary>
    /// Command for navigating to an external web browser to display a webpage.
    /// </summary>
    public sealed class WebBrowserCommand : GenericCommand<string>
    {
        #region Constructors

        /// <summary>
        /// Create an instance of the command for external webpage browsing.
        /// </summary>
        public WebBrowserCommand()
            : base("WebBrowserCommand", Platform.Current.Navigation.NavigateToWebBrowser, (address) => { return address is string && !string.IsNullOrWhiteSpace(address.ToString()); })
        {
        }

        #endregion
    }
}