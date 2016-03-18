using MediaAppSample.Core.Commands;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.Core.ViewModels
{
    public partial class ShellViewModel : ViewModelBase
    {
        #region Properties

        private bool _IsMenuOpen;
        public bool IsMenuOpen
        {
            get { return _IsMenuOpen; }
            set { this.SetProperty(ref _IsMenuOpen, value); }
        }

        private ICommand _ToggleMenuCommand = null;
        public ICommand ToggleMenuCommand
        {
            get { return _ToggleMenuCommand ?? (_ToggleMenuCommand = new GenericCommand("ToggleMenuCommand", () => this.IsMenuOpen = !this.IsMenuOpen)); }
        }

        #endregion Properties

        #region Constructors

        public ShellViewModel()
        {
            this.Title = Strings.Resources.ViewTitleWelcome;

            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion Constructors

        #region Methods

        protected override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
            }

            // If the view parameter contains any navigation requests, forward on to the global navigation service
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New && e.Parameter is NavigationRequest)
                Platform.Current.Navigation.NavigateTo(e.Parameter as NavigationRequest);

            return base.OnLoadStateAsync(e, isFirstRun);
        }

        #endregion Methods
    }

    public partial class ShellViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
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