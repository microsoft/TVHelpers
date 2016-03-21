using MediaAppSample.Core.Models;
using MediaAppSample.Core.Services;
using MediaAppSample.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace MediaAppSample.Core
{
    /// <summary>
    /// Singleton object which holds instances to all the services in this application.
    /// Also provides core app functionality for initializing and suspending your application,
    /// handling exceptions, and more.
    /// </summary>
    public sealed class Platform : PlatformBase
    {
        #region Properties

        /// <summary>
        /// Provides access to application services.
        /// </summary>
        public static Platform Current { get; private set; }

        private MainViewModel _ViewModel;
        /// <summary>
        /// Gets the MainViewModel global instance for the application.
        /// </summary>
        public MainViewModel ViewModel
        {
            get { return _ViewModel; }
            private set { this.SetProperty(ref _ViewModel, value); }
        }

        #endregion

        #region Constructors

        static Platform()
        {
            Current = new Platform();
        }

        private Platform()
        {
            // Instantiate all the application services.
            this.Logger = new LoggingService();
            this.Analytics = new DefaultAnalyticsProvider();
            this.BackgroundTasks = new BackgroundTasksManager();
            this.Storage = new StorageManager();
            this.AppInfo = new AppInfoProvider();
            this.AuthManager = new AuthorizationManager();
            this.Cryptography = new CryptographyProvider();
            this.Notifications = new NotificationsService();
            this.Ratings = new RatingsManager();
            this.VoiceCommandManager = new VoiceCommandManager();
            this.Jumplist = new JumplistManager();
            this.WebAccountManager = new WebAccountManager();
            this.SharingManager = new SharingManager();
        }

        #endregion

        #region Methods

        #region Application Core

        /// <summary>
        /// Logic performed during initialization of the application.
        /// </summary>
        /// <param name="mode">Mode indicates how this app instance is being run.</param>
        /// <returns>Awaitable task is returned.</returns>
        public override async Task AppInitializingAsync(InitializationModes mode)
        {
            // Call to base.AppInitializing is required to be executed first so all adapters and the framework are properly initialized
            await base.AppInitializingAsync(mode);

            // Your custom app logic which you want to always run at start of
            // your app should be placed here.

            if (this.ViewModel == null)
                this.ViewModel = new MainViewModel();

            if (mode == InitializationModes.New)
            {
                // Check for previous app crashes
                await Platform.Current.Logger.CheckForFatalErrorReportsAsync(this.ViewModel);

                // Check to see if the user should be prompted to rate the application
                await Platform.Current.Ratings.CheckForRatingsPromptAsync(this.ViewModel);
            }
        }

        /// <summary>
        /// Logic performed during suspend of the application.
        /// </summary>
        public override void AppSuspending()
        {
            // Your custom app logic which you want to always run at suspend of
            // your app should be placed here.

            // Call to base.AppSuspending is required to be executed last so all adapters and the framework are properly shutdown or saved
            base.AppSuspending();
        }

        /// <summary>
        /// Global unhandled exception handler for your application.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>True if the exception was handled else false.</returns>
        public bool AppUnhandledException(Exception e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // If the Native debugger is in use, give us a clue in the Output window at least
                System.Diagnostics.Debug.WriteLine("Unhandled exception:" + e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);

                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            // Only log this when the debugger is not attached and you're in RELEASE mode
            try
            {
                Platform.Current.Analytics.Error(e, "Unhandled Exception");
            }
            catch { }

            try
            {
                Platform.Current.Logger.LogErrorFatal(e);
            }
            catch (Exception exLog)
            {
                System.Diagnostics.Debug.WriteLine("Exception logging to Logger in AppUnhandledException!");
                System.Diagnostics.Debug.WriteLine(exLog.ToString());
            }

            return false;
        }

        /// <summary>
        /// Logic performed during sign out of a user in this application.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        internal override async Task SignoutAll()
        {
            await base.SignoutAll();

            // Instantiate a new instance of settings and the MainViewModel 
            // to ensure no previous user data is shown on the UI.
            this.ResetAppSettings();
            this.ViewModel = new MainViewModel();
        }

        #endregion

        #region Background Tasks

        /// <summary>
        /// Work that should be performed from the background agent.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public Task TimedBackgroundWorkAsync(BackgroundWorkCostValue cost, CancellationToken ct)
        {
            try
            {
                return Task.CompletedTask;

                //// Perform work that needs to be done on a background task/agent...
                //if (Platform.Current.AuthManager.IsAuthenticated() == false)
                //    return Task.CompletedTask;

                //// SAMPLE - Load data from your API, do any background work here.
                //var data = await DataSource.Current.GetItems(ct);
                //if (data != null)
                //{
                //    var items = data.ToObservableCollection();
                //    if (items.Count > 0)
                //    {
                //        var index = DateTime.Now.Second % items.Count;
                //        Platform.Current.Notifications.DisplayToast(items[index]);
                //    }
                //}

                //ct.ThrowIfCancellationRequested();

                //if (cost <= BackgroundWorkCostValue.Medium)
                //{
                //    // Update primary tile
                //    await Platform.Current.Notifications.CreateOrUpdateTileAsync(new ModelList<ItemModel>(data));

                //    ct.ThrowIfCancellationRequested();

                //    // Update all tiles pinned from this application
                //    await Platform.Current.Notifications.UpdateAllSecondaryTilesAsync(ct);
                //}
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogErrorFatal(ex, "Failed to complete BackgroundWork from background task due to: {0}", ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Generate Models

        /// <summary>
        /// Creates a querystring parameter string from a model instance.
        /// </summary>
        /// <param name="model">Model to convert into a querystring.</param>
        /// <returns>Query string representing the model provided.</returns>
        public string GenerateModelArguments(IModel model)
        {
            if (model == null)
                return null;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("model", model.GetType().Name);

            // For each model you want to support, you'll add any custom properties 
            // to the dictionary based on the type of object
            if (model is ContentItemBase)
            {
                var item = model as ContentItemBase;
                dic.Add("ID", item.ID.ToString());
            }
            else
            {
                return null;
            }

            // Create a querystring from the dictionary collection
            return GeneralFunctions.CreateQuerystring(dic);
        }

        /// <summary>
        /// Generates a unique tile ID used for secondary tiles based on a model instance.
        /// </summary>
        /// <param name="model">Model to convert into a unique tile ID.</param>
        /// <returns>String representing a unique tile ID for the model else null if not supported.</returns>
        public string GenerateModelTileID(IModel model)
        {
            // For each model you want to support, you'll want to customize the ID that gets generated to be unique.
            if (model is MainViewModel)
            {
                return string.Empty;
            }
            else if (model is ContentItemBase)
            {
                var item = model as ContentItemBase;
                return "ContentItemBase_" + item.ID;
            }
            else
                return null;
        }

        /// <summary>
        /// Converts a tile ID back into an object instance.
        /// </summary>
        /// <param name="tileID">Tile ID to retrieve an object instance for.</param>
        /// <param name="ct">Cancelation token.</param>
        /// <returns>Object instance if found else null.</returns>
        public Task<IModel> GenerateModelFromTileIdAsync(string tileID, CancellationToken ct)
        {
            try
            {
                //if (tileID.StartsWith("ItemModel_"))
                //{
                //    int id = int.Parse(tileID.Split('_').Last());
                //    using (var api = new ClientApi())
                //    {
                //        return await api.GetItemByID(id, ct);
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Couldn't generate model from tileID = {0}", tileID), ex);
            }

            throw new NotImplementedException(string.Format("App has not implemented creating a model from tileID = {0}", tileID));
        }

        #endregion

        #endregion
    }
}
