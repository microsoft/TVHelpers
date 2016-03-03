using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using System;
using System.Threading.Tasks;
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

        public override string Title
        {
            get { return _title ?? Strings.Resources.ApplicationName; }
        }
        
        public object BrowserInstance { get; set; }
        
        private bool _ShowNavigation;
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

        public CommandBase BrowserHomeCommand { get; private set; }

        public Func<bool> BrowserCanGoBack { get; private set; }

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

        public override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
            }

            return base.OnLoadStateAsync(e, isFirstRun);
        }

        public virtual void InitialNavigation()
        {
            if (this.ViewParameter is string)
                this.NavigateTo(this.ViewParameter.ToString());
        }

        public override bool OnBackNavigationRequested()
        {
            if (this.ForceBrowserGoBackOnNavigationBack == false && this.BrowserCanGoBack != null && this.BrowserCanGoBack())
            {
                this.BrowserGoBack();
                return true;
            }
            else
                return base.OnBackNavigationRequested();
        }

        public override bool OnForwardNavigationRequested()
        {
            if (this.BrowserCanGoForward != null && this.BrowserCanGoForward())
            {
                this.BrowserGoForward();
                return true;
            }
            else
                return base.OnForwardNavigationRequested();
        }

        protected override void ClearStatus()
        {
            this.BrowseErrorMessage = null;
            base.ClearStatus();
        }

        public bool Navigating(Uri uri)
        {
            this.ShowBusyStatus();
            this.ShowBrowser = false;
            this.BrowserRefreshCommand.RaiseCanExecuteChanged();

            // True if navigation should be cancelled, False if it should continue
            return false;
        }

        public void Navigated(Uri uri, string title = null)
        {
            this.SetTitle(title);
            this.ClearStatus();
            this.ShowBrowser = true;
            this.Commands.NavigateGoBackCommand.RaiseCanExecuteChanged();
            this.Commands.NavigateGoForwardCommand.RaiseCanExecuteChanged();
            this.BrowserHomeCommand.RaiseCanExecuteChanged();
            this.BrowserRefreshCommand.RaiseCanExecuteChanged();
        }

        public void NavigationFailed(Uri uri, Exception exception, string title = null)
        {
            this.ClearStatus();
            this.ShowBrowser = false;
            this.Commands.NavigateGoBackCommand.RaiseCanExecuteChanged();
            this.Commands.NavigateGoForwardCommand.RaiseCanExecuteChanged();
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
        public virtual WebBrowserViewModel ViewModel { get { return this; } }
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