using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class DetailsViewBase : ViewBase<DetailsViewModel>
    {
    }

    public sealed partial class DetailsView : DetailsViewBase
    {
        public DetailsView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new DetailsViewModel());

            await base.OnLoadStateAsync(e);
        }
    }
}
