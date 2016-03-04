using MediaAppSample.Core.Models;
using MediaAppSample.Core.ViewModels;
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

            // Do custom tile creation based on the type of the model passed into this method.
            if (model is MainViewModel || model is ModelList<ContentItemBase>)
            {
                var list = (model as MainViewModel)?.Items ?? model as ModelList<ContentItemBase>;
                
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

                if(status)
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
                                    Children =
                                    {
                                        new TileText(){ Text = item.Title },
                                        new TileText(){ Text = item.Description, Style = TileTextStyle.CaptionSubtle }
                                    }
                                }
                            },

                            TileWide = new TileBinding()
                            {
                                Content = new TileBindingContentAdaptive()
                                {
                                    Children =
                                    {
                                        new TileText(){ Text = item.Title, Style = TileTextStyle.Subtitle },
                                        new TileText(){ Text = item.Description, Style = TileTextStyle.CaptionSubtle }
                                    }
                                }
                            },

                            TileLarge = new TileBinding()
                            {
                                Content = new TileBindingContentAdaptive()
                                {
                                    Children =
                                    {
                                        new TileText(){ Text = item.Title, Style = TileTextStyle.Subtitle },
                                        new TileText(){ Text = item.Description, Style = TileTextStyle.CaptionSubtle }
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
            if(model is ContentItemBase)
            {
                var item = model as ContentItemBase;
                tc = new ToastContent()
                {
                    Visual = new ToastVisual()
                    {
                        TitleText = new ToastText() { Text = item.Title },
                        BodyTextLine1 = new ToastText() { Text = item.Description }
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
