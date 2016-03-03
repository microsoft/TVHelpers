using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class SettingsViewBase : ViewBase<SettingsViewModel>
    {
    }

    public sealed partial class SettingsView : SettingsViewBase
    {
        public SettingsView()
        {
            this.InitializeComponent();

            // Remove the Debug pivot if in release mode
#if !DEBUG
            pivot.Items.Remove(piDebug);
#endif
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new SettingsViewModel());

            if (e.NavigationEventArgs.Parameter is int)
            {
                SettingsViews view = (SettingsViews)e.NavigationEventArgs.Parameter;
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

                    default:
                        pivot.SelectedIndex = 0;
                        break;
                }
            }
            else
                pivot.SelectedIndex = 0;

            return base.OnLoadStateAsync(e);
        }
    }
}
