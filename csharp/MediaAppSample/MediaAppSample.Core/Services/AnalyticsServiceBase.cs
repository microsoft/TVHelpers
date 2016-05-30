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
            get { return this.GetService<AnalyticsServiceBase>(); }
            set { this.SetService<AnalyticsServiceBase>(value); }
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