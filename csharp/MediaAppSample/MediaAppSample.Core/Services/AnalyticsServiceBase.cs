using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the analytics service of the platform currently executing.
        /// </summary>
        public AnalyticsServiceBase Analytics
        {
            get { return this.GetAdapter<AnalyticsServiceBase>(); }
            set { this.Register<AnalyticsServiceBase>(value); }
        }
    }

    /// <summary>
    /// Base class providing access to the analytics service for the platform currently executing.
    /// </summary>
    public abstract class AnalyticsServiceBase : ServiceBase
    {
        #region Methods

        [Conditional("RELEASE")]
        public abstract void NewPageView(Type pageType);

        /// <summary>
        /// Logs an event to the analytics service.
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="value">Value to store</param>
        [Conditional("RELEASE")]
        public void Event(string eventName, object value)
        {
            Dictionary<string, string> metrics = null;
            if (value != null)
            {
                metrics = new Dictionary<string, string>();
                metrics.Add(eventName, value.ToString());
            }
            this.Event(eventName, metrics);
        }

        /// <summary>
        /// Logs an event to the analytics service.
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        /// <param name="pairs">Key/Value dictionary of parameters to log to the event name specified</param>
        [Conditional("RELEASE")]
        public abstract void Event(string eventName, Dictionary<string, string> metrics = null);

        /// <summary>
        /// Logs an error to the analytics service.
        /// </summary>
        /// <param name="message">Friendly message describing the exception or where this might have originated from</param>
        /// <param name="ex">The exception object</param>
        [Conditional("RELEASE")]
        public abstract void Error(Exception ex, string message = null);

        [Conditional("RELEASE")]
        public abstract void SetUsername(string username);

        /// <summary>
        /// Sets the current location to the analytics service.
        /// </summary>
        /// <param name="loc">Location value to log</param>
        [Conditional("RELEASE")]
        public virtual void SetCurrentLocation(ILocationModel loc)
        {
            if (loc != null)
            {
                var metrics = new Dictionary<string, string>();
                metrics.Add("LocationDisplayName", loc.LocationDisplayName);
                metrics.Add("Latitude", loc.Latitude.ToString());
                metrics.Add("Longitude", loc.Longitude.ToString());

                this.Event("CurrentLocation", metrics);
            }
        }

        #endregion Methods
    }

    #region Classes

    /// <summary>
    /// If no analytics service was specified, this dummy class will be used which implements AnalyticsProviderBase but does not do anything.
    /// Used to prevent null value exceptions when any code tries to log to the analytics adapter.
    /// </summary>
    internal class DefaultAnalyticsProvider : AnalyticsServiceBase
    {
        public override void NewPageView(Type pageType)
        {
        }

        public override void Error(Exception ex, string message = null)
        {
        }

        public override void Event(string eventName, Dictionary<string, string> metrics = null)
        {
        }

        public override void SetUsername(string username)
        {
        }
    }

    #endregion Classes
}