using MediaAppSample.Core;
using MediaAppSample.Core.Models;
using MediaAppSample.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.Core.Services
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Gets the ability to navigate to different parts of an application specific to the platform currently executing.
        /// </summary>
        public NavigationManagerBase Navigation
        {
            get { return this.GetService<NavigationManagerBase>(); }
            set { this.SetService<NavigationManagerBase>(value); }
        }
    }

    /// <summary>
    /// Base class for accessing navigation services on the platform currently executing.
    /// </summary>
    public abstract partial class NavigationManagerBase : ServiceBase
    {
        #region Properties
        
        /// <summary>
        /// Gets or sets the frame inside a Window. If not set, the ParentFrame will be returned (the frame inside a Window object).
        /// </summary>
        public Frame Frame
        {
            get
            {
                var frame = Window.Current.Content as Frame;
                return frame.GetChildFrame() ?? frame;
            }
        }
        
        /// <summary>
        /// Gets access to the Frame inside a Window object.
        /// </summary>
        public Frame ParentFrame
        {
            get
            {
                return Window.Current.Content as Frame;
            }
        }

        /// <summary>
        /// Indicates if the Window currently has a child frame. Used for SplitView pages where there is a child frame.
        /// </summary>
        public bool IsChildFramePresent
        {
            get
            {
                return this.ParentFrame != this.Frame;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialization logic which is called on launch of this application.
        /// </summary>
        protected internal override void Initialize()
        {
            // Register for phone hardware back button press
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            /// Register the main window of the application
            this.RegisterCoreWindow();

            base.Initialize();
        }

        /// <summary>
        /// Serializes a parameter to string if not a primitive type so that app suspension can properly happen.
        /// </summary>
        /// <param name="obj">Parameter object to serialize.</param>
        /// <returns></returns>
        protected object SerializeParameter(object obj)
        {
            return NavigationParameterSerializer.Serialize(obj);
        }

        #region Navigation

        /// <summary>
        /// Indicates whether or not a back navigation can occur. Will also check to see if the frame contains a WebView and if the WebView can go back as well.
        /// </summary>
        /// <returns>True if a back navigation can occur else false.</returns>
        public bool CanGoBack()
        {
            if (this.Frame == null)
                return false;
            else
            {
                if (this.Frame.DataContext is WebBrowserViewModel && (this.Frame.DataContext as WebBrowserViewModel).BrowserCanGoBack())
                    return true;
                else
                    return this.Frame.CanGoBack || this.ParentFrame.CanGoBack;
            }
        }

        /// <summary>
        /// Indicates whether or not a forward navigation can occur. Will also check to see if the frame contains a WebView and if the WebView can go forward as well.
        /// </summary>
        /// <returns>True if a forward navigation can occur else false.</returns>
        public bool CanGoForward()
        {
            if (this.Frame == null)
                return false;
            else
            {
                if (this.Frame.DataContext is WebBrowserViewModel && (this.Frame.DataContext as WebBrowserViewModel).BrowserCanGoForward())
                    return true;
                else
                    return this.Frame.CanGoForward || this.ParentFrame.CanGoForward;
            }
        }

        /// <summary>
        /// Navigates back one page. Will also check to see if the frame contains a WebView and if the WebView can go back, it will perform back on that WebView instead.
        /// </summary>
        /// <returns>True if a back navigation occurred else false.</returns>
        public bool GoBack()
        {

            if (this.IsChildFramePresent && this.Frame.CanGoBack)
            {
                if (this.ViewModelAllowGoBack(this.Frame))
                {
                    this.Frame.GoBack();
                    return true;
                }
                else
                    return false;
            }

            if (this.ParentFrame.CanGoBack)
            {
                if (this.ViewModelAllowGoBack(this.ParentFrame))
                {
                    this.ParentFrame.GoBack();
                    return true;
                }
                else
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Navigates forward one page. Will also check to see if the frame contains a WebView and if the WebView can go forward, it will perform forward on that WebView instead.
        /// </summary>
        /// <returns>True if a forward navigation occurred else false.</returns>
        public bool GoForward()
        {
            // Check if the frame contains a WebView and if it can go forward
            if (this.Frame.DataContext is WebBrowserViewModel)
            {
                var vm = this.Frame.DataContext as WebBrowserViewModel;
                if (vm.BrowserCanGoForward())
                {
                    vm.BrowserGoForward();
                    return true;
                }
            }

            // Check the child frame to go forward
            if (this.IsChildFramePresent && this.Frame.CanGoForward)
            {
                if (this.ViewModelAllowGoForward(this.Frame))
                {
                    this.Frame.GoForward();
                    return true;
                }
                else
                    return false;
            }

            // Finally check the parent frame to go forward
            if (this.ParentFrame.CanGoForward)
            {
                if (this.ViewModelAllowGoForward(this.ParentFrame))
                {
                    this.ParentFrame.GoForward();
                    return true;
                }
                else
                    return false;
            }

            // Nothing can go forward, return false.
            return false;
        }

        /// <summary>
        /// Checks a ViewModels to see if it will allow a nagivation back.
        /// </summary>
        /// <param name="frame">Frame to check.</param>
        /// <returns>True if allowed else false.</returns>
        private bool ViewModelAllowGoBack(Frame frame)
        {
            if (frame.DataContext is ViewModelBase)
            {
                var vm = frame.DataContext as ViewModelBase;
                return !vm.OnBackNavigationRequested();
            }
            else
                return true;
        }

        /// <summary>
        /// Checks a ViewModels to see if it will allow a nagivation forward.
        /// </summary>
        /// <param name="frame">Frame to check.</param>
        /// <returns>True if allowed else false.</returns>
        private bool ViewModelAllowGoForward(Frame frame)
        {
            if (frame.DataContext is ViewModelBase)
            {
                var vm = frame.DataContext as ViewModelBase;
                return !vm.OnForwardNavigationRequested();
            }
            else
                return true;
        }

        /// <summary>
        /// Clears the navigation backstack of the window.
        /// </summary>
        public void ClearBackstack()
        {
            this.Frame?.BackStack.Clear();
            this.ParentFrame?.BackStack.Clear();
            this.UpdateTitleBarBackButton();
        }

        /// <summary>
        /// Removes the previous page in the navigation backstack.
        /// </summary>
        public void RemovePreviousPage()
        {
            if (this.IsChildFramePresent && this.Frame.BackStack.Count > 1)
            {
                this.Frame.BackStack.RemoveAt(this.Frame.BackStack.Count - 1);
                return;
            }
            if (this.ParentFrame.BackStack.Count > 1)
                this.ParentFrame.BackStack.RemoveAt(this.ParentFrame.BackStack.Count - 1);
        }

        /// <summary>
        /// Updates the navigate back button in the app window's title bar.
        /// </summary>
        public void UpdateTitleBarBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Platform.Current.Navigation.CanGoBack() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        #endregion Navigation

        #region Activation/Deactivation

        /// <summary>
        /// Registers the window with all window events.
        /// </summary>
        private void RegisterCoreWindow()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += ViewBase_BackRequested;
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
            Window.Current.CoreWindow.PointerPressed += this.CoreWindow_PointerPressed;
        }

        /// <summary>
        /// Handle activation of the event and any navigation necessary.
        /// </summary>
        /// <param name="e">Activation args.</param>
        /// <param name="rootFrame">Root frame of the app.</param>
        public void HandleActivation(IActivatedEventArgs e, Frame rootFrame)
        {
            var handled = false;

            try
            {
                Platform.Current.Analytics.Event("HandleActivation", e.Kind);

                switch (e.Kind)
                {
                    case ActivationKind.Launch:
                        var launchArg = e as LaunchActivatedEventArgs;
                        Platform.Current.Logger.Log(LogLevels.Warning, "Calling OnActivation({0})  TileID: {1}  Arguments: {2}", e?.GetType().Name, launchArg.TileId, launchArg.Arguments);
                        handled = this.OnActivation(launchArg);
                        break;

                    case ActivationKind.VoiceCommand:
                        var voiceArgs = e as VoiceCommandActivatedEventArgs;
                        Platform.Current.Logger.Log(LogLevels.Warning, "Calling OnActivation({0})", e?.GetType().Name);
                        handled = this.OnActivation(voiceArgs);
                        break;

                    case ActivationKind.ToastNotification:
                        var toastArgs = e as ToastNotificationActivatedEventArgs;
                        Platform.Current.Logger.Log(LogLevels.Warning, "Calling OnActivation({0})  Arguments: {1}", e?.GetType().Name, toastArgs.Argument);
                        handled = this.OnActivation(toastArgs);
                        break;

                    case ActivationKind.Protocol:
                        var protocolArgs = e as ProtocolActivatedEventArgs;
                        Platform.Current.Logger.Log(LogLevels.Warning, "Calling OnActivation({0})  Arguments: {1}", e?.GetType().Name, protocolArgs.Uri.ToString());
                        handled = this.OnActivation(protocolArgs);
                        break;

                    default:
                        Platform.Current.Logger.LogError(new Exception(string.Format("Can't call OnActivation({0}) as it's not implemented!", e.Kind)));
                        handled = false;
                        break;
                }

                if (handled == false || rootFrame?.Content == null)
                    Platform.Current.Navigation.Home();

                Platform.Current.Logger.Log(LogLevels.Information, "Completed Navigation.HandleActivation({0}) on RootFrame: {1} --- OnActivation Handled? {2}", e?.GetType().Name, rootFrame?.Content?.GetType().Name, handled);
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during App Navigation.HandleActivation({0}) on RootFrame: {1}", e?.GetType().Name, rootFrame?.Content?.GetType().Name);
                throw ex;
            }
        }

        /// <summary>
        /// Exits an application.
        /// </summary>
        public void Exit()
        {
            Platform.Current.Analytics.Event("ApplicationExit");
            Application.Current.Exit();
        }

        #endregion

        #region Common Pages

        public void About()
        {
            this.Settings(SettingsViews.About);
        }

        public void PrivacyPolicy()
        {
            this.Settings(SettingsViews.PrivacyPolicy);
        }

        public void TermsOfService()
        {
            this.Settings(SettingsViews.TermsOfService);
        }

        public async Task RateApplicationAsync()
        {
            Platform.Current.Analytics.Event("RateApplication");
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", global::Windows.ApplicationModel.Package.Current.Id.FamilyName)));
        }

        #endregion

        #region Email

        /// <summary>
        /// Send an e-mail.
        /// </summary>
        /// <param name="subject">Subject of the message</param>
        /// <param name="body">Body of the message</param>
        /// <param name="toRecipients">To recipients</param>
        /// <param name="ccRecipients">CC recipients</param>
        /// <param name="bccRecipients">BCC recipients</param>
        /// <param name="attachments">File attachments to the message.</param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task SendEmailAsync(string subject, string body, string[] toRecipients, string[] ccRecipients, string[] bccRecipients, params IStorageFile[] attachments)
        {
            if (toRecipients == null || toRecipients.Length == 0)
                throw new ArgumentNullException(nameof(toRecipients));

            Platform.Current.Analytics.Event("SendEmail");
            var msg = new EmailMessage();

            if (toRecipients != null)
                foreach (var address in toRecipients)
                    msg.To.Add(new EmailRecipient(address));

            if (ccRecipients != null)
                foreach (var address in ccRecipients)
                    msg.CC.Add(new EmailRecipient(address));

            if (bccRecipients != null)
                foreach (var address in bccRecipients)
                    msg.Bcc.Add(new EmailRecipient(address));

            msg.Subject = subject;
            msg.Body = body;

            if(attachments != null)
            {
                foreach(IStorageFile file in attachments)
                {
                    var stream = global::Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(file);
                    var ea = new EmailAttachment(file.Name, stream);
                    msg.Attachments.Add(ea);
                }
            }
            
            await EmailManager.ShowComposeNewEmailAsync(msg);
        }

        /// <summary>
        /// Send an e-mail.
        /// </summary>
        /// <param name="subject">Subject of the message</param>
        /// <param name="body">Body of the message</param>
        /// <param name="toRecipients">To recipients</param>
        /// <param name="attachments">File attachments to the message.</param>
        /// <returns>Awaitable task is returned.</returns>
        public Task SendEmailAsync(string subject, string body, string toRecipient, params IStorageFile[] attachments)
        {
            return this.SendEmailAsync(subject, body, new string[] { toRecipient }, null, null, attachments);
        }

        /// <summary>
        /// Send an e-mail.
        /// </summary>
        /// <param name="subject">Subject of the message</param>
        /// <param name="body">Body of the message</param>
        /// <param name="toRecipients">To recipients</param>
        /// <param name="ccRecipients">CC recipients</param>
        /// <param name="attachments">File attachments to the message.</param>
        /// <returns>Awaitable task is returned.</returns>
        public Task SendEmailAsync(string subject, string body, string[] toRecipients, string[] ccRecipients = null, params IStorageFile[] attachments)
        {
            return this.SendEmailAsync(subject, body, toRecipients, ccRecipients, null, attachments);
        }

        #endregion

        #region Web

        /// <summary>
        /// Navigates to an external web browser.
        /// </summary>
        /// <param name="webAddress">URL to navigate to.</param>
        public void NavigateToWebBrowser(string webAddress)
        {
            Platform.Current.Analytics.Event("NavigateToWebBrowser");
            this.NavigateToWeb(webAddress, true);
        }

        /// <summary>
        /// Navigates to an internal app web browser.
        /// </summary>
        /// <param name="webAddress">URL to navigate to.</param>
        public void NavigateToWebView(string webAddress)
        {
            Platform.Current.Analytics.Event("NavigateToWebView");
            this.NavigateToWeb(webAddress, false);
        }

        private void NavigateToWeb(string webAddress, bool showExternally)
        {
            if (string.IsNullOrWhiteSpace(webAddress))
                throw new ArgumentNullException(nameof(webAddress));

            webAddress = webAddress.Trim();

            // if the URL is a twitter handle, forward to Twitter.com
            if (webAddress.StartsWith("@") && webAddress.Length > 1)
                webAddress = "https://twitter.com/" + webAddress.Substring(1);

            if (showExternally)
            {
                var t = Launcher.LaunchUriAsync(new Uri(webAddress, UriKind.Absolute));
            }
            else
            {
                this.WebView(webAddress);
            }
        }

        #endregion

        #region Secondary Windows

        /// <summary>
        /// Navigates to a page specified in the navigation request object.
        /// </summary>
        /// <param name="request">Request object instance.</param>
        public void NavigateTo(NavigationRequest request)
        {
            if(request != null)
                this.Frame.Navigate(Type.GetType(request.ViewType), this.SerializeParameter(request.ViewParameter));
        }

        #endregion

        #endregion

        #region Event Handlers

        private void ViewBase_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = this.GoBack();
        }

        /// <summary>
        /// Invoked when the hardware back button is pressed. For Windows Phone only.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = this.GoBack();
        }

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations, when
        /// this page is active and occupies the entire window.  Used to detect keyboard navigation
        /// between pages even when the page itself doesn't have focus.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Only investigate further when Left, Right, or the dedicated Previous or Next keys
            // are pressed
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                || e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (
                virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right
                || virtualKey == VirtualKey.GoBack || virtualKey == VirtualKey.GoForward
                )
                )
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    e.Handled = this.GoBack();
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    e.Handled = this.GoForward();
                }
            }
        }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        /// page is active and occupies the entire window.  Used to detect browser-style next and
        /// previous mouse button clicks to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                if (backPressed)
                    e.Handled = this.GoBack();
                if (forwardPressed)
                    e.Handled = this.GoForward();
            }
        }

        #endregion
    }
}

#region Classes

internal static class NavigationParameterSerializer
{
    /// <summary>
    /// Serializes an object if its a non-primitive type.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    internal static object Serialize(object obj)
    {
        if (obj == null)
            return null;
        else if (obj.GetType().GetTypeInfo().IsEnum)
            // Convert enums to int
            return (int)obj;
        else if (TypeUtility.IsPrimitive(obj.GetType()))
            // Return primitive types as-is
            return obj;
        else
        {
            // Only serialize non-primitive types to string
            var dic = new Dictionary<string, string>();
            dic.Add("Type", obj.GetType().AssemblyQualifiedName);
            dic.Add("Parameter", Serializer.Serialize(obj, SerializerTypes.Json));
            return GeneralFunctions.CreateQuerystring(dic);
        }
    }


    /// <summary>
    /// Deserializes an object if its a string and has serialization info else returns the object as-is.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    internal static object Deserialize(object parameter)
    {
        if (parameter is string)
        {
            var p = parameter.ToString();
            if (p.StartsWith("Type="))
            {
                var dic = GeneralFunctions.ParseQuerystring(p);
                var type = Type.GetType(dic["Type"]);
                var data = dic["Parameter"];
                return Newtonsoft.Json.JsonConvert.DeserializeObject(data, type);
            }
        }

        return parameter;
    }
}

/// <summary>
/// Represents navigation instructions that can be serialized and performed at a later time.
/// </summary>
public class NavigationRequest
{
    public NavigationRequest()
    {
    }

    public NavigationRequest(Type viewType, object viewParameter = null)
    {
        this.ViewType = viewType.AssemblyQualifiedName;
        this.ViewParameter = viewParameter;
    }

    /// <summary>
    /// Full type name of a view/page that needs to be instantiated.
    /// </summary>
    public string ViewType { get; set; }

    /// <summary>
    /// Object instance to pass in as a parameter.
    /// </summary>
    public object ViewParameter { get; set; }
}

#endregion