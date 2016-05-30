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

using System;

namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Used to hold background tasks execution status information.
    /// </summary>
    public class BackgroundTaskRunInfo : ModelBase
    {
        /// <summary>
        /// Gets or sets the name of the task that was executed.
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Gets or sets the start time of the task execution.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the task execution.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets whether or not the task completed successfully or not.
        /// </summary>
        public bool RunSuccessfully { get; set; }

        /// <summary>
        /// Gets or sets the reason the task was cancelled.
        /// </summary>
        public string CancelReason { get; set; }

        /// <summary>
        /// Gets or sets the details of any exception that was thrown during the task execution.
        /// </summary>
        public string ExceptionDetails { get; set; }

        /// <summary>
        /// Gets the number of milliseconds the task took to run.
        /// </summary>
        public string TimeToRun { get { return (this.EndTime - this.StartTime).TotalMilliseconds.ToString() + " ms"; } }
    }
}
