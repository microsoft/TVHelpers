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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the notifications service of the platform currently executing. Provides you the ability to display toasts or manage tiles or etc on the executing platform.
        /// </summary>
        public NotificationsService Notifications
        {
            get { return this.GetService<NotificationsService>(); }
            protected set { this.SetService<NotificationsService>(value); }
        }
    }

    public sealed partial class NotificationsService : ServiceBase, IServiceSignout
    {
        #region Constructors

        internal NotificationsService()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialization logic which is called on launch of this application.
        /// </summary>
        protected internal override void Initialize()
        {
            // TODO Register push notifications address here
            base.Initialize();
        }

        #region Public

        /// <summary>
        /// On signout of a user, clear tiles, toasts, notifications
        /// </summary>
        /// <returns></returns>
        public async Task SignoutAsync()
        {
            ToastNotificationManager.History.Clear();

            // Clear primary tile
            this.ClearTile(Platform.Current.ViewModel);

            // Clear secondary tiles
            IReadOnlyList<SecondaryTile> list = await SecondaryTile.FindAllAsync();
            foreach (var tile in list)
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Clear();
        }

        /// <summary>
        /// Checks to see if a tile exists associated the the model specified. How it determines which tile to check against is determined by the
        /// class implementing this interface. If the platform does not support tiles, then the implementation should do nothing.
        /// </summary>
        /// <param name="model">Model which contains the data to find the tile.</param>
        /// <returns>True if a tile exists associated to the model specified or false if no tile was found.</returns>
        public bool HasTile(IModel model)
        {
            var tileID = Platform.Current.GenerateModelTileID(model);
            if (tileID == string.Empty)
                return true;
            else if (!string.IsNullOrEmpty(tileID))
                return SecondaryTile.Exists(tileID);
            else
                return false;
        }

        /// <summary>
        /// Updates all tiles or any other UI features currently in use on the device.
        /// </summary>
        public async Task UpdateAllSecondaryTilesAsync(CancellationToken ct)
        {
            // Find all secondary tiles
            var list = await SecondaryTile.FindAllAsync();
            foreach (var tile in list)
            {
                var model = await Platform.Current.GenerateModelFromTileIdAsync(tile.TileId, ct);
                ct.ThrowIfCancellationRequested();

                if (model != null)
                {
                    await this.CreateOrUpdateTileAsync(model);
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        /// <summary>
        /// Deletes a tile associated to the model specified. How it determines which tile to delete is determined by the class implementing this
        /// interface. If the platform does not support tiles, then the implementation should do nothing.
        /// </summary>
        /// <param name="model">Model which contains the data necessary to find the tile to delete.</param>
        public async Task<bool> DeleteTileAsync(IModel model)
        {
            var tileID = Platform.Current.GenerateModelTileID(model);
            if (!string.IsNullOrEmpty(tileID))
            {
                SecondaryTile tile = new SecondaryTile(tileID);
                return await tile.RequestDeleteAsync();
            }
            else
                throw new ArgumentException("Tile does not exist for model!");
        }

        /// <summary>
        /// Clears a tile associated to the model specified. How it determines which tile(s) to clear is determined by the class implementing this
        /// interface. If the platform does not support tiles, then the implementation should do nothing.
        /// </summary>
        /// <param name="model">Model which contains the data necessary to find the tile to clear.</param>
        public void ClearTile(IModel model)
        {
            var tileID = Platform.Current.GenerateModelTileID(model);
            if (tileID == string.Empty)
                TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            else if (!string.IsNullOrEmpty(tileID))
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileID).Clear();
            else
                throw new ArgumentException("Tile does not exist for model!");
        }

        #endregion

        #region Private

        private class TileVisualOptions
        {
            public TileVisualOptions()
            {
                this.Square71x71Logo = new Uri("ms-appx:///Assets/Square71x71Logo.png");
                this.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png");
                this.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
                this.Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");
            }
            public Uri Square71x71Logo { get; set; }
            public Uri Square150x150Logo { get; set; }
            public Uri Wide310x150Logo { get; set; }
            public Uri Square310x310Logo { get; set; }
            public Rect Rect { get; set; }
            public Windows.UI.Popups.Placement PopupPlacement { get; set; }
        }

        private async Task<bool> CreateOrUpdateSecondaryTileAsync(SecondaryTile tile, TileVisualOptions options)
        {
            if (tile == null)
                return false;

            tile.VisualElements.ShowNameOnSquare150x150Logo = true;

            tile.VisualElements.Square71x71Logo = options.Square71x71Logo ?? null;
            tile.VisualElements.Square150x150Logo = options.Square150x150Logo ?? null;

            if (!(ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons"))))
            {
                tile.VisualElements.Wide310x150Logo = options.Wide310x150Logo ?? null;
                tile.VisualElements.Square310x310Logo = options.Square310x310Logo ?? null;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;
            }

            if (SecondaryTile.Exists(tile.TileId))
            {
                return await tile.UpdateAsync();
            }
            else
            {
                if (!ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons")))
                {
                    if (options.Rect == null)
                        return await tile.RequestCreateAsync();
                    else
                        return await tile.RequestCreateForSelectionAsync(options.Rect, options.PopupPlacement);
                }
                else if (ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons")))
                {
                    // OK, the tile is created and we can now attempt to pin the tile.
                    // Since pinning a secondary tile on Windows Phone will exit the app and take you to the start screen, any code after 
                    // RequestCreateForSelectionAsync or RequestCreateAsync is not guaranteed to run.  For an example of how to use the OnSuspending event to do
                    // work after RequestCreateForSelectionAsync or RequestCreateAsync returns, see Scenario9_PinTileAndUpdateOnSuspend in the SecondaryTiles.WindowsPhone project.
                    return await tile.RequestCreateAsync();
                }
            }

            return false;
        }

        #endregion

        #endregion
    }
}
