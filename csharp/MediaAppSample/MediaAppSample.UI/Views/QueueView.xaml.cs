using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.UI.Views
{
    public abstract class QueueViewBase : ViewBase<QueueViewModel>
    {
    }

    public sealed partial class QueueView : QueueViewBase
    {
        public QueueView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new QueueViewModel());

            await base.OnLoadStateAsync(e);
        }
    }
}
