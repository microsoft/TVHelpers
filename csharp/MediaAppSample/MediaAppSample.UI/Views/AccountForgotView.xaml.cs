using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class AccountForgetViewBase : ViewBase<AccountForgotViewModel>
    {
    }

    public sealed partial class AccountForgotView : AccountForgetViewBase
    {
        public AccountForgotView()
        {
            this.InitializeComponent();
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new AccountForgotViewModel());

            return base.OnLoadStateAsync(e);
        }
    }
}
