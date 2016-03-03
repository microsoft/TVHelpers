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
        protected abstract Frame CreateFrame();

        protected abstract bool OnActivation(LaunchActivatedEventArgs e);

        protected abstract bool OnActivation(ToastNotificationActivatedEventArgs e);

        protected abstract bool OnActivation(VoiceCommandActivatedEventArgs e);

        protected abstract bool OnActivation(ProtocolActivatedEventArgs e);

        protected abstract void NavigateToSecondaryWindow(NavigationRequest request);

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

        public abstract void Gallery(object parameter);

        public abstract void Media(object parameter);
    }
}