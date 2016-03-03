using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class GalleryViewBase : ViewBase<GalleryViewModel>
    {
    }

    public sealed partial class GalleryView : GalleryViewBase
    {
        public GalleryView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new GalleryViewModel());

            await base.OnLoadStateAsync(e);
        }
    }
}
