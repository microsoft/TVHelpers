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
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the app info service of the platform currently executing.
        /// </summary>
        public VoiceCommandManager VoiceCommandManager
        {
            get { return this.GetService<VoiceCommandManager>(); }
            protected set { this.SetService<VoiceCommandManager>(value); }
        }
    }

    public sealed class VoiceCommandManager : ServiceBase, IServiceSignout
    {
        #region Constructors

        internal VoiceCommandManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all items from the voice command definition on user signout.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public async Task SignoutAsync()
        {
            await this.ClearPhraseListAsync("CommandSet", "ItemName");
        }

        /// <summary>
        /// Clear all phrases for a command set.
        /// </summary>
        /// <param name="commandSetName">Name of the command set.</param>
        /// <param name="phraseListName">Name of the phrase list.</param>
        /// <param name="countryCode">Country code for the command set.</param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task ClearPhraseListAsync(string commandSetName, string phraseListName, string countryCode = null)
        {
            await this.UpdatePhraseListAsync(commandSetName, phraseListName, null, countryCode);
        }

        /// <summary>
        /// Updates all phrases in a command set.
        /// </summary>
        /// <param name="commandSetName">Name of the command set.</param>
        /// <param name="phraseListName">Name of the phrase list.</param>
        /// <param name="list">Strings for the phrase list.</param>
        /// <param name="countryCode">Country code for the command set.</param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task UpdatePhraseListAsync(string commandSetName, string phraseListName, IEnumerable<string> list, string countryCode = "en-us")
        {
            try
            {
                if (string.IsNullOrEmpty(countryCode))
                    countryCode = System.Globalization.CultureInfo.CurrentCulture.Name.ToLower();

                if (list == null)
                    list = new List<string>();

                // Update the destination phrase list, so that Cortana voice commands can use destinations added by users.
                // When saving a trip, the UI navigates automatically back to this page, so the phrase list will be
                // updated automatically.
                VoiceCommandDefinition cd;
                if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue(commandSetName + "_" + countryCode, out cd))
                    await cd.SetPhraseListAsync(phraseListName, list);
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while updating voice commands!");
            }
        }

        #endregion
    }
}
