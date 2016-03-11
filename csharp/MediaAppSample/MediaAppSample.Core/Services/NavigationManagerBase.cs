using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.Core.Services
{
    /// <summary>
    /// Base class for accessing navigation services on the platform currently executing.
    /// </summary>
    public abstract partial class NavigationManagerBase : ServiceBase
    {
        #region Properties

        #region Core Navigation Commands

        private CommandBase _navigateToHomeCommand = null;
        /// <summary>
        /// Command to access backwards page navigation..
        /// </summary>
        public CommandBase NavigateToHomeCommand
        {
            get { return _navigateToHomeCommand ?? (_navigateToHomeCommand = new NavigationCommand("NavigateToHomeCommand", this.Home)); }
        }

        private CommandBase _navigateGoBackCommand = null;
        /// <summary>
        /// Command to access backwards page navigation..
        /// </summary>
        public CommandBase NavigateGoBackCommand
        {
            get { return _navigateGoBackCommand ?? (_navigateGoBackCommand = new NavigationCommand("NavigateGoBackCommand", () => this.GoBack(), this.CanGoBack)); }
        }

        private CommandBase _navigateGoForwardCommand = null;
        /// <summary>
        /// Command to access forard page navigation.
        /// </summary>
        public CommandBase NavigateGoForwardCommand
        {
            get { return _navigateGoForwardCommand ?? (_navigateGoForwardCommand = new NavigationCommand("NavigateGoForwardCommand", () => this.GoForward(), this.CanGoForward)); }
        }

        #endregion

        #region Page Commands

        private CommandBase _navigateToModelCommand = null;
        /// <summary>
        /// Command to access navigating to an instance of a model (Navigation manager handles actually forwarding to the appropriate view for 
        /// the model pass into a parameter. 
        /// </summary>
        public CommandBase NavigateToModelCommand
        {
            get { return _navigateToModelCommand ?? (_navigateToModelCommand = new NavigationCommand()); }
        }

        private CommandBase _navigateToSettingsCommand = null;
        /// <summary>
        /// Command to navigate to the settings view.
        /// </summary>
        public CommandBase NavigateToSettingsCommand
        {
            get { return _navigateToSettingsCommand ?? (_navigateToSettingsCommand = new NavigationCommand("NavigateToSettingsCommand", this.Settings)); }
        }

        private CommandBase _navigateToAboutCommand = null;
        /// <summary>
        /// Command to navigate to the about view.
        /// </summary>
        public CommandBase NavigateToAboutCommand
        {
            get { return _navigateToAboutCommand ?? (_navigateToAboutCommand = new NavigationCommand("NavigateToAboutCommand", this.About)); }
        }

        private CommandBase _navigateToPrivacyPolicyCommand = null;
        /// <summary>
        /// Command to navigate to the application's privacy policy view.
        /// </summary>
        public CommandBase NavigateToPrivacyPolicyCommand
        {
            get { return _navigateToPrivacyPolicyCommand ?? (_navigateToPrivacyPolicyCommand = new NavigationCommand("NavigateToPrivacyPolicyCommand", this.PrivacyPolicy)); }
        }

        private CommandBase _navigateToTermsOfServiceCommand = null;
        /// <summary>
        /// Command to navigate to the application's terms of service view.
        /// </summary>
        public CommandBase NavigateToTermsOfServiceCommand
        {
            get { return _navigateToTermsOfServiceCommand ?? (_navigateToTermsOfServiceCommand = new NavigationCommand("NavigateToTermsOfServiceCommand", this.TermsOfService)); }
        }
        
        private CommandBase _navigateToQueueCommand = null;
        public CommandBase NavigateToQueueCommand
        {
            get { return _navigateToQueueCommand ?? (_navigateToQueueCommand = new NavigationCommand("NavigateToQueueCommand", Platform.Current.Navigation.Queue)); }
        }

        private CommandBase _navigateToMoviesCommand = null;
        public CommandBase NavigateToMoviesCommand
        {
            get { return _navigateToMoviesCommand ?? (_navigateToMoviesCommand = new NavigationCommand("NavigateToMoviesCommand", Platform.Current.Navigation.Movies)); }
        }

        private CommandBase _navigateToTVCommand = null;
        public CommandBase NavigateToTVCommand
        {
            get { return _navigateToTVCommand ?? (_navigateToTVCommand = new NavigationCommand("NavigateToTVCommand", Platform.Current.Navigation.TV)); }
        }

        private CommandBase _navigateToMediaCommand = null;
        public CommandBase NavigateToMediaCommand
        {
            get { return _navigateToMediaCommand ?? (_navigateToMediaCommand = new NavigationCommand("NavigateToMediaCommand", Platform.Current.Navigation.Media)); }
        }

        private CommandBase _navigateToDetailsCommand = null;
        public CommandBase NavigateToDetailsCommand
        {
            get { return _navigateToDetailsCommand ?? (_navigateToDetailsCommand = new NavigationCommand("NavigateToDetailsCommand", Platform.Current.Navigation.Details)); }
        }

        #endregion

        #region Web Browser Commands

        private CommandBase _navigateToWebViewCommand = null;
        /// <summary>
        /// Command to navigate to the internal web view.
        /// </summary>
        public CommandBase NavigateToWebViewCommand
        {
            get { return _navigateToWebViewCommand ?? (_navigateToWebViewCommand = new WebViewCommand()); }
        }

        private CommandBase _navigateToWebBrowserCommand = null;
        /// <summary>
        /// Command to navigate to the external web browser.
        /// </summary>
        public CommandBase NavigateToWebBrowserCommand
        {
            get { return _navigateToWebBrowserCommand ?? (_navigateToWebBrowserCommand = new WebBrowserCommand()); }
        }

        #endregion

        #region Map Commands

        private CommandBase _navigateToMapExternalCommand = null;
        /// <summary>
        /// Command to access the external maps view.
        /// </summary>
        public CommandBase NavigateToMapExternalCommand
        {
            get { return _navigateToMapExternalCommand ?? (_navigateToMapExternalCommand = new MapExternalCommand()); }
        }

        private CommandBase _navigateToMapExternalDrivingCommand = null;
        /// <summary>
        /// Command to access the device's map driving directions view.
        /// </summary>
        public CommandBase NavigateToMapExternalDrivingCommand
        {
            get { return _navigateToMapExternalDrivingCommand ?? (_navigateToMapExternalDrivingCommand = new MapExternalCommand(MapExternalOptions.DrivingDirections)); }
        }

        private CommandBase _navigateToMapExternalWalkingCommand = null;
        /// <summary>
        /// Command to access the device's map walking directions view.
        /// </summary>
        public CommandBase NavigateToMapExternalWalkingCommand
        {
            get { return _navigateToMapExternalWalkingCommand ?? (_navigateToMapExternalWalkingCommand = new MapExternalCommand(MapExternalOptions.WalkingDirections)); }
        }

        #endregion

        #region Search Commands

        private CommandBase _navigateToSearchCommand = null;
        /// <summary>
        /// Command to navigate to the application's search view.
        /// </summary>
        public CommandBase NavigateToSearchCommand
        {
            get { return _navigateToSearchCommand ?? (_navigateToSearchCommand = new GenericCommand<string>("NavigateToSearchCommand", (e) => this.Search(e))); }
        }

        #endregion

        #region Account Commands

        private CommandBase _navigateToAccountSignInCommand = null;
        /// <summary>
        /// Command to navigate to the account sign in view.
        /// </summary>
        public CommandBase NavigateToAccountSignInCommand
        {
            get { return _navigateToAccountSignInCommand ?? (_navigateToAccountSignInCommand = new NavigationCommand("NavigateToAccountSignInCommand", this.AccountSignin)); }
        }

        private CommandBase _navigateToAccountSignUpCommand = null;
        /// <summary>
        /// Command to navigate to the account sign up view.
        /// </summary>
        public CommandBase NavigateToAccountSignUpCommand
        {
            get { return _navigateToAccountSignUpCommand ?? (_navigateToAccountSignUpCommand = new NavigationCommand("NavigateToAccountSignUpCommand", this.AccountSignup)); }
        }

        private CommandBase _navigateToAccountForgotCommand = null;
        /// <summary>
        /// Command to navigate to the account forgot crentials view.
        /// </summary>
        public CommandBase NavigateToAccountForgotCommand
        {
            get { return _navigateToAccountForgotCommand ?? (_navigateToAccountForgotCommand = new NavigationCommand("NavigateToAccountForgotCommand", this.AccountForgot)); }
        }

        #endregion

        #region Rate App Commands

        private CommandBase _navigateToRateAppCommand = null;
        /// <summary>
        /// Command to navigate to the platform's rate application functionality.
        /// </summary>
        public CommandBase NavigateToRateAppCommand
        {
            get { return _navigateToRateAppCommand ?? (_navigateToRateAppCommand = new NavigationCommand("NavigateToRateAppCommand", async () => await this.RateApplicationAsync())); }
        }

        #endregion

        #endregion

        #region Abstract Methods

        protected abstract Frame CreateFrame();

        protected abstract bool OnActivation(LaunchActivatedEventArgs e);

        protected abstract bool OnActivation(ToastNotificationActivatedEventArgs e);

        protected abstract bool OnActivation(VoiceCommandActivatedEventArgs e);

        protected abstract bool OnActivation(ProtocolActivatedEventArgs e);

        public abstract void Home(object parameter = null);

        public abstract void NavigateTo(IModel model);

        protected abstract void WebView(object parameter);

        public abstract void AccountSignin(object parameter = null);

        public abstract void AccountSignup(object parameter = null);

        public abstract void AccountForgot(object parameter = null);

        public abstract void Settings(object parameter = null);

        public abstract void Search(object parameter = null);

        public abstract void Details(object parameter);

        public abstract void Queue();

        public abstract void Movies();

        public abstract void TV();

        public abstract void Media(object parameter);

        #endregion
    }
}