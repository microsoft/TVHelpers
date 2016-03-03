using MediaAppSample.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MediaAppSample.Core
{
    /// <summary>
    /// Enumeration describing the level/type of information being logged
    /// </summary>
    public enum LogLevels
    {
        Debug,
        Information,
        Warning,
        Error,
        FatalError,
        Off
    }
}

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets access to the logging service of the platform currently executing.
        /// </summary>
        public LoggingService Logger
        {
            get { return this.GetAdapter<LoggingService>(); }
            protected internal set { this.Register<LoggingService>(value); }
        }
    }

    public sealed class LoggingService : ServiceBase
    {
        #region Constants 

        private const int MAX_ENTRY_COUNT = 400;
        private const string ERROR_REPORT_FILENAME = "Application.log";
        private static StorageFolder ERROR_REPORT_DATA_CONTAINER = ApplicationData.Current.LocalCacheFolder;

        #endregion

        #region Properties

        private LogLevels _CurrentLevel;
        /// <summary>
        /// Gets the current level of events for which logging is storing.
        /// </summary>
        public LogLevels CurrentLevel
        {
            get { return _CurrentLevel; }
            set { this.SetProperty(ref _CurrentLevel, value); }
        }

        private List<ILogger> Loggers { get; set; }

        private ObservableCollection<string> _Messages;
        /// <summary>
        /// Gets the list of messages that have been logged.
        /// </summary>
        public ObservableCollection<string> Messages
        {
            get { return _Messages; }
            private set { this.SetProperty(ref _Messages, value); }
        }

        #endregion

        #region Constructors

        internal LoggingService()
        {
            this.Loggers = new List<ILogger>();
            this.Messages = new ObservableCollection<string>();
#if DEBUG
            this.CurrentLevel = LogLevels.Debug;
            this.Loggers.Add(new DebugLoggerProvider());
#else
            this.CurrentLevel = LogLevels.Warning;
#endif
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="level">Level of event.</param>
        /// <param name="message">Message to log.</param>
        /// <param name="args">Arguments to string.Format into the specified message.</param>
        public void Log(LogLevels level, string message, params object[] args)
        {
            if (level < this.CurrentLevel)
                return;

            message = this.FormatString(message, args);
            message = string.Format(CultureInfo.InvariantCulture, "{0}  {1}:  {2}", DateTime.Now, level, message);

            Messages.Insert(0, message);
            if (Messages.Count > MAX_ENTRY_COUNT)
                Messages.RemoveAt(MAX_ENTRY_COUNT - 1);

            foreach (ILogger logger in Loggers)
                logger.Log(message);
        }

        /// <summary>
        /// Logs an error event.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message">Message to log.</param>
        /// <param name="args">Arguments to string.Format into the specified message.</param>
        public void LogError(Exception ex, string message = null, params object[] args)
        {
            if (LogLevels.Error < this.CurrentLevel)
                return;

            if (ex == null)
            {
                this.Log(LogLevels.Error, message, args);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(message))
                message = ex.Message;
            else if (args != null)
                message = FormatString(message, args);

            message = string.Format(CultureInfo.InvariantCulture, "{0} -- {1}", message, ex);

            Platform.Current.Analytics.Error(ex, message);
            this.Log(LogLevels.Error, message);
            foreach (ILogger logger in Loggers)
                logger.LogException(ex, message);
        }

        /// <summary>
        /// Logs a fatal event.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message">Message to log.</param>
        /// <param name="args">Arguments to string.Format into the specified message.</param>
        public void LogErrorFatal(Exception ex, string message = null, params object[] args)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));
            if (LogLevels.FatalError < this.CurrentLevel)
                return;

            if (string.IsNullOrWhiteSpace(message))
                message = ex.Message;
            else if (args != null)
                message = FormatString(message, args);

            message = string.Format(CultureInfo.InvariantCulture, "{0} -- FATAL EXCEPTION: {1}", message, ex);

            Platform.Current.Analytics.Error(ex, message);
            this.Log(LogLevels.FatalError, message);
            foreach (ILogger logger in Loggers)
                logger.LogExceptionFatal(ex, message);

            string data = this.GenerateApplicationReport();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Platform.Current.Storage.SaveFileAsync(ERROR_REPORT_FILENAME, data, ERROR_REPORT_DATA_CONTAINER);
                    tcs.SetResult(null);
                }
                catch (Exception taskEx)
                {
                    tcs.SetException(taskEx);
                }
            });

            tcs.Task.Wait();
        }

        /// <summary>
        /// Checks to see if any error logs were stored from an app crash and prompts the user to send to the developer.
        /// </summary>
        /// <param name="vm">ViewModel instance that is used to show a message box from.</param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task CheckForFatalErrorReportsAsync(ViewModelBase vm)
        {
            try
            {
                if (await Platform.Current.Storage.DoesFileExistsAsync(ERROR_REPORT_FILENAME, ERROR_REPORT_DATA_CONTAINER))
                {
                    if (await vm.ShowMessageBoxAsync(Strings.Resources.ApplicationProblemPromptMessage, Strings.Resources.ApplicationProblemPromptTitle, new string[] { Strings.Resources.TextYes, Strings.Resources.TextNo }) == 0)
                    {
                        string subject = string.Format(Strings.Resources.ApplicationProblemEmailSubjectTemplate, Windows.ApplicationModel.Package.Current.DisplayName, Platform.Current.AppInfo.VersionNumber);
                        var attachment = await Platform.Current.Storage.GetFileAsync(ERROR_REPORT_FILENAME, ERROR_REPORT_DATA_CONTAINER);

                        string body = Strings.Resources.ApplicationProblemEmailBodyTemplate;
                        body += await Platform.Current.Storage.ReadFileAsStringAsync(ERROR_REPORT_FILENAME, ERROR_REPORT_DATA_CONTAINER);

                        await Platform.Current.Navigation.SendEmailAsync(subject, body, Strings.Resources.ApplicationSupportEmailAddress, attachment);
                    }

                    await Platform.Current.Storage.DeleteFileAsync(ERROR_REPORT_FILENAME, ERROR_REPORT_DATA_CONTAINER);
                }
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while attempting to check for fatal error reports!");
            }
        }

        /// <summary>
        /// Builds an application report with system details and logged messages.
        /// </summary>
        /// <param name="ex">Exception object if available.</param>
        /// <returns>String representing the system and app logging data.</returns>
        public string GenerateApplicationReport(Exception ex = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("UTC TIME: " + DateTime.Now.ToUniversalTime().ToString());
            if (Platform.Current.AppInfo != null)
            {
                sb.AppendLine(string.Format("APP NAME: {0} {1} {2} {3}", Windows.ApplicationModel.Package.Current.DisplayName, Platform.Current.AppInfo.VersionNumber, Platform.Current.AppInfo.IsTrial ? "TRIAL" : "", Platform.Current.AppInfo.IsTrialExpired ? "EXPIRED" : "").Trim());
                if (Platform.Current.AppInfo.IsTrial && Platform.Current.AppInfo.TrialExpirationDate.Year != 9999)
                    sb.AppendLine("TRIAL EXPIRATION: " + Platform.Current.AppInfo.TrialExpirationDate);
                sb.AppendLine("INSTALLED: " + Windows.ApplicationModel.Package.Current.InstalledDate.DateTime);
            }
            sb.AppendLine("INITIALIZATION MODE: " + Platform.Current.InitializationMode);
            sb.AppendLine(string.Format("CULTURE: {0}  UI CULTURE: {1}", CultureInfo.CurrentCulture.Name, CultureInfo.CurrentUICulture.Name));
            
            sb.AppendLine(string.Format("OS: {0} {1} ({2}) Device Form: {3}", Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily, Windows.ApplicationModel.Package.Current.Id.Architecture.ToString(), Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion, Windows.System.Profile.AnalyticsInfo.DeviceForm));

            if (Window.Current != null)
            {
                sb.AppendLine("USER INTERACTION MODE: " + Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView().UserInteractionMode.ToString());

                var di = DisplayInformation.GetForCurrentView();
                sb.AppendLine(string.Format("SCREEN RAW DPI - X: {0} Y: {1}", di.RawDpiX, di.RawDpiY));
                sb.AppendLine(string.Format("SCREEN ORIENTATION - CURRENT: {0}  Native: {1}", di.CurrentOrientation, di.NativeOrientation));
                sb.AppendLine(string.Format("DEVICE PHYSICAL PIXELS: {0} x {1}", this.GetResolution(Window.Current.Bounds.Width, DisplayInformation.GetForCurrentView().ResolutionScale).ToString("0.#"), this.GetResolution(Window.Current.Bounds.Height, DisplayInformation.GetForCurrentView().ResolutionScale).ToString("0.#")));
                sb.AppendLine("SCREEN RESOLUTION SCALE: " + di.ResolutionScale);
                sb.AppendLine(string.Format("DEVICE LOGICAL PIXELS: {0} x {1}", Window.Current.Bounds.Width.ToString("0.#"), Window.Current.Bounds.Height.ToString("0.#")));
            }
            
            if (ex != null)
            {
                sb.AppendLine("");
                sb.AppendLine("EXCEPTION TYPE: " + ex.GetType().FullName);
                sb.AppendLine("EXCEPTION MESSAGE: " + ex.Message);
                sb.AppendLine("EXCEPTION STACKTRACE: " + ex.StackTrace);
                sb.AppendLine("EXCEPTION INNER: " + ex.InnerException);
            }
            
            sb.AppendLine("");
            sb.AppendLine("LOGS:");
            sb.AppendLine("------------------------");
            foreach (string msg in Messages)
                sb.AppendLine(msg);

            var data = sb.ToString();
            data = data.Replace("\\r\\n", Environment.NewLine).Replace("\r\n", Environment.NewLine);
            return data;
        }

        private double GetResolution(double pixels, ResolutionScale rs)
        {
            return pixels * ((double)rs / 100.0);
        }

        private string FormatString(string msg, object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    msg = string.Format(CultureInfo.InvariantCulture, msg, args);
                }
                catch (FormatException) { }
            }

            return msg;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Interfaced use to log data to custom sources.
        /// </summary>
        private interface ILogger
        {
            #region Methods

            void Log(string msg);

            void LogException(Exception ex, string message);

            void LogExceptionFatal(Exception ex, string message);

            #endregion
        }

        /// <summary>
        /// Logger implementation for logging to the debug window.
        /// </summary>
        private sealed class DebugLoggerProvider : ILogger
        {
            #region Constructors

            internal DebugLoggerProvider()
            {
            }

            #endregion

            #region Methods

            public void Log(string msg)
            {
                Debug.WriteLine(msg);
            }

            public void LogException(Exception ex, string message)
            {
                Debug.WriteLine(string.Format("{0} --- {1}", message, ex.ToString()));
            }

            public void LogExceptionFatal(Exception ex, string message)
            {
                Debug.WriteLine(string.Format("{0} --- {1}", message, ex.ToString()));
            }

            #endregion
        }

        #endregion Classes
    }
}