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
using System.Collections.Generic;
using Windows.ApplicationModel.Store;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the app info service of the platform currently executing.
        /// </summary>
        public AppInfoProvider AppInfo
        {
            get { return this.GetService<AppInfoProvider>(); }
            protected set { this.SetService<AppInfoProvider>(value); }
        }
    }

    /// <summary>
    /// Base class providing access to the application currently executing specific to the platform this app is executing on.
    /// </summary>
    public sealed class AppInfoProvider : ServiceBase
    {
        #region Variables

        private LicenseInformation _licenseInfo = null;

        #endregion

        #region Constructors

        public AppInfoProvider()
        {
#if DEBUG
            _licenseInfo = CurrentAppSimulator.LicenseInformation;
#else
            try
            {
                _licenseInfo = CurrentApp.LicenseInformation;
            }
            catch
            {
                _licenseInfo = CurrentAppSimulator.LicenseInformation;
            }
#endif
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets version number of the application currently executing.
        /// </summary>
        public Version VersionNumber
        {
            get
            {
                return Windows.ApplicationModel.Package.Current.Id.Version.ToVersion();
            }
        }
        
        /// <summary>
        /// Creates a deep link to your application with the specified arguments.
        /// </summary>
        /// <param name="arguments">Dictionary of different parameters to create a query string for the arguments.</param>
        /// <returns>String representing the deep link.</returns>
        public string GetDeepLink(Dictionary<string, string> arguments = null)
        {
            return this.GetDeepLink(GeneralFunctions.CreateQuerystring(arguments));
        }

        /// <summary>
        /// Creates a deep link to your application with the specified arguments.
        /// </summary>
        /// <param name="arguments">String representation of the arguments.</param>
        /// <returns>String representing the deep link.</returns>
        public string GetDeepLink(string arguments)
        {
            return "MediaAppSample:" + arguments;
        }

        /// <summary>
        /// Gets the deep link URL to where this application can be downloaded from.
        /// </summary>
        public string StoreURL
        {
            get { return string.Format("ms-windows-store:PDP?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName); }
        }

        /// <summary>
        /// Gets whether or not this application is running in trial mode.
        /// </summary>
        public bool IsTrial
        {
            get { return _licenseInfo.IsTrial; }
        }

        /// <summary>
        /// Gets whether or not this application trial is expired.
        /// </summary>
        public bool IsTrialExpired
        {
            get { return (_licenseInfo.IsActive == false); }
        }

        /// <summary>
        /// Gets the DateTime of when this application's trial will expire.
        /// </summary>
        public DateTime TrialExpirationDate
        {
            get { return _licenseInfo.ExpirationDate.DateTime; }
        }

        #endregion Properties
    }
}
