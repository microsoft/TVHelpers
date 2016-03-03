using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class WelcomeViewBase : ViewBase<WelcomeViewModel>
    {
    }

    public sealed partial class WelcomeView : WelcomeViewBase
    {
        public WelcomeView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new WelcomeViewModel());

            await base.OnLoadStateAsync(e);
        }
    }
}
