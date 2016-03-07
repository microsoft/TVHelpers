using MediaAppSample.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ApplicationFrame : Frame
    {
        public ApplicationFrame()
        {
            this.InitializeComponent();

            // Watch for changes to the app settings
            Platform.Current.PropertyChanged += Current_PropertyChanged;
            Platform.Current.AppSettingsRoaming.PropertyChanged += AppSettingsRoaming_PropertyChanged;

            // Set the theme on initialization of the frame
            this.UpdateUI();
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Platform.Current.AppSettingsRoaming))
                Platform.Current.AppSettingsRoaming.PropertyChanged += AppSettingsRoaming_PropertyChanged;
        }

        private async void AppSettingsRoaming_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ApplicationTheme":

                    // Update theme on the UI thread only
                    if (this.Dispatcher.HasThreadAccess)
                        this.UpdateUI();
                    else
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => this.UpdateUI());
                        break;
            }
        }

        private void UpdateUI()
        {
            try
            {
                // Update the theme when the app settings property changes
                this.RequestedTheme = (ElementTheme)Platform.Current.AppSettingsRoaming.ApplicationTheme;
            }
            catch { }
        }
    }
}