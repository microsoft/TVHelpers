using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using System;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class WebBrowserViewModel : ViewModelBase
    {
        #region Events

        public event EventHandler RefreshRequested;
        public event EventHandler GoForwardRequested;
        public event EventHandler GoBackwardsRequested;
        public event EventHandler GoHomeRequested;
        public event EventHandler<string> NavigateToRequested;

        #endregion

        #region Properties

        private string _title;
        /// <summary>
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return _title ?? Strings.Resources.ApplicationName; }
        }
        
        public object BrowserInstance { get; set; }
        
        private bool _ShowNavigation;
        /// <summary>
        /// Gets whether or not the navigation command bar should be shown.
        /// </summary>
        public bool ShowNavigation
        {
            get { return _ShowNavigation; }
            private set { this.SetProperty(ref _ShowNavigation, value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether navigating back on the view should skip back override to allow browser back history to be navigated to on back presses.
        /// </summary>
        protected bool ForceBrowserGoBackOnNavigationBack { get; set; }

        private bool _ShowBrowser;
        /// <summary>
        /// Gets whether the browser should be visible or not.
        /// </summary>
        public bool ShowBrowser
        {
            get { return _ShowBrowser; }
            private set
            {
                if (this.SetProperty(ref _ShowBrowser, value))
                    this.BrowserRefreshCommand.RaiseCanExecuteChanged();
            }
        }

        private string _BrowseErrorMessage;
        /// <summary>
        /// Gets any error messages that need to be shown on the UI.
        /// </summary>
        public string BrowseErrorMessage
        {
            get { return _BrowseErrorMessage; }
            private set { this.SetProperty(ref _BrowseErrorMessage, value); }
        }

        /// <summary>
        /// Gets a command used to refresh the current view.
        /// </summary>
        public CommandBase BrowserRefreshCommand { get; private set; }

        /// <summary>
        /// Gets a command used to take the user back to the home webpage.
        /// </summary>
        public CommandBase BrowserHomeCommand { get; private set; }

        /// <summary>
        /// Returns whether or not the browser can go back or not.
        /// </summary>
        public Func<bool> BrowserCanGoBack { get; private set; }

        /// <summary>
        /// Returns whether or not the browser can go forward or not.
        /// </summary>
        public Func<bool> BrowserCanGoForward { get; private set; }

        #endregion Properties

        #region Constructors

        public WebBrowserViewModel(bool showNavigation = true)
        {
            if (DesignMode.DesignModeEnabled)
                return;
            
            this.ShowNavigation = showNavigation;
            this.BrowserRefreshCommand = new GenericCommand<IModel>("WebViewViewModel-RefreshCommand", this.BrowserRefresh, () => this.ShowBrowser);
            this.BrowserHomeCommand = new GenericCommand("WebBrowserViewModel-Home", this.BrowserGoHome, () => this.BrowserCanGoBack());

            this.SetBrowserFunctions(() => false, () => false);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Initial page that should be navigated on launch of the application. 
        /// </summary>
        public virtual void InitialNavigation()
        {
            if (this.ViewParameter is string)
                this.NavigateTo(this.ViewParameter.ToString());
        }

        protected internal override bool OnBackNavigationRequested()
        {
            // Allow the browser to tell the global navigation that it should override back navigation and instead nav back in the browser view.
            if (this.ForceBrowserGoBackOnNavigationBack == false && this.BrowserCanGoBack != null && this.BrowserCanGoBack())
            {
                this.BrowserGoBack();
                return true;
            }
            else
                return base.OnBackNavigationRequested();
        }

        protected internal override bool OnForwardNavigationRequested()
        {
            // Allow the browser to tell the global navigation that it should override forward navigation and instead nav forward in the browser view.
            if (this.BrowserCanGoForward != null && this.BrowserCanGoForward())
            {
                this.BrowserGoForward();
                return true;
            }
            else
                return base.OnForwardNavigationRequested();
        }

        /// <summary>
        /// Clears any error status messagse.
        /// </summary>
        protected override void ClearStatus()
        {
            this.BrowseErrorMessage = null;
            base.ClearStatus();
        }

        /// <summary>
        /// Notify the VM that the browser is in the process of navigating to a particular page and offer the ability for it to cancel the navigation.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool Navigating(Uri uri)
        {
            this.ShowBusyStatus();
            this.ShowBrowser = false;
            this.BrowserRefreshCommand.RaiseCanExecuteChanged();

            // True if navigation should be cancelled, False if it should continue
            return false;
        }

        /// <summary>
        /// Notify this VM that a page has been navigated to.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="title"></param>
        public void Navigated(Uri uri, string title = null)
        {
            this.SetTitle(title);
            this.ClearStatus();
            this.ShowBrowser = true;
            Platform.Current.Navigation.NavigateGoBackCommand.RaiseCanExecuteChanged();
            Platform.Current.Navigation.NavigateGoForwardCommand.RaiseCanExecuteChanged();
            this.BrowserHomeCommand.RaiseCanExecuteChanged();
            this.BrowserRefreshCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Notify this VM that a navigation failure has occurred.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="exception"></param>
        /// <param name="title"></param>
        public void NavigationFailed(Uri uri, Exception exception, string title = null)
        {
            this.ClearStatus();
            this.ShowBrowser = false;
            Platform.Current.Navigation.NavigateGoBackCommand.RaiseCanExecuteChanged();
            Platform.Current.Navigation.NavigateGoForwardCommand.RaiseCanExecuteChanged();
#if DEBUG
            this.BrowseErrorMessage = exception == null ? Strings.WebBrowser.TextWebErrorGeneric : exception.ToString();
#else
            this.BrowseErrorMessage = Strings.WebBrowser.TextWebErrorGeneric;
#endif
            Platform.Current.Logger.LogError(exception, "Error navigating to " + uri.ToString());
        }

        /// <summary>
        /// Sets the view title.
        /// </summary>
        /// <param name="title">Title text to display.</param>
        private void SetTitle(string title)
        {
            _title = title;
            this.NotifyPropertyChanged(() => this.Title);
        }

        /// <summary>
        /// Configure the VM to perform execute custom functions when the browser can go back/forward.
        /// </summary>
        /// <param name="canGoBack"></param>
        /// <param name="canGoForward"></param>
        public void SetBrowserFunctions(Func<bool> canGoBack, Func<bool> canGoForward)
        {
            if (canGoBack != null) this.BrowserCanGoBack = canGoBack;
            if (canGoForward != null) this.BrowserCanGoForward = canGoForward;
        }

        /// <summary>
        /// Refreshes the web browser.
        /// </summary>
        public void BrowserRefresh()
        {
            if (this.RefreshRequested != null)
                this.RefreshRequested(this.BrowserInstance, new EventArgs());
        }

        /// <summary>
        /// Navigates the web browser backwards.
        /// </summary>
        public void BrowserGoBack()
        {
            if (this.GoBackwardsRequested != null)
                this.GoBackwardsRequested(this.BrowserInstance, new EventArgs());
        }

        /// <summary>
        /// Navigates the web browser forward.
        /// </summary>
        public void BrowserGoForward()
        {
            if (this.GoForwardRequested != null)
                this.GoForwardRequested(this.BrowserInstance, new EventArgs());
        }

        /// <summary>
        /// Navigates the web browser to the home page.
        /// </summary>
        public void BrowserGoHome()
        {
            if (this.GoHomeRequested != null)
                this.GoHomeRequested(this.BrowserInstance, new EventArgs());
        }

        /// <summary>
        /// Navigate to a specific web page.
        /// </summary>
        /// <param name="url">URL to navigate to.</param>
        public void NavigateTo(string url)
        {
            if (this.NavigateToRequested != null && !string.IsNullOrEmpty(url))
                this.NavigateToRequested(this.BrowserInstance, url);
        }

        #endregion Methods
    }

    public partial class WebBrowserViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public WebBrowserViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class WebBrowserViewModel : MediaAppSample.Core.ViewModels.WebBrowserViewModel
    {
        public WebBrowserViewModel()
            : base()
        {
        }
    }
}