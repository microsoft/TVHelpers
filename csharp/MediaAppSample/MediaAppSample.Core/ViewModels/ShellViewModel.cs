using MediaAppSample.Core.Models;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.Core.ViewModels
{
    public partial class ShellViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return Strings.Resources.ViewTitleWelcome; }
        }

        #endregion Properties

        #region Constructors

        public ShellViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = true;
        }

        #endregion Constructors

        #region Methods

        public override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
            }

            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New && e.Parameter is NavigationRequest)
                Platform.Current.Navigation.NavigateTo(e.Parameter as NavigationRequest);

            return base.OnLoadStateAsync(e, isFirstRun);
        }

        #endregion Methods
    }

    public partial class ShellViewModel
    {
        public ShellViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class ShellViewModel : MediaAppSample.Core.ViewModels.MainViewModel
    {
        public ShellViewModel()
        {
        }
    }
}