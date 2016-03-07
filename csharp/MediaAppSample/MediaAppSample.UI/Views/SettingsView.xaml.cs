using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System;
using System.Threading.Tasks;

namespace MediaAppSample.UI.Views
{
    public abstract class SettingsViewBase : ViewBase<SettingsViewModel>
    {
    }

    public sealed partial class SettingsView : SettingsViewBase
    {
        private const string LAST_SELECTED_INDEX = "LastSelectedIndex";

        public SettingsView()
        {
            this.InitializeComponent();

            // Remove the Debug pivot if in release mode
#if !DEBUG
            pivot.Items.Remove(piDebug);
#endif
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (this.ViewModel == null)
                this.SetViewModel(new SettingsViewModel());

            try
            {
                if (e.PageState.ContainsKey(LAST_SELECTED_INDEX))
                {
                    // Restore the last viewed pivot from page state
                    pivot.SelectedIndex = (int)e.PageState[LAST_SELECTED_INDEX];
                }
                else
                {
                    // Use the page parameter to determine the starting pivot
                    int selected = e.NavigationEventArgs.Parameter is int ? (int)e.NavigationEventArgs.Parameter : 0;
                    SettingsViews view = (SettingsViews)selected;
                    switch (view)
                    {
                        case SettingsViews.PrivacyPolicy:
                            pivot.SelectedIndex = 2;
                            break;

                        case SettingsViews.TermsOfService:
                            pivot.SelectedIndex = 3;
                            break;

                        case SettingsViews.About:
                            pivot.SelectedIndex = 4;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error with determining which Pivot to set to SelectedIndex.");
            }

            await base.OnLoadStateAsync(e);
        }

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            // Save current pivot to page state
            e.PageState[LAST_SELECTED_INDEX] = pivot.SelectedIndex;

            return base.OnSaveStateAsync(e);
        }
    }
}
