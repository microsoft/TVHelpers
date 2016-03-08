using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;

namespace MediaAppSample.UI.Views
{
    public abstract class GalleryMoviesViewBase : ViewBase<GalleryViewModel>
    {
    }

    public sealed partial class GalleryMoviesView : GalleryMoviesViewBase
    {
        public GalleryMoviesView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (this.ViewModel == null)
                this.SetViewModel(Platform.Current.GalleryMoviesViewModel);

            await base.OnLoadStateAsync(e);
        }
    }
}
