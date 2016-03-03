using MediaAppSample.Core;
using MediaAppSample.Core.Models;
using System;
using System.Threading;
using Windows.ApplicationModel.Background;

namespace MediaAppSample.BackgroundTasks
{
    public sealed class TimedWorkerTask : IBackgroundTask
    {
        private BackgroundTaskRunInfo _info = new BackgroundTaskRunInfo();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
#if !DEBUG
            // Check if the app is alread in the foreground and if so, don't run the agent
            if (AgentSync.IsApplicationLaunched())
                return;
#endif
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            if (Platform.Current == null)
            {
                Platform.Current = new Platform();
                await Platform.Current.AppInitializingAsync(InitializationModes.Background);
                Platform.Current.Logger.Log(LogLevels.Information, "Starting background task '{0}'...", taskInstance.Task.Name);
            }

            CancellationTokenSource cts = new CancellationTokenSource();

            taskInstance.Canceled += (sender, reason) =>
            {
                Platform.Current.Logger.Log(LogLevels.Warning, "Background task '{0}' is being cancelled due to '{1}'...", taskInstance.Task.Name, reason);
                _info.CancelReason = reason.ToString();
                _info.EndTime = DateTime.UtcNow;
                cts.Cancel();
                cts.Dispose();
            };

            try
            {
                _info.StartTime = DateTime.UtcNow;
                await Platform.Current.TimedBackgroundWorkAsync(BackgroundWorkCost.CurrentBackgroundWorkCost, cts.Token);
                _info.RunSuccessfully = true;
                Platform.Current.Logger.Log(LogLevels.Information, "Completed execution of background task '{0}'!", taskInstance.Task.Name);
            }
            catch(OperationCanceledException)
            {
                Platform.Current.Logger.Log(LogLevels.Warning, "Background task '{0}' had an OperationCanceledException with reason '{1}'.", taskInstance.Task.Name, _info.CancelReason);
            }
            catch (Exception ex)
            {
                _info.ExceptionDetails = ex.ToString();
                Platform.Current.Logger.LogErrorFatal(ex, "Background task '{0}' failed with exception to run to completion: {1}", taskInstance.Task.Name, ex.Message);
            }
            finally
            {
                _info.EndTime = DateTime.UtcNow;
                Platform.Current.Storage.SaveSetting("TASK_" + taskInstance.Task.Name, _info, Windows.Storage.ApplicationData.Current.LocalSettings);
                Platform.Current.AppSuspending();
                deferral.Complete();
            }
        }
    }
}
