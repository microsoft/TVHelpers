using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;

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
            if (this.ViewModel == null)
                this.SetViewModel(Platform.Current.QueueViewModel);

            await base.OnLoadStateAsync(e);
        }
    }
}
