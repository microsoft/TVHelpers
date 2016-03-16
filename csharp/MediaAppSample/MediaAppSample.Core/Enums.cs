namespace MediaAppSample.Core
{
    #region Enumerations

    /// <summary>
    /// Enumeration used to indicate if core code is executing in a new instance of the application, 
    /// if it were resumed, or if executing in the background.
    /// </summary>
    public enum InitializationModes
    {
        /// <summary>
        /// New instance of the application launched.
        /// </summary>
        New,

        /// <summary>
        /// App restored from a suspended state.
        /// </summary>
        Restore,

        /// <summary>
        /// App background task launched.
        /// </summary>
        Background
    }

    /// <summary>
    /// Device families supported by Windows.
    /// </summary>
    public enum DeviceFamily
    {
        Unknown,
        Xbox,
        Desktop,
        Mobile,
        IoT,
    }

    /// <summary>
    /// Enumeration representing each way maps can be displayed.
    /// </summary>
    public enum MapExternalOptions
    {
        /// <summary>
        /// Standard directions
        /// </summary>
        Normal,

        /// <summary>
        /// Directions for driving
        /// </summary>
        DrivingDirections,

        /// <summary>
        /// Directions for walking
        /// </summary>
        WalkingDirections
    }

    #endregion
}
