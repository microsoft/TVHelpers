using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class MediaViewBase : ViewBase<MediaViewModel>
    {
    }

    public sealed partial class MediaView : MediaViewBase
    {
        public MediaView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new MediaViewModel());

            await base.OnLoadStateAsync(e);
        }
    }
}
