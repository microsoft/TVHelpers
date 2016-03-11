using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.Core.ViewModels
{
    public abstract class ViewModelBase : ModelBase, IDisposable
    {
        #region Variables

        private DispatcherTimer _statusTimer = null;

        #endregion Variables

        #region Properties

        /// <summary>
        /// Gets access to all the platform services.
        /// </summary>
        public Platform Platform { get { return Platform.Current; } }

        /// <summary>
        /// Gets access to the dispatcher for this view or application.
        /// </summary>
        private CoreDispatcher Dispatcher
        {
            get
            {
                if (this.View != null)
                    return this.View.Dispatcher;

                var coreWindow1 = CoreWindow.GetForCurrentThread();
                if (coreWindow1 != null)
                    return coreWindow1.Dispatcher;

                var coreWindow2 = CoreApplication.MainView.CoreWindow;
                if (coreWindow2 != null)
                    return coreWindow2.Dispatcher;

                throw new InvalidOperationException("Dispatcher not accessible!");
            }
        }

        private bool _IsUserAuthenticated = false;
        /// <summary>
        /// Gets whether or not a user is authenticated in this application.
        /// </summary>
        public bool IsUserAuthenticated
        {
            get { return _IsUserAuthenticated; }
            private set { this.SetProperty(ref _IsUserAuthenticated, value); }
        }

        private Page _View;
        /// <summary>
        /// Gets or sets access to the page instance associated to this ViewModel instance.
        /// </summary>
        public Page View
        {
            get { return _View; }
            set { _View = value; }
        }

        /// <summary>
        /// Gets access to the parameter passed to the page.
        /// </summary>
        public object ViewParameter { get; private set; }

        /// <summary>
        /// True or false indicating whether or not the view model has been called at least by a view bound to this view model.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// True or false indicating whether or not this view model requires the user to be authenticated.
        /// </summary>
        public bool RequiresAuthorization { get; protected set; }

        /// <summary>
        /// Gets or sets the title for the view specified by this view model.
        /// </summary>
        private string _Title;
        public virtual string Title
        {
            get { return _Title; }
            protected set { this.SetProperty(ref _Title, value); }
        }

        #region Status Properties

        private bool _StatusIsBusy = false;
        /// <summary>
        /// Gets whether or not the view model is in busy status.
        /// </summary>
        public bool StatusIsBusy
        {
            get { return _StatusIsBusy; }
            private set { this.SetProperty(ref _StatusIsBusy, value); }
        }

        private bool _StatusIsBlocking;
        /// <summary>
        /// Gets whether or not the busy status indicator for this view model should be blocking the UI view.
        /// </summary>
        public bool StatusIsBlocking
        {
            get { return _StatusIsBlocking; }
            private set { this.SetProperty(ref _StatusIsBlocking, value); }
        }

        private double? _StatusProgressValue = null;
        /// <summary>
        /// Gets the numerical value of the progress bar that should be displayed by busy status indicator.
        /// </summary>
        public double? StatusProgressValue
        {
            get { return _StatusProgressValue; }
            protected set { this.SetProperty(ref _StatusProgressValue, value); }
        }

        private string _StatusText;
        /// <summary>
        /// Gets the text that should be displayed by the busy status indicator.
        /// </summary>
        public string StatusText
        {
            get { return _StatusText; }
            private set { this.SetProperty(ref _StatusText, value); }
        }

        #endregion

        private CommandBase _navigateToAccountSignoutCommand = null;
        /// <summary>
        /// Command to sign out of the application.
        /// </summary>
        public CommandBase NavigateToAccountSignoutCommand
        {
            get { return _navigateToAccountSignoutCommand ?? (_navigateToAccountSignoutCommand = new GenericCommand("NavigateToAccountSignOutCommand", async () => await this.SignoutAsync())); }
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the view instance is in a child frame of the window.
        /// </summary>
        public bool IsViewInChildFrame
        {
            get { return Platform.Current.Navigation.IsChildFramePresent; }
        }

        /// <summary>
        /// Get a boolean indicating whether or not this view should show a home button.
        /// </summary>
        public bool ShowHomeButton
        {
            get
            {
                return Platform.Current.ViewModel.IsInitialized == false
                    && this.IsUserAuthenticated
                    && Platform.Current.Navigation.CanGoBack() == false;
            }
        }

        #endregion Properties

        #region Constructors

        public ViewModelBase()
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion

        #region Methods

        #region Navigation Methods

        /// <summary>
        /// Signs a users out of the application.
        /// </summary>
        /// <param name="isSilent">True to prompt the user else false to sign out immediately.</param>
        /// <returns>Awaitable task is returned.</returns>
        private async Task SignoutAsync(bool isSilent = false)
        {
            int result = 0;

            if (isSilent == false)
                result = await this.ShowMessageBoxAsync(Strings.Account.TextSignoutConfirmation, Strings.Account.TextSignout, new string[] { Strings.Account.TextSignout, Strings.Resources.TextCancel }, 1);

            if (result == 0)
            {
                try
                {
                    this.ShowBusyStatus(Strings.Account.TextSigningOut, true);
                    Platform.Current.Analytics.Event("AccountSignout");

                    // Allow the app core to signout
                    await Platform.Current.SignoutAll();

                    // Navigate home after successful signout
                    Platform.Current.Navigation.Home();
                }
                catch (Exception ex)
                {
                    Platform.Current.Logger.LogError(ex, "Error during ViewModelBase.SignoutAsync");
                    throw ex;
                }
                finally
                {
                    this.ClearStatus();
                }
            }
        }

        /// <summary>
        /// Allows a view model to prevent back navigation if it needs to.
        /// </summary>
        /// <returns>True if on back navigation should be cancelled else false.</returns>
        protected internal virtual bool OnBackNavigationRequested()
        {
            return false;
        }

        /// <summary>
        /// Allows a view model to prevent forward navigation if it needs to.
        /// </summary>
        /// <returns>True if on forward navigation should be cancelled else false.</returns>
        protected internal virtual bool OnForwardNavigationRequested()
        {
            return false;
        }

        #endregion

        #region Load/Save State Methods

        /// <summary>
        /// Called by a view's OnNavigatedTo event, LoadState allows view models to perform initialization logic on initial or subsequent views of this page and view model instance.
        /// </summary>
        /// <param name="view">View that is being shown.</param>
        /// <param name="e">Arguments containing navigation and page state data.</param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task LoadStateAsync(Page view, LoadStateEventArgs e)
        {
            // Store properties and subscribe to events
            bool isFirstRun = !this.IsInitialized;
            this.IsInitialized = true;
            this.View = view;
            this.ViewParameter = e.Parameter;
            this.IsUserAuthenticated = Platform.Current.AuthManager.IsAuthenticated();
            Platform.Current.AuthManager.UserAuthenticatedStatusChanged += AuthenticationManager_UserAuthenticated;

            // Check if user is authorized to view this page
            if (this.RequiresAuthorization && !this.IsUserAuthenticated)
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    await this.SignoutAsync(true);
                });
                return;
            }

            // Update properties with values from page state if found
            if (isFirstRun && e.PageState.Count > 0)
            {
                foreach (var pair in _preservePropertiesList)
                    this.LoadPropertyFromState(e, pair.Value);
            }
            
            // Fire load state on inherited view model instances
            await this.OnLoadStateAsync(e, isFirstRun);
        }

        /// <summary>
        /// Inherited view model instances perform any load state logic.
        /// </summary>
        /// <param name="e">Event args with all navigation data.</param>
        /// <param name="isFirstRun">True if this is the first execution of the view/viewmodel</param>
        /// <returns>Awaitable task is returned.</returns>
        protected virtual Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Inherited view model instances perform any save state logic.
        /// </summary>
        /// <param name="e">Event args with all navigation data.</param>
        /// <returns>Awaitable task is returned.</returns>
        protected virtual Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by a view's OnNavigatedFrom event, SaveState allows view models to perform suspend logic whenever the page is navigated away from.
        /// </summary>
        /// <param name="e">Event args with all navigation data.</param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task SaveStateAsync(SaveStateEventArgs e)
        {
            await this.OnSaveStateAsync(e);

            this.View = null;

            try
            {
                // Save all tracked properties to state
                foreach (var pair in _preservePropertiesList)
                    this.SavePropertyToState(e, pair.Value);
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to save properties to {0} page state.", this.GetType().Name);
            }

            Platform.Current.AuthManager.UserAuthenticatedStatusChanged -= AuthenticationManager_UserAuthenticated;

            // Dispose the viewmodel if navigating backwards
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.Back)
                this.Dispose();
        }

        public virtual void OnApplicationResuming()
        {
        }

        #endregion

        #region Status Methods

        /// <summary>
        /// Copies status data from another ViewModel.
        /// </summary>
        /// <param name="vm">ViewModel to copy status data from.</param>
        protected void CopyStatus(ViewModelBase vm)
        {
            this.StatusIsBlocking = vm.StatusIsBlocking;
            this.StatusIsBusy = vm.StatusIsBusy;
            this.StatusProgressValue = vm.StatusProgressValue;
            this.StatusText = vm.StatusText;
        }

        /// <summary>
        /// Show the busy UI to the user for an indefinite period of time.
        /// </summary>
        /// <param name="message">Message to display on the UI</param>
        /// <param name="isBlocking">True if full screen blocking UI else false.</param>
        protected void ShowBusyStatus(string message = null, bool isBlocking = false)
        {
            this.ShowTimedStatus(message, 0);
            this.StatusIsBusy = true;
            this.StatusIsBlocking = isBlocking;
        }

        /// <summary>
        /// Show the busy UI to the user for a definitive period of time.
        /// </summary>
        /// <param name="message">Message to display on the UI</param>
        /// <param name="milliseconds">Time in milliseconds to display the message.</param>
        protected void ShowTimedStatus(string message, int milliseconds = 5000)
        {
            this.StatusIsBusy = false;
            this.StatusIsBlocking = false;
            this.StatusText = message;

            if (DesignMode.DesignModeEnabled)
                return;

            this.ShutdownTimer();
            if (milliseconds > 0)
            {
                _statusTimer = new DispatcherTimer();
                _statusTimer.Interval = TimeSpan.FromMilliseconds(milliseconds);
                _statusTimer.Tick += (s, e) =>
                {
                    this.ClearStatus();
                    this.ShutdownTimer();
                };
                _statusTimer.Start();
            }
        }

        /// <summary>
        /// Clears any status messages on the UI.
        /// </summary>
        /// <param name="obj"></param>
        protected void ClearStatus(object obj)
        {
            this.ShutdownTimer();
            this.InvokeOnUIThread(this.ClearStatus);
        }

        /// <summary>
        /// Clears any status messages on the UI.
        /// </summary>
        protected virtual void ClearStatus()
        {
            this.StatusText = string.Empty;
            this.StatusProgressValue = null;
            this.StatusIsBusy = false;
            this.StatusIsBlocking = false;
        }

        private void ShutdownTimer()
        {
            if (_statusTimer != null)
            {
                _statusTimer.Stop();
                _statusTimer = null;
            }
        }

        #endregion Status Methods

        #region Property State Methods

        /// <summary>
        /// Notify subscribers on the UI thread that a property changed occurred.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        protected void NotifyPropertyChangedOnUI<TProperty>(Expression<Func<TProperty>> property)
        {
            this.InvokeOnUIThread(() => this.NotifyPropertyChanged(property));
        }

        private Dictionary<string, PropertyInfo> _preservePropertiesList = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// Registers properties to save/retrieve from page state during tombstoning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        protected void PreservePropertyState<T>(Expression<Func<T>> property)
        {
            var pi = this.GetPropertyInfo(property);
            if (pi != null && !_preservePropertiesList.ContainsKey(pi.Name))
                _preservePropertiesList.Add(pi.Name, pi);
        }

        /// <summary>
        /// Loads a property with data from page state if found.
        /// </summary>
        /// <param name="e">Load state event args for the view.</param>
        /// <param name="key">Name of the property.</param>
        /// <param name="pi">Property info object.</param>
        private void LoadPropertyFromState(LoadStateEventArgs e, PropertyInfo pi)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            // Check if value exists in state bag
            if (e.PageState != null && e.PageState.ContainsKey(pi.Name))
            {
                var storedValue = e.PageState[pi.Name];
                if (storedValue != null)
                {
                    Platform.Current.Logger.Log(LogLevels.Debug, "{0} - Restoring property {1} from page state of value {2}", this.GetType().Name, pi.Name, storedValue);
                    this.SetPropertyValue(pi, storedValue);
                }
            }
        }

        /// <summary>
        /// Saves a property to page state.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="pi"></param>
        private void SavePropertyToState(SaveStateEventArgs e, PropertyInfo pi)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            if (pi == null)
                throw new ArgumentNullException(nameof(pi));

            var value = pi.GetValue(this);

            // Add key/value to state bag
            if (e.PageState.ContainsKey(pi.Name))
                e.PageState[pi.Name] = value;
            else
                e.PageState.Add(pi.Name, value);

            Platform.Current.Logger.Log(LogLevels.Debug, "{0} - Saved property {1} to page state of value {2}", this.GetType().Name, pi.Name, value);
        }

        #endregion Property State Methods

        #region Dispatcher Methods

        /// <summary>
        /// Runs a function on the currently executing platform's UI thread.
        /// </summary>
        /// <param name="action">Code to be executed on the UI thread</param>
        /// <param name="priority">Priority to indicate to the system when to prioritize the execution of the code</param>
        /// <returns>Task representing the code to be executing</returns>
        protected void InvokeOnUIThread(Action action, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (this.Dispatcher == null || this.Dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                // Execute asynchronously on the thread the Dispatcher is associated with.
                var task = this.Dispatcher.RunAsync(priority, () => action());
            }
        }

        #endregion

        #region MessageBox Methods

        /// <summary>
        /// Displays a message box dialog.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns>Awaitable call which returns the index of the button clicked.</returns>
        protected internal Task<int> ShowMessageBoxAsync(string message, CancellationToken ct)
        {
            return this.ShowMessageBoxAsync(message, Strings.Resources.ApplicationName, null, 0, ct);
        }

        /// <summary>
        /// Displays a message box dialog.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="buttonNames">List of buttons to display.</param>
        /// <param name="defaultIndex">Index of the default button of the dialog box.</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns>Awaitable call which returns the index of the button clicked.</returns>
        protected internal Task<int> ShowMessageBoxAsync(string message, IList<string> buttonNames = null, int defaultIndex = 0, CancellationToken? ct = null)
        {
            return this.ShowMessageBoxAsync(message, Strings.Resources.ApplicationName, buttonNames, defaultIndex, ct);
        }

        /// <summary>
        /// Displays a message box dialog.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="title">Title of the message box.</param>
        /// <param name="buttonNames">List of buttons to display.</param>
        /// <param name="defaultIndex">Index of the default button of the dialog box.</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns>Awaitable call which returns the index of the button clicked.</returns>
        protected internal async Task<int> ShowMessageBoxAsync(string message, string title, IList<string> buttonNames = null, int defaultIndex = 0, CancellationToken? ct = null)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("The specified message cannot be null or empty.", "message");

            // Set a default title if no title was specified.
            if (string.IsNullOrWhiteSpace(title))
                title = Strings.Resources.ApplicationName;

            int result = defaultIndex;
            MessageDialog dialog = new MessageDialog(message, title);

            // Show all the button names specified or just an OK label if no names were specified.
            if (buttonNames != null && buttonNames.Count > 0)
                foreach (string button in buttonNames)
                    dialog.Commands.Add(new UICommand(button, new UICommandInvokedHandler((o) => result = buttonNames.IndexOf(button))));
            else
                dialog.Commands.Add(new UICommand(Strings.Resources.TextOk, new UICommandInvokedHandler((o) => result = 0)));

            // Set the default button of the dialog
            dialog.DefaultCommandIndex = (uint)defaultIndex;

            // Show on the appropriate thread
            if (this.Dispatcher == null || this.Dispatcher.HasThreadAccess)
            {
                await dialog.ShowAsync().AsTask(ct.HasValue ? ct.Value : CancellationToken.None);
                return result;
            }
            else
            {
                var tcs = new TaskCompletionSource<int>();

                // Execute asynchronously on the thread the Dispatcher is associated with.
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    await dialog.ShowAsync().AsTask(ct.HasValue ? ct.Value : CancellationToken.None);
                    tcs.TrySetResult(result);
                });
                return tcs.Task.Result;
            }
        }

        #endregion

        #region Refresh Async

        private CommandBase _RefreshCommand;
        /// <summary>
        /// Gets a command instance for refreshing the page.
        /// </summary>
        public CommandBase RefreshCommand
        {
            get { return _RefreshCommand ?? (_RefreshCommand = new GenericCommand("RefreshCommand", async () => await this.RefreshAsync(), () => this.IsRefreshEnabled)); }
        }

        private bool _IsRefreshVisible;
        /// <summary>
        /// Gets or sets whether or not the refresh button should be shown on the page.
        /// </summary>
        public bool IsRefreshVisible
        {
            get { return _IsRefreshVisible; }
            protected set { this.SetProperty(ref _IsRefreshVisible, value); }
        }

        private bool _IsRefreshEnabled = true;
        /// <summary>
        /// Gets or sets whether or not the refresh button is enabled or not.
        /// </summary>
        public bool IsRefreshEnabled
        {
            get { return _IsRefreshEnabled; }
            private set
            {
                if (this.SetProperty(ref _IsRefreshEnabled, value))
                    this.RefreshCommand.RaiseCanExecuteChanged();
            }
        }

        private CancellationTokenSource _cts;

        /// <summary>
        /// Refreshes data on the entire page. 
        /// </summary>
        /// <returns></returns>
        public async Task RefreshAsync()
        {
            if (_cts != null)
                return;

            try
            {
                this.IsRefreshEnabled = false;
                _cts = new CancellationTokenSource();
                await this.OnRefreshAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error refreshing view model '{0}' with parameters: {1}", this.GetType().Name, this.ViewParameter);
            }
            finally
            {
                this.IsRefreshEnabled = true;
                this.RefreshReset();
            }
        }

        /// <summary>
        /// Inherited view models can implement their own logic as to what happens when a page refresh is requested.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected virtual Task OnRefreshAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        // Clean up after a refresh occurs.
        private void RefreshReset()
        {
            if(_cts?.IsCancellationRequested == false)
                _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public virtual void Dispose()
        {
            this.RefreshReset();
        }

        #endregion

        #region Caching

        private const string APP_CACHE_PATH = @"AppDataCache\{0}_{1}.data";

        /// <summary>
        /// Fills a property with data from the app cache if available.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        protected async Task<T> LoadFromCacheAsync<T>(Expression<Func<T>> property, string identifier = null)
        {
            if (property == null)
                return default(T);
            
            var key = this.GetPropertyInfo(property).Name + identifier;
            
            try
            {
                return await Platform.Current.Storage.LoadFileAsync<T>(string.Format(APP_CACHE_PATH, this.GetType().Name, key), Windows.Storage.ApplicationData.Current.LocalCacheFolder);
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error retrieving '{0}' from cache data.", key);
                return default(T);
            }
        }

        /// <summary>
        /// Stores a property's data to the app cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        protected async Task SaveToCacheAsync<T>(Expression<Func<T>> property, string identifier = null)
        {
            if (property == null)
                return;

            var pi = this.GetPropertyInfo(property);
            var key = pi.Name + identifier;

            try
            {
                object data = pi.GetValue(this);
                await Platform.Current.Storage.SaveFileAsync(string.Format(APP_CACHE_PATH, this.GetType().Name, key), data, Windows.Storage.ApplicationData.Current.LocalCacheFolder);
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error saving '{0}' to cache data.", key);
            }
        }

        #endregion

        #region Tasks

        /// <summary>
        /// Waits for a list of task to complete before continuing execution.
        /// </summary>
        /// <param name="tasks">List of tasks to execute and wait for all to complete.</param>
        /// <returns>Awaitable task is returned.</returns>
        protected async Task WaitAllAsync(CancellationToken ct, params Task[] tasks)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            var t = Task.Factory.StartNew(() =>
            {
                try
                {
                    Task.WaitAll(tasks, ct);
                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, ct);

            await tcs.Task;
        }

        #endregion

        #endregion Methods

        #region Event Handlers

        private void AuthenticationManager_UserAuthenticated(object sender, bool e)
        {
            // Subscribes to user authentication changed event from AuthorizationManager
            this.InvokeOnUIThread(() =>
            {
                this.IsUserAuthenticated = e;
            });
        }

        #endregion
    }

    #region Classes

    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.LoadState"/>event
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.SaveState"/>event
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// Class used to hold the event data required when a page attempts to load state.
    /// </summary>
    public sealed class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the navigation event args passed to the OnNavigatingTo event of the page.
        /// </summary>
        public NavigationEventArgs NavigationEventArgs { get; private set; }

        /// <summary>
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </summary>
        public IDictionary<string, object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadStateEventArgs"/> class.
        /// </summary>
        /// <param name="navigationParameter">
        /// The parameter value passed to <see cref="Frame.Navigate(Type, Object)"/> 
        /// when this page was initially requested.
        /// </param>
        /// <param name="pageState">
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </param>
        public LoadStateEventArgs(NavigationEventArgs e, IDictionary<string, object> pageState, bool isViewInitialized)
            : base()
        {
            this.NavigationEventArgs = e;
            this.PageState = pageState;
            this.Parameter = NavigationParameterSerializer.Deserialize(e.Parameter); // Deserializes the parameter from the navigation event if necessary and stores instance
            this.IsViewInitialized = isViewInitialized;
        }

        /// <summary>
        /// Gets the deserialized instance of the parameter passed to this page.
        /// </summary>
        public object Parameter { get; private set; }

        public bool IsViewInitialized { get; private set; }
    }

    /// <summary>
    /// Class used to hold the event data required when a page attempts to save state.
    /// </summary>
    public sealed class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the navigation event args passed to the OnNavigatedFrom event of the page.
        /// </summary>
        public NavigationEventArgs NavigationEventArgs { get; private set; }

        /// <summary>
        /// An empty dictionary to be populated with serializable state.
        /// </summary>
        public IDictionary<string, object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        public SaveStateEventArgs(NavigationEventArgs e, IDictionary<string, object> pageState)
            : base()
        {
            this.NavigationEventArgs = e;
            this.PageState = pageState;
        }
    }

    #endregion
}