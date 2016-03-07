using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class ItemViewBase : ViewBase<ItemViewModel>
    {
    }

    public sealed partial class ItemView : ItemViewBase
    {
        public ItemView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new ItemViewModel());

            return base.OnLoadStateAsync(e);
        }
    }
}
