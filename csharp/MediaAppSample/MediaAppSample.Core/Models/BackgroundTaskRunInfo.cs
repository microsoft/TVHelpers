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
