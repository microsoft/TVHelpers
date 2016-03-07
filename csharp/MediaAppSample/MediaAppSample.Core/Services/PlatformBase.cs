using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace MediaAppSample.Core.Services
{
    /// <summary>
    /// Base class used to manage application executing and to access all platform adapters available to the solution.
    /// </summary>
    public abstract partial class PlatformBase : ModelBase
    {
        #region Variables

        private Dictionary<Type, ServiceBase> _services = new Dictionary<Type, ServiceBase>();
        private bool _settingsIsLocalDataDirty = false;
        private bool _settingsIsRoamingDataDirty = false;

        #endregion Variables

        #region Properties

        /// <summary>
        /// Gets the current device family this app is executing on.
        /// </summary>
        public static DeviceFamily DeviceFamily { get; private set; }

        /// <summary>
        /// Gets whether or not the current device is Windows Desktop.
        /// </summary>
        public bool IsDesktop { get { return DeviceFamily == DeviceFamily.Desktop; } }

        /// <summary>
        /// Gets whether or not the current device is Windows Mobile.
        /// </summary>
        public bool IsMobile { get { return DeviceFamily == DeviceFamily.Mobile; } }

        /// <summary>
        /// Gets whether or not the current device is executing on Windows Mobile Continuum Desktop.
        /// </summary>
        public bool IsMobileContinuumDesktop { get { return this.IsMobile && UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse; } }

        /// <summary>
        /// Indicates the initialization mode of this app instance.
        /// </summary>
        public InitializationModes InitializationMode { get; private set; }

        private AppSettingsLocal _AppSettingsLocal;
        /// <summary>
        /// Gets local app settings for this app.
        /// </summary>
        public AppSettingsLocal AppSettingsLocal
        {
            get { return _AppSettingsLocal; }
            private set { this.SetProperty(ref _AppSettingsLocal, value); }
        }

        private AppSettingsRoaming _AppSettingsRoaming;
        /// <summary>
        /// Gets roaming app settings for this app.
        /// </summary>
        public AppSettingsRoaming AppSettingsRoaming
        {
            get { return _AppSettingsRoaming; }
            private set { this.SetProperty(ref _AppSettingsRoaming, value); }
        }
        
        #endregion Properties

        #region Constructors

        static PlatformBase()
        {
            Debug.WriteLine("DeviceFamily: " + AnalyticsInfo.VersionInfo.DeviceFamily);

            // Determine what device this is running on and store the appropriate enumeration representing that device
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop":
                    DeviceFamily = DeviceFamily.Desktop;
                    break;
                case "Windows.Mobile":
                    DeviceFamily = DeviceFamily.Mobile;
                    break;
                case "Windows.Xbox":
                    DeviceFamily = DeviceFamily.Xbox;
                    break;
                case "Windows.IoT":
                    DeviceFamily = DeviceFamily.IoT;
                    break;
                default:
                    DeviceFamily = DeviceFamily.Unknown;
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Global initialization of the app and loads all app settings and initializes all services.
        /// </summary>
        /// <param name="mode">Specifies the mode of this app instance and how it's executing.</param>
        /// <returns>Awaitable task is returned.</returns>
        public virtual Task AppInitializingAsync(InitializationModes mode)
        {
            this.InitializationMode = mode;
            this.Logger.Log(LogLevels.Warning, "APP INITIALIZING - Initialization mode is {0}", this.InitializationMode);

            this.AppSettingsLocal = this.Storage.LoadSetting<AppSettingsLocal>("AppSettingsLocal", ApplicationData.Current.LocalSettings) ?? new AppSettingsLocal();
            this.AppSettingsLocal.PropertyChanged += AppSettingsLocal_PropertyChanged;
            this.AppSettingsRoaming = this.Storage.LoadSetting<AppSettingsRoaming>("AppSettingsRoaming", ApplicationData.Current.RoamingSettings) ?? new AppSettingsRoaming();
            this.AppSettingsRoaming.PropertyChanged += AppSettingsRoaming_PropertyChanged;

            // Initializes all service
            foreach (var service in _services)
                this.CheckInitialization(service.Value);

            // Execute only on first runs of the platform
            if (mode == InitializationModes.New)
            {
                // Provide platform languages to analytics
                this.Analytics.Event("CurrentCulture", System.Globalization.CultureInfo.CurrentCulture.Name);
                this.Analytics.Event("CurrentUICulture", System.Globalization.CultureInfo.CurrentUICulture.Name);
            }

            // Register all background agents
            if (mode != InitializationModes.Background)
            {
                var btm = Platform.Current.BackgroundTasks.RegisterAllAsync();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Global suspension of the app and any custom logic to execute on suspend of the app.
        /// </summary>
        public virtual void AppSuspending()
        {
            Platform.Current.Logger.Log(LogLevels.Warning, "APP SUSPENDING - Initialization mode was {0}", this.InitializationMode);

            // Save app settings
            this.SaveSettings();

            if (this.AppSettingsLocal != null)
                this.AppSettingsLocal.PropertyChanged -= AppSettingsLocal_PropertyChanged;
            if (this.AppSettingsRoaming != null)
                this.AppSettingsRoaming.PropertyChanged -= AppSettingsRoaming_PropertyChanged;
        }

        /// <summary>
        /// Saves any app settings only if the data had changed.
        /// </summary>
        public void SaveSettings()
        {
            if (_settingsIsLocalDataDirty)
            {
                this.Storage.SaveSetting("AppSettingsLocal", this.AppSettingsLocal, ApplicationData.Current.LocalSettings);
                _settingsIsLocalDataDirty = false;
            }

            if (_settingsIsRoamingDataDirty)
            {
                this.Storage.SaveSetting("AppSettingsRoaming", this.AppSettingsRoaming, ApplicationData.Current.RoamingSettings);
                _settingsIsRoamingDataDirty = false;
            }
        }

        /// <summary>
        /// Reset all the app settings back to their defaults.
        /// </summary>
        protected void ResetAppSettings()
        {
            if (this.AppSettingsLocal != null)
                this.AppSettingsLocal.PropertyChanged -= AppSettingsLocal_PropertyChanged;
            if (this.AppSettingsRoaming != null)
                this.AppSettingsRoaming.PropertyChanged -= AppSettingsRoaming_PropertyChanged;

            _settingsIsLocalDataDirty = true;
            _settingsIsRoamingDataDirty = true;

            this.AppSettingsLocal = new AppSettingsLocal();
            this.AppSettingsLocal.PropertyChanged += AppSettingsLocal_PropertyChanged;
            this.AppSettingsRoaming = new AppSettingsRoaming();
            this.AppSettingsRoaming.PropertyChanged += AppSettingsRoaming_PropertyChanged;

            this.SaveSettings();
        }

        /// <summary>
        /// Signs a user out of the app.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        internal virtual async Task SignoutAll()
        {
            var services = _services.Values.Where(w => w is IServiceSignout);
            var list = new List<Task>();

            foreach (var service in services)
                list.Add(((IServiceSignout)service).SignoutAsync());

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    Task.WaitAll(list.ToArray());
                }
                catch (Exception ex)
                {
                    Platform.Current.Logger.LogError(ex, "Error while trying to call SignoutAsync on each of the platform services.");
                    throw;
                }
            });
        }

        /// <summary>
        /// Retrieve an instance of a type registered as a platform service.
        /// </summary>
        /// <typeparam name="T">Type reference of the service to retrieve.</typeparam>
        /// <returns>Instance of type T if it was already initialized or null if not found.</returns>
        private T GetService<T>() where T : ServiceBase
        {
            if (_services.ContainsKey(typeof(T)))
                return (T)_services[typeof(T)];
            else
                return default(T);
        }

        /// <summary>
        /// Registers and intializes an instance of an adapter.
        /// </summary>
        /// <typeparam name="T">Type reference of the service to register and initialize.</typeparam>
        private void SetService<T>(T instance) where T : ServiceBase
        {
            // Check if T is already registered
            if (_services.ContainsKey(typeof(T)))
            {
                // Shutdown the old instance of T
                var service = _services[typeof(T)];
                _services.Remove(typeof(T));
            }

            _services.Add(typeof(T), instance);
        }

        /// <summary>
        /// Initializes a service if not already initialized.
        /// </summary>
        /// <param name="service">Service instance to intialize.</param>
        private void CheckInitialization(ServiceBase service)
        {
            if (service != null && service.Initialized == false)
                service.Initialize();
        }

        #endregion Methods

        #region Event Handlers

        private void AppSettingsLocal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _settingsIsLocalDataDirty = true;
        }

        private void AppSettingsRoaming_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _settingsIsRoamingDataDirty = true;
        }

        #endregion Events
    }
}