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
using MediaAppSample.Core.ViewModels;
using NotificationsExtensions;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;
using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MediaAppSample.Core.Services
{
    public sealed partial class NotificationsService : ServiceBase
    {
        #region Methods

        /// <summary>
        /// Creates or updates one or multiple live tiles on the executing platform. How it is manifests itself is determined by the class that
        /// implements this interface. If the platform does not support this feature, then the implementation should do nothing.
        /// </summary>
        /// <param name="model">Model which contains the data necessary to create a new tile or to find the tile to update</param>
        public async Task<bool> CreateOrUpdateTileAsync(IModel model)
        {
            if (model == null)
                return false;

            await Task.CompletedTask;

            // Do custom tile creation based on the type of the model passed into this method.
            if (model is QueueViewModel)
            {
                var list = (model as QueueViewModel)?.Queue;

                // Get the blank badge XML payload for a badge number
                XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

                // Set the value of the badge in the XML to our number
                XmlElement badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
                badgeElement.SetAttribute("value", list?.Count.ToString());

                // Create the badge notification
                BadgeNotification badge = new BadgeNotification(badgeXml);

                // Create the badge updater for the application
                BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();

                // And update the badge
                badgeUpdater.Update(badge);
            }
            else if (model is ContentItemBase)
            {
                var item = model as ContentItemBase;

                var tile = new SecondaryTile();
                tile.TileId = Platform.Current.GenerateModelTileID(item);
                tile.Arguments = Platform.Current.GenerateModelArguments(model);
                tile.DisplayName = item.Title;
                var status = await this.CreateOrUpdateSecondaryTileAsync(tile, new TileVisualOptions());

                if (status)
                {
                    #region Create ItemModel TileContent

                    // Construct the tile content
                    TileContent content = new TileContent()
                    {
                        Visual = new TileVisual()
                        {
                            TileMedium = new TileBinding()
                            {
                                Content = new TileBindingContentAdaptive()
                                {
                                    BackgroundImage = new TileBackgroundImage() { Source = item.ImageThumbLandscapeSmall },
                                    Children =
                                    {
                                        new AdaptiveText(){ Text = item.Title },
                                        new AdaptiveText(){ Text = item.Description, HintStyle = AdaptiveTextStyle.CaptionSubtle }
                                    }
                                }
                            },

                            TileWide = new TileBinding()
                            {
                                Content = new TileBindingContentAdaptive()
                                {
                                    BackgroundImage = new TileBackgroundImage() { Source = item.ImageThumbLandscapeSmall },
                                    Children =
                                    {
                                        new AdaptiveText(){ Text = item.Title, HintStyle = AdaptiveTextStyle.Subtitle },
                                        new AdaptiveText(){ Text = item.Description, HintStyle = AdaptiveTextStyle.CaptionSubtle }
                                    }
                                }
                            },

                            TileLarge = new TileBinding()
                            {
                                Content = new TileBindingContentAdaptive()
                                {
                                    BackgroundImage = new TileBackgroundImage() { Source = item.ImageThumbLandscapeSmall },
                                    Children =
                                    {
                                        new AdaptiveText(){ Text = item.Title, HintStyle = AdaptiveTextStyle.Subtitle },
                                        new AdaptiveText(){ Text = item.Description, HintStyle = AdaptiveTextStyle.CaptionSubtle }
                                    }
                                }
                            }
                        }
                    };

                    #endregion

                    var notification = new TileNotification(content.GetXml());
                    notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId).Update(notification);
                }

                return status;
            }

            return false;
        }

        /// <summary>
        /// Displays a toasts associated to the tile specified. How it determines what should be displayed is determined by the class implementing this
        /// interface. If the platform does not support toasts, then the implementation should do nothing.
        /// </summary>
        /// <param name="model">Model which contains the data needed for the toast.</param>
        public void DisplayToast(IModel model)
        {
            ToastContent tc = null;

            // Do custom toast notifications based on the type of the model passed into this method.
            if (model is ContentItemBase)
            {
                var item = model as ContentItemBase;
                tc = new ToastContent()
                {
                    Visual = new ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                        {
                            new AdaptiveText() {Text = item.Title },
                            new AdaptiveText() {Text = item.Description}
                        }
                        }
                    },
                    ActivationType = ToastActivationType.Foreground,
                    Scenario = ToastScenario.Default,
                    Launch = Platform.Current.GenerateModelArguments(model),
                    Duration = ToastDuration.Short
                };
            }

            if (tc != null)
            {
                ToastNotification toast = new ToastNotification(tc.GetXml());
                toast.ExpirationTime = DateTime.Now.AddHours(1);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
        }

        #endregion
    }
}
