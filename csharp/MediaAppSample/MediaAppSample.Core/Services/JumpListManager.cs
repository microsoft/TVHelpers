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

using MediaAppSample.Core.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the app info service of the platform currently executing.
        /// </summary>
        public JumplistManager Jumplist
        {
            get { return this.GetService<JumplistManager>(); }
            protected set { this.SetService<JumplistManager>(value); }
        }
    }

    /// <summary>
    /// Base class providing access to the application currently executing specific to the platform this app is executing on.
    /// </summary>
    public sealed class JumplistManager : ServiceBase, IServiceSignout
    {
        #region Properties

        public static bool IsSupported { get; set; }
        
        private CommandBase _ClearJumplistCommand = null;
        /// <summary>
        /// Clears the task bar jump list of this application.
        /// </summary>
        public CommandBase ClearCommand
        {
            get { return _ClearJumplistCommand ?? (_ClearJumplistCommand = new NavigationCommand("Jumplist-ClearCommand", async () => await this.ClearAsync())); }
        }

        #endregion

        #region Constructors

        internal JumplistManager()
        {
        }

        static JumplistManager()
        {
            if(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.StartScreen.JumpList"))
                IsSupported = JumpList.IsSupported();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear out any jump list items on user signout to protect user senstive data.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public Task SignoutAsync()
        {
            if (IsSupported)
                return this.ClearAsync();
            else
                return Task.CompletedTask;
        }

        /// <summary>
        /// Adds an item to the app's jump list.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task AddItemAsync(JumpItemInfo info)
        {
            if (!IsSupported)
                return;

            if (info == null || string.IsNullOrEmpty(info.Name))
                throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(info.Name))
                throw new ArgumentNullException(nameof(info.Name));

            try
            {
                var jumpList = await JumpList.LoadCurrentAsync();

                if (jumpList.SystemGroupKind == JumpListSystemGroupKind.None)
                    jumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;

                // Remove item if already existing
                var existingItem = jumpList.Items.FirstOrDefault(f => f.DisplayName.Equals(info.Name, StringComparison.CurrentCultureIgnoreCase) || f.Arguments.Equals(info.Arguments, StringComparison.CurrentCultureIgnoreCase));
                if(existingItem != null)
                    jumpList.Items.Remove(existingItem);

                // Add item to the top of the list
                var item = JumpListItem.CreateWithArguments(info.Arguments, info.Name);
                item.Description = info.Description ?? string.Empty;
                if(!string.IsNullOrEmpty(info.GroupName)) item.GroupName = info.GroupName;

                //item.GroupName = info.GroupName ?? jumpList.SystemGroupKind.ToString();

                item.Logo = info.Logo;
                jumpList.Items.Add(item);

                // Save the updated list
                await jumpList.SaveAsync();
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not add to jump list!");
            }
        }

        /// <summary>
        /// Clears all items from the jump list.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public async Task ClearAsync()
        {
            if (!IsSupported)
                return;

            // Get the app's jump list.
            var jumpList = await JumpList.LoadCurrentAsync();

            // Disable the system-managed jump list group.
            jumpList.SystemGroupKind = JumpListSystemGroupKind.None;

            // Remove any previously added custom jump list items.
            jumpList.Items.Clear();

            // Save the changes to the app's jump list.
            await jumpList.SaveAsync();
        }

        #endregion
    }

    #region Classes

    public class JumpItemInfo
    {
        public JumpItemInfo()
        {
            this.Logo = new Uri("ms-appx:///Assets/Square44x44Logo.png");
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public Uri Logo { get; set; }
        public string Arguments { get; set; }
    }

    #endregion
}
