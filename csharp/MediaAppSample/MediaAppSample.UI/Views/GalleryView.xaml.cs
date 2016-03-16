using MediaAppSample.Core;
using MediaAppSample.Core.Models;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;

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
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (this.ViewModel == null)
            {
                if (e.Parameter is string)
                {
                    switch (e.Parameter.ToString())
                    {
                        case nameof(ItemTypes.TvSeries):
                        case nameof(ItemTypes.TvEpisode):
                            this.SetViewModel(Platform.Current.ViewModel.GalleryTvViewModel);
                            break;

                        default:
                            this.SetViewModel(Platform.Current.ViewModel.GalleryMoviesViewModel);
                            break;
                    }
                }
                else
                    this.SetViewModel(Platform.Current.ViewModel.GalleryMoviesViewModel);
            }

            await base.OnLoadStateAsync(e);
        }
    }
}
