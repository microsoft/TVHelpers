using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.System;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the location service of the platform currently executing.
        /// </summary>
        public GeolocationService Geolocation
        {
            get { return this.GetService<GeolocationService>(); }
            protected set { this.SetService<GeolocationService>(value); }
        }
    }

    public sealed class GeolocationService : ServiceBase
    {
        #region Variables

        private Geolocator _geolocator = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the status of the location service of the platform currently executing.
        /// </summary>
        public PositionStatus Status { get; private set; }

        public delegate void LocationChangedEventHandler(object sender, LocationChangedEventArgs e);

        /// <summary>
        /// Event which notifies you of when a location changed was detected.
        /// </summary>
        public event LocationChangedEventHandler LocationChanged;

        private LocationModel _currentLocation = null;
        /// <summary>
        /// Gets or sets the last known location found. The location will be stores by the storage manager so that the susquent load of the platform has a location immediately
        /// available to use if needed.
        /// </summary>
        public LocationModel CurrentLocation
        {
            get
            {
                // Return the last saved location if no location found.
                if (_currentLocation == null)
                    this.CurrentLocation = Platform.Current.AppSettingsLocal.LocationLastKnown;
                
                return _currentLocation;
            }
            private set
            {
                if (value == null)
                    return;

                if (_currentLocation == null)
                {
                    // Store the location and wire up events
                    _currentLocation = value;
                    _currentLocation.PropertyChanged += CurrentLocationInfo_PropertyChanged;
                    this.NotifyPropertyChanged(() => this.CurrentLocation);
                    this.NotifyLocationChangedEvent();
                    Platform.Current.AppSettingsLocal.LocationLastKnown = value;
                }
                else
                {
                    // Update the current location with the new values
                    if (_currentLocation.SetLocation(value))
                        Platform.Current.AppSettingsLocal.LocationLastKnown = value;
                }
            }
        }

        private CommandBase _ManageLocationServicesCommand = null;
        /// <summary>
        /// Manage location settings in the Windows Settings app.
        /// </summary>
        public CommandBase ManageLocationServicesCommand
        {
            // Deep linking to Settings app sections: https://msdn.microsoft.com/en-us/library/windows/apps/mt228342.aspx
            get { return _ManageLocationServicesCommand ?? (_ManageLocationServicesCommand = new GenericCommand("ManageLocationServicesCommand", async () => await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location")))); }
        }

        #endregion Properties

        #region Constructors

        internal GeolocationService()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the current location for the user.
        /// </summary>
        /// <param name="highAccuracy">True if the system should find a highly accurate location or false if a quicker location should be returned. High accuracy takes more
        /// time to determine the user's location.</param>
        /// <returns>Location information representing the current location of the user.</returns>
        public async Task<Geoposition> GetSingleCoordinateAsync(bool highAccuracy = false, double movementThreshold = 0, CancellationToken? token = null)
        {
            // Request permission to access location
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch(accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    try
                    {
                        Platform.Current.Logger.Log(LogLevels.Debug, "GetSingleCoordinate Started...");
                        Geolocator geo = new Geolocator();

                        geo.DesiredAccuracy = highAccuracy ? PositionAccuracy.High : PositionAccuracy.Default;
                        if (movementThreshold > 0)
                            geo.MovementThreshold = movementThreshold;

                        // Retrieve the current user's location
                        Geoposition loc = await geo.GetGeopositionAsync(new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 60)).AsTask(token.HasValue ? token.Value : CancellationToken.None);
                        Platform.Current.Logger.Log(LogLevels.Debug, "GetSingleCoordinate Completed!");

                        // Store location and update statuses and analytics
                        this.CurrentLocation = loc.Coordinate.AsLocationModel();
                        this.Status = geo.LocationStatus;
                        Platform.Current.Analytics.SetCurrentLocation(this.CurrentLocation);

                        // Return location found
                        return loc;
                    }
                    catch (TaskCanceledException)
                    {
                        Platform.Current.Logger.Log(LogLevels.Debug, "GetSingleCoordinate cancelled!");
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Platform.Current.Logger.Log(LogLevels.Warning, "GetSingleCoordinate exception. Exception.Message = {0}", ex.Message);
                        return null;
                    }
                    
                case GeolocationAccessStatus.Denied:
                    Platform.Current.Logger.Log(LogLevels.Warning, "GetSingleCoordinate access denied!");
                    return null;

                case GeolocationAccessStatus.Unspecified:
                    Platform.Current.Logger.Log(LogLevels.Warning, "GetSingleCoordinate unspecified error!");
                    return null;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Starts location tracking for the platform currently executing.
        /// </summary>
        /// <param name="highAccuracy">True if the system should find a highly accurate location or false if a quicker location should be returned. High accuracy takes more
        /// time to determine the user's location.</param>
        public async Task StartTrackingAsync(bool highAccuracy = false, double movementThreshold = double.MinValue, uint reportInterval = uint.MinValue)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    try
                    {
                        if (_geolocator == null)
                        {
                            Platform.Current.Logger.Log(LogLevels.Information, "StartTracking Started...");
                            _geolocator = new Geolocator();

                            // Set locator properties
                            _geolocator.DesiredAccuracy = highAccuracy ? PositionAccuracy.High : PositionAccuracy.Default;
                            if (movementThreshold > 0)
                                _geolocator.MovementThreshold = movementThreshold;
                            if(reportInterval > 0)
                                _geolocator.ReportInterval = reportInterval;

                            // Watch for location change events and update properties when updated
                            _geolocator.StatusChanged += (sender, args) =>
                            {
                                Platform.Current.Logger.Log(LogLevels.Debug, "StartTracking StatusChanged = {0}", args.Status);
                                this.Status = args.Status;
                            };
                            _geolocator.PositionChanged += (sender, args) =>
                            {
                                Platform.Current.Logger.Log(LogLevels.Debug, "StartTracking PositionChanged = {0}, {1}", args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
                                this.CurrentLocation = args.Position.Coordinate.AsLocationModel();
                                Platform.Current.Analytics.SetCurrentLocation(this.CurrentLocation);
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Platform.Current.Logger.LogError(ex, "StartTracking exception = {0}", ex.Message);
                        this.StopTracking();
                    }
                    break;

                case GeolocationAccessStatus.Denied:
                    Platform.Current.Logger.Log(LogLevels.Warning, "StartTracking access denied!");
                    this.StopTracking();
                    break;

                case GeolocationAccessStatus.Unspecified:
                    Platform.Current.Logger.Log(LogLevels.Warning, "StartTracking unspecified error!");
                    this.StopTracking();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Stops location tracking for the platform currently executing.
        /// </summary>
        public void StopTracking()
        {
            if (_geolocator != null)
                _geolocator = null;
            Platform.Current.Logger.Log(LogLevels.Information, "StopTracking Completed!");
        }

        /// <summary>
        /// Notifies subscribers that the location changed.
        /// </summary>
        private void NotifyLocationChangedEvent()
        {
            this.LocationChanged?.Invoke(this, new LocationChangedEventArgs(this.CurrentLocation));
        }

        private void CurrentLocationInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Notify that the lat or lon were updated
            if (e.PropertyName == "Latitude" || e.PropertyName == "Longitude")
                this.NotifyLocationChangedEvent();
        }

        #endregion
    }

    #region Classes

    /// <summary>
    /// Event args for when location changed event occurs.
    /// </summary>
    public sealed class LocationChangedEventArgs : EventArgs
    {
        public ILocationModel Location { get; private set; }

        public LocationChangedEventArgs(ILocationModel loc)
        {
            this.Location = loc;
        }
    }

    #endregion
}
