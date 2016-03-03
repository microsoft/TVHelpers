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
            get { return this.GetAdapter<VoiceCommandManager>(); }
            set { this.Register<VoiceCommandManager>(value); }
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
