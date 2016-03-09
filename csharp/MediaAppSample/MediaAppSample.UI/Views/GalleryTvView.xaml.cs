using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;

namespace MediaAppSample.UI.Views
{
    public abstract class GalleryTvViewBase : ViewBase<GalleryViewModel>
    {
    }

    public sealed partial class GalleryTvView : GalleryTvViewBase
    {
        public GalleryTvView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (this.ViewModel == null)
                this.SetViewModel(Platform.Current.ViewModel.GalleryTvViewModel);

            await base.OnLoadStateAsync(e);
        }
    }
}
