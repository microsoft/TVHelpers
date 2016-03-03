using MediaAppSample.Core.Models;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class QueueViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return "Queue"; }
        }

        #endregion Properties

        #region Constructors

        public QueueViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion Constructors

        #region Methods

        public override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
            }

            return base.OnLoadStateAsync(e, isFirstRun);
        }

        public override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return base.OnSaveStateAsync(e);
        }

        #endregion Methods
    }

    public partial class QueueViewModel
    {
        public QueueViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class QueueViewModel : MediaAppSample.Core.ViewModels.QueueViewModel
    {
        public QueueViewModel()
        {
        }
    }
}