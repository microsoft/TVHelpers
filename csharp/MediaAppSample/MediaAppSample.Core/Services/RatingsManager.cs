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

using MediaAppSample.Core.ViewModels;
using System;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the ratings manager used to help promote users to rate your application.
        /// </summary>
        public RatingsManager Ratings
        {
            get { return this.GetService<RatingsManager>(); }
            protected set { this.SetService<RatingsManager>(value); }
        }
    }

    /// <summary>
    /// Used to determine when and if a user should be prompted to rate the application being executed.
    /// </summary>
    public sealed class RatingsManager : ServiceBase
    {
        #region Properties

        private const string LAST_PROMPTED_FOR_RATING = "LastPromptedForRating";
        private const string LAUNCH_COUNT = "LaunchCount";

        private DateTime LastPromptedForRating { get; set; }

        #endregion

        #region Constructors

        internal RatingsManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes business logic to determine if an instance of the application should prompt the user to solicit user ratings.
        /// If it determines it should, the dialog to solicit ratings will be displayed.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public async Task CheckForRatingsPromptAsync(ViewModelBase vm)
        {
            bool showPrompt = false;

            // PLACE YOUR CUSTOM RATE PROMPT LOGIC HERE!
            this.LastPromptedForRating = Platform.Current.Storage.LoadSetting<DateTime>(LAST_PROMPTED_FOR_RATING);

            // If trial, not expired, and less than 2 days away from expiring, set as TRUE
            bool preTrialExpiredBasedPrompt = 
                Platform.Current.AppInfo.IsTrial 
                && !Platform.Current.AppInfo.IsTrialExpired 
                && DateTime.Now.AddDays(2) > Platform.Current.AppInfo.TrialExpirationDate;

            if (preTrialExpiredBasedPrompt && this.LastPromptedForRating == DateTime.MinValue)
            {
                showPrompt = true;
            }
            else if (this.LastPromptedForRating != DateTime.MinValue && this.LastPromptedForRating.AddDays(21) < DateTime.Now)
            {
                // Every X days after the last prompt, set as TRUE
                showPrompt = true;
            }
            else if(this.LastPromptedForRating == DateTime.MinValue && Windows.ApplicationModel.Package.Current.InstalledDate.DateTime.AddDays(3) < DateTime.Now)
            {
                // First time prompt X days after initial install
                showPrompt = true;
            }

            if(showPrompt)
                await this.PromptForRatingAsync(vm);
        }

        /// <summary>
        /// Displays a dialog to the user requesting the user to provide ratings/feedback for this application.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        private async Task PromptForRatingAsync(ViewModelBase vm)
        {
            // Prompt the user to rate the app
            var result = await vm.ShowMessageBoxAsync(Strings.Resources.PromptRateApplicationMessage, Strings.Resources.PromptRateApplicationTitle, new string[] { Strings.Resources.TextYes, Strings.Resources.TextMaybeLater }, 1);

            // Store the time the user was prompted
            Platform.Current.Storage.SaveSetting(LAST_PROMPTED_FOR_RATING, DateTime.Now);

            if (result == 0)
            {
                // Navigate user to the platform specific rating mechanism
                await Platform.Current.Navigation.RateApplicationAsync();
            }
        }

        #endregion
    }
}