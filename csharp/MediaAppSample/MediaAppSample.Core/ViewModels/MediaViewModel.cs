using MediaAppSample.Core.Models;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class MediaViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return "Media"; }
        }

        #endregion Properties

        #region Constructors

        public MediaViewModel()
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

    public partial class MediaViewModel
    {
        public MediaViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class MediaViewModel : MediaAppSample.Core.ViewModels.MediaViewModel
    {
        public MediaViewModel()
        {
        }
    }
}