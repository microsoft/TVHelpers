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

using MediaAppSample.Core.Commands;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.System;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the geocoding service adapter implement of the platform currently executing.
        /// </summary>
        public BackgroundTasksManager BackgroundTasks
        {
            get { return this.GetService<BackgroundTasksManager>(); }
            protected set { this.SetService<BackgroundTasksManager>(value); }
        }
    }

    /// <summary>
    /// Task manager is responsible for registering and unregistering all background tasks used by this application.
    /// </summary>
    public sealed class BackgroundTasksManager : ServiceBase, IServiceSignout
    {
        #region Properties

        public bool AreTasksRegistered { get; set; }
        
        private CommandBase _ManageBackgroundTasksCommand = null;
        /// <summary>
        /// Manage background apps from the Windows Settings app.
        /// </summary>
        public CommandBase ManageBackgroundTasksCommand
        {
            // Deep linking to Settings app sections: https://msdn.microsoft.com/en-us/library/windows/apps/mt228342.aspx
            get { return _ManageBackgroundTasksCommand ?? (_ManageBackgroundTasksCommand = new GenericCommand("ManageBackgroundTasksCommand", async () => await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-backgroundapps")))); }
        }

        #endregion

        #region Constructors

        internal BackgroundTasksManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers all background tasks related to this application.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public async Task RegisterAllAsync()
        {
            try
            {
                // Keep track of the previous version of the app. If the app has been updated, we must first remove the previous task registrations and then re-add them.
                var previousVersion = Platform.Current.Storage.LoadSetting<string>("PreviousAppVersion");
                if (previousVersion != Platform.Current.AppInfo.VersionNumber.ToString())
                {
                    this.RemoveAll();
                    Platform.Current.Storage.SaveSetting("PreviousAppVersion", Platform.Current.AppInfo.VersionNumber.ToString());
                }

                // Propmts users to give access to run background tasks.
                var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity || backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
                    try
                    {
                        // Register each of your background tasks here:
                        RegisterBackgroundTaskAsync("MediaAppSample.BackgroundTasks.TimedWorkerTask", "MediaAppSampleTimeTriggerTask", new TimeTrigger(15, false), null);

                        // Flag that registration was completed
                        this.AreTasksRegistered = true;
                    }
                    catch (Exception ex)
                    {
                        Platform.Current.Logger.LogError(ex, "Failed to register background tasks.");
                    }
                }
                else
                {
                    // User did not give the app access to run background tasks
                    Platform.Current.Logger.Log(LogLevels.Information, "Could not register tasks because background access status is '{0}'.", backgroundAccessStatus);
                    this.AreTasksRegistered = false;
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during BackgroundTaskManager.RegisterAllAsync()");
            }
        }

        /// <summary>
        /// Indicates whether or not the app has permissions to run background tasks.
        /// </summary>
        /// <returns>True if allowed else false.</returns>
        public bool CheckIfAllowed()
        {
            var status = BackgroundExecutionManager.GetAccessStatus();

            var allowed = status == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity 
                || status == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity;
            
            if (allowed == false)
                this.AreTasksRegistered = false;

            return allowed;
        }

        /// <summary>
        /// Removes background task registrations when the user signs out of the app.
        /// </summary>
        /// <returns>Awaitable task is returned.</returns>
        public Task SignoutAsync()
        {
            this.RemoveAll();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes all registered background tasks related to this application.
        /// </summary>
        private void RemoveAll()
        {
            this.Remove(null);
            BackgroundExecutionManager.RemoveAccess();
        }

        /// <summary>
        /// Register a background task with the specified taskEntryPoint, name, trigger,
        /// and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        private BackgroundTaskRegistration RegisterBackgroundTaskAsync(string taskEntryPoint, string taskName, IBackgroundTrigger trigger, IBackgroundCondition condition = null)
        {
            if (string.IsNullOrWhiteSpace(taskEntryPoint))
                throw new ArgumentNullException(nameof(taskEntryPoint));
            if (string.IsNullOrWhiteSpace(taskName))
                throw new ArgumentNullException(nameof(taskName));
            if (trigger == null)
                throw new ArgumentNullException(nameof(trigger));

            try
            {
                // Remove if existing
                this.Remove(taskName);

                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();

                if (condition != null)
                {
                    builder.AddCondition(condition);

                    // If the condition changes while the background task is executing then it will
                    // be canceled.
                    builder.CancelOnConditionLoss = true;
                }

                builder.Name = taskName;
                builder.TaskEntryPoint = taskEntryPoint;

                builder.SetTrigger(trigger);

                var registrationTask = builder.Register();
                return registrationTask;
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while trying to register task '{0}': {1}", taskName, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>
        private void Remove(string name)
        {
            // Loop through all background tasks and unregister all tasks with matching name
            foreach (var taskKeyPair in BackgroundTaskRegistration.AllTasks)
            {
                if (string.IsNullOrEmpty(name) || taskKeyPair.Value.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    taskKeyPair.Value.Unregister(true);
                    Platform.Current.Storage.SaveSetting("TASK_" + taskKeyPair.Key, null, Windows.Storage.ApplicationData.Current.LocalSettings);
                    Platform.Current.Logger.Log(LogLevels.Debug, "TaskManager removed background task '{0}'", name);
                }
            }
        }

        #endregion
    }
}
