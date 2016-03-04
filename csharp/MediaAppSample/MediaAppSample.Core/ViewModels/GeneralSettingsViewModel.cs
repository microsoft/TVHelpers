using MediaAppSample.Core.Commands;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.System;

namespace MediaAppSample.Core.ViewModels
{
    public partial class GeneralSettingsViewModel : ViewModelBase
    {
        #region Properties
        
        public override string Title
        {
            get
            {
                return Strings.Resources.TextTitleGeneral;
            }
        }
        
        private bool _ShowWindowsHelloSetupButton = false;
        public bool ShowWindowsHelloSetupButton
        {
            get { return _ShowWindowsHelloSetupButton; }
            private set { this.SetProperty(ref _ShowWindowsHelloSetupButton, value); }
        }

        private bool _ShowWindowsHelloRemoveButton;
        public bool ShowWindowsHelloRemoveButton
        {
            get { return _ShowWindowsHelloRemoveButton; }
            private set { this.SetProperty(ref _ShowWindowsHelloRemoveButton, value); }
        }

        public CommandBase RemoveWindowsHelloCommand { get; private set; }

        private bool _WindowsHelloIsBusy = false;
        public bool WindowsHelloIsBusy
        {
            get { return _WindowsHelloIsBusy; }
            private set { this.SetProperty(ref _WindowsHelloIsBusy, value); }
        }

        private string _BackgroundTasksStatus;
        public string BackgroundTasksStatus
        {
            get { return _BackgroundTasksStatus; }
            private set { this.SetProperty(ref _BackgroundTasksStatus, value); }
        }

        private CommandBase _ManageBackgroundTasksCommand;
        public CommandBase ManageBackgroundTasksCommand
        {
            get { return _ManageBackgroundTasksCommand; }
            private set { this.SetProperty(ref _ManageBackgroundTasksCommand, value); }
        }

        private string _LocationServicesStatus;
        public string LocationServicesStatus
        {
            get { return _LocationServicesStatus; }
            private set { this.SetProperty(ref _LocationServicesStatus, value); }
        }
        
        private CommandBase _ManageLocationServicesCommand;
        public CommandBase ManageLocationServicesCommand
        {
            get { return _ManageLocationServicesCommand; }
            private set { this.SetProperty(ref _ManageLocationServicesCommand, value); }
        }

        #endregion

        #region Constructors

        public GeneralSettingsViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;
            
            Platform.Current.AppSettingsLocal.PropertyChanged += AppSettingsLocal_PropertyChanged;
            Platform.Current.AppSettingsRoaming.PropertyChanged += AppSettingsRoaming_PropertyChanged;

            // Deep linking to Settings app sections: https://msdn.microsoft.com/en-us/library/windows/apps/mt228342.aspx
            this.ManageLocationServicesCommand = new GenericCommand("ManageLocationServicesCommand", async () => await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location")));
            this.ManageBackgroundTasksCommand = new GenericCommand("ManageBackgroundTasksCommand", async () => await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-backgroundapps")));
        }

        ~GeneralSettingsViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;
            Platform.Current.AppSettingsLocal.PropertyChanged -= AppSettingsLocal_PropertyChanged;
            Platform.Current.AppSettingsRoaming.PropertyChanged -= AppSettingsRoaming_PropertyChanged;
        }

        #endregion

        #region Methods
         
        public override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if(this.View != null)
                this.View.GotFocus += View_GotFocus;

            if (isFirstRun)
            {
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        private void View_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var t1 = this.RefreshLocationStatus();
            var t2 = this.RefreshBackgroundTasksStatus();
        }

        private async Task RefreshLocationStatus()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            this.LocationServicesStatus = accessStatus == GeolocationAccessStatus.Denied ?
                "Location access has not been enabled for this app. Use the manage button and ensure this app has been enabled to use location services." : string.Empty;
        }

        private async Task RefreshBackgroundTasksStatus()
        {
            var allowed = Platform.Current.BackgroundTasksManager.CheckIfAllowed();

            this.BackgroundTasksStatus = !allowed ?
                "Background tasks have not been enabled for this app. Use the manage button and ensure this app has been enabled to run in the background." : string.Empty;

            if (!Platform.Current.BackgroundTasksManager.AreTasksRegistered && allowed)
                await Platform.Current.BackgroundTasksManager.RegisterAllAsync();
        }
        

        public override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            try
            {
                if (this.View != null)
                    this.View.GotFocus -= View_GotFocus;
                Platform.Current.SaveSettings();
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during GeneralSettingsViewModel.OnSaveStateAsync");
                throw;
            }
            return base.OnSaveStateAsync(e);
        }

        #endregion

        #region Event Handlers

        private void AppSettingsLocal_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void AppSettingsRoaming_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        #endregion
    }

    public partial class GeneralSettingsViewModel
    {
        public GeneralSettingsViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class GeneralSettingsViewModel : MediaAppSample.Core.ViewModels.GeneralSettingsViewModel
    {
        public GeneralSettingsViewModel()
        {
        }
    }
}