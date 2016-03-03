using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the geocoding service adapter implement of the platform currently executing.
        /// </summary>
        public BackgroundTasksManager BackgroundTasksManager
        {
            get { return this.GetAdapter<BackgroundTasksManager>(); }
            protected set { this.Register<BackgroundTasksManager>(value); }
        }
    }

    /// <summary>
    /// Task manager is responsible for registering and unregistering all background tasks used by this application.
    /// </summary>
    public sealed class BackgroundTasksManager : ServiceBase, IServiceSignout
    {
        #region Properties

        public bool AreTasksRegistered { get; set; }

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
                    this.RegisterBackgroundTaskAsync("MediaAppSample.BackgroundTasks.TimedWorkerTask", "MediaAppSampleTimeTriggerTask", new TimeTrigger(15, false), null);

                    // Flag that registration was completed
                    this.AreTasksRegistered = true;
                }
                catch(Exception ex)
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
