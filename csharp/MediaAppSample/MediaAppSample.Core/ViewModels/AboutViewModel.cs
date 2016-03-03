using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.Core.ViewModels
{
    public partial class AboutViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return Strings.Resources.ViewTitleAbout; }
        }

        public string TwitterAddress { get { return Strings.Resources.ApplicationSupportTwitterUsername; } }

        #endregion Properties

        #region Constructors

        public AboutViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion Constructors

        #region Methods

        #endregion Methods
    }

    public partial class AboutViewModel
    {
        public AboutViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class AboutViewModel : MediaAppSample.Core.ViewModels.AboutViewModel
    {
        public AboutViewModel()
        {
        }
    }
}