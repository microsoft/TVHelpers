namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Container class for local application settings.  Create all your local app setting properties here.
    /// </summary>
    public sealed class AppSettingsLocal : ModelBase
    {
        #region Properties

        private LocationModel _LocationLastKnown;
        /// <summary>
        /// Gets or sets the last known location for the user.
        /// </summary>
        public LocationModel LocationLastKnown
        {
            get { return _LocationLastKnown; }
            set { this.SetProperty(ref _LocationLastKnown, value); }
        }

        #endregion
    }

    /// <summary>
    /// Container class for roaming application settings.  Create all your roaming app setting properties here.
    /// </summary>
    public sealed class AppSettingsRoaming : ModelBase
    {
        #region Properties

        private int _ApplicationTheme = 0;
        /// <summary>
        /// Gets or sets the application theme desired by the user.
        /// </summary>
        public int ApplicationTheme
        {
            get { return _ApplicationTheme; }
            set { this.SetProperty(ref _ApplicationTheme, value); }
        }

        #endregion
    }
}