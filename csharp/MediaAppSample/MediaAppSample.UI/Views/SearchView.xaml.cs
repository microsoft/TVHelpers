using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class SearchViewBase : ViewBase<SearchViewModel>
    {
    }

    public sealed partial class SearchView : SearchViewBase
    {
        public SearchView()
        {
            this.InitializeComponent();
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new SearchViewModel());

            return base.OnLoadStateAsync(e);
        }
    }
}
