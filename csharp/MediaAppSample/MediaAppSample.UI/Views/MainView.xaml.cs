using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class MainViewBase : ViewBase<MainViewModel>
    {
    }

    public sealed partial class MainView : MainViewBase
    {
        public MainView()
        {
            this.InitializeComponent();
            this.SizeChanged += MainView_SizeChanged;
        }

        protected override Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(Platform.Current.ViewModel);

            return base.OnLoadStateAsync(e);
        }

        private void MainView_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            Platform.Current.ViewModel.DeviceWindowHeight = e.NewSize.Height - 50;
        }
    }
}
