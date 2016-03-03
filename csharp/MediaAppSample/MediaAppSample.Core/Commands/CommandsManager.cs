using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Commands
{
    /// <summary>
    /// Centralized instance of commands that are commonly used throughout the application.
    /// </summary>
    public sealed class CommandsManager
    {
        #region Constructors

        /// <summary>
        /// Singleton access to reusable command instances for the application.
        /// </summary>
        public static CommandsManager Instance { get; private set; }

        static CommandsManager()
        {
            Instance = new CommandsManager();
        }

        #endregion Constructors

        #region Properties

        #region General Commands

        private CommandBase _navigateToHomeCommand = null;
        /// <summary>
        /// Command to access backwards page navigation..
        /// </summary>
        public CommandBase NavigateToHomeCommand
        {
            get { return _navigateToHomeCommand ?? (_navigateToHomeCommand = new NavigationCommand("NavigateToHomeCommand", Platform.Current.Navigation.Home)); }
        }

        private CommandBase _navigateGoBackCommand = null;
        /// <summary>
        /// Command to access backwards page navigation..
        /// </summary>
        public CommandBase NavigateGoBackCommand
        {
            get { return _navigateGoBackCommand ?? (_navigateGoBackCommand = new NavigationCommand("NavigateGoBackCommand", ()=> Platform.Current.Navigation.GoBack(), Platform.Current.Navigation.CanGoBack)); }
        }

        private CommandBase _navigateGoForwardCommand = null;
        /// <summary>
        /// Command to access forard page navigation.
        /// </summary>
        public CommandBase NavigateGoForwardCommand
        {
            get { return _navigateGoForwardCommand ?? (_navigateGoForwardCommand = new NavigationCommand("NavigateGoForwardCommand", () => Platform.Current.Navigation.GoForward(), Platform.Current.Navigation.CanGoForward)); }
        }

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
            get { return _navigateToSettingsCommand ?? (_navigateToSettingsCommand = new NavigationCommand("NavigateToSettingsCommand", Platform.Current.Navigation.Settings)); }
        }

        private CommandBase _navigateToAboutCommand = null;
        /// <summary>
        /// Command to navigate to the about view.
        /// </summary>
        public CommandBase NavigateToAboutCommand
        {
            get { return _navigateToAboutCommand ?? (_navigateToAboutCommand = new NavigationCommand("NavigateToAboutCommand", Platform.Current.Navigation.About)); }
        }

        private CommandBase _navigateToRateAppCommand = null;
        /// <summary>
        /// Command to navigate to the platform's rate application functionality.
        /// </summary>
        public CommandBase NavigateToRateAppCommand
        {
            get { return _navigateToRateAppCommand ?? (_navigateToRateAppCommand = new NavigationCommand("NavigateToRateAppCommand", async () => await Platform.Current.Navigation.RateApplicationAsync())); }
        }

        private CommandBase _navigateToPrivacyPolicyCommand = null;
        /// <summary>
        /// Command to navigate to the application's privacy policy view.
        /// </summary>
        public CommandBase NavigateToPrivacyPolicyCommand
        {
            get { return _navigateToPrivacyPolicyCommand ?? (_navigateToPrivacyPolicyCommand = new NavigationCommand("NavigateToPrivacyPolicyCommand", Platform.Current.Navigation.PrivacyPolicy)); }
        }

        private CommandBase _navigateToTermsOfServiceCommand = null;
        /// <summary>
        /// Command to navigate to the application's terms of service view.
        /// </summary>
        public CommandBase NavigateToTermsOfServiceCommand
        {
            get { return _navigateToTermsOfServiceCommand ?? (_navigateToTermsOfServiceCommand = new NavigationCommand("NavigateToTermsOfServiceCommand", Platform.Current.Navigation.TermsOfService)); }
        }

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

        private CommandBase _shareCommand = null;
        /// <summary>
        /// Command to navigate to the share functionality.
        /// </summary>
        public CommandBase ShareCommand
        {
            get { return _shareCommand ?? (_shareCommand = new ShareCommand()); }
        }


        private CommandBase _navigateToGalleryCommand = null;
        public CommandBase NavigateToGalleryCommand
        {
            get { return _navigateToGalleryCommand ?? (_navigateToGalleryCommand = new NavigationCommand("NavigateToGalleryCommand", Platform.Current.Navigation.Gallery)); }
        }


        private CommandBase _navigateToQueueCommand = null;
        public CommandBase NavigateToQueueCommand
        {
            get { return _navigateToQueueCommand ?? (_navigateToQueueCommand = new NavigationCommand("NavigateToQueueCommand", Platform.Current.Navigation.Queue)); }
        }


        private CommandBase _navigateToMediaCommand = null;
        public CommandBase NavigateToMediaCommand
        {
            get { return _navigateToMediaCommand ?? (_navigateToMediaCommand = new NavigationCommand("NavigateToMediaCommand", Platform.Current.Navigation.Media)); }
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
            get { return _navigateToSearchCommand ?? (_navigateToSearchCommand = new GenericCommand<string>("NavigateToSearchCommand", (e) => Platform.Current.Navigation.Search(e))); }
        }

        #endregion

        #region Account Commands

        private CommandBase _navigateToAccountSignInCommand = null;
        /// <summary>
        /// Command to navigate to the account sign in view.
        /// </summary>
        public CommandBase NavigateToAccountSignInCommand
        {
            get { return _navigateToAccountSignInCommand ?? (_navigateToAccountSignInCommand = new NavigationCommand("NavigateToAccountSignInCommand", Platform.Current.Navigation.AccountSignin)); }
        }

        private CommandBase _navigateToAccountSignUpCommand = null;
        /// <summary>
        /// Command to navigate to the account sign up view.
        /// </summary>
        public CommandBase NavigateToAccountSignUpCommand
        {
            get { return _navigateToAccountSignUpCommand ?? (_navigateToAccountSignUpCommand = new NavigationCommand("NavigateToAccountSignUpCommand", Platform.Current.Navigation.AccountSignup)); }
        }

        private CommandBase _navigateToAccountForgotCommand = null;
        /// <summary>
        /// Command to navigate to the account forgot crentials view.
        /// </summary>
        public CommandBase NavigateToAccountForgotCommand
        {
            get { return _navigateToAccountForgotCommand ?? (_navigateToAccountForgotCommand = new NavigationCommand("NavigateToAccountForgotCommand", Platform.Current.Navigation.AccountForgot)); }
        }

        private CommandBase _navigateToNewWindowCommand = null;
        /// <summary>
        /// Command to navigate to the account forgot crentials view.
        /// </summary>
        public CommandBase NavigateToNewWindowCommand
        {
            get
            {
                return Platform.Current == null ? null : _navigateToNewWindowCommand ?? (_navigateToNewWindowCommand = new GenericCommand<ViewModelBase>("NavigateToNewWindowCommand", async (e) =>
                {
                    await Platform.Current.Navigation.NavigateInNewWindow(e.View.GetType(), e.ViewParameter);
                }));
            }
        }

        #endregion

        #region Email Commands

        private CommandBase _navigateToSupportEmailCommand = null;
        /// <summary>
        /// Command to initiate sending an email to support with device info.
        /// </summary>
        public CommandBase NavigateToSupportEmailCommand
        {
            get { return _navigateToSupportEmailCommand ?? (_navigateToSupportEmailCommand = new GenericCommand("NavigateToSupportEmailCommand", async () => await this.SendSupportEmailAsync())); }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Sends an email to support with device information.
        /// </summary>
        private async Task SendSupportEmailAsync()
        {
            var subject = string.Format(Strings.Resources.ApplicationSupportEmailSubjectTemplate, Windows.ApplicationModel.Package.Current.DisplayName, Platform.Current.AppInfo.VersionNumber);
            var file = await Platform.Current.Storage.SaveFileAsync("Application.log", Platform.Current.Logger.GenerateApplicationReport(), Windows.Storage.ApplicationData.Current.TemporaryFolder);

            var body = Strings.Resources.ApplicationSupportEmailBodyTemplate;
            #region DEBUG
            body += System.Environment.NewLine + await Platform.Current.Storage.ReadFileAsStringAsync(file.Path, Windows.Storage.ApplicationData.Current.TemporaryFolder);
            #endregion

            await Platform.Current.Navigation.SendEmailAsync(subject, body, Strings.Resources.ApplicationSupportEmailAddress, file);
        }

        #endregion
    }
}