using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class TermsOfServiceViewModel : WebBrowserViewModel
    {
        #region Properties

        public override string Title
        {
            get { return Strings.Resources.ViewTitleTermsOfService; }
        }

        #endregion Properties

        #region Constructors

        public TermsOfServiceViewModel(bool showNavigation = true) : base(showNavigation)
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion Constructors

        #region Methods

        public override void InitialNavigation()
        {
            this.NavigateTo("http://go.microsoft.com/fwlink/?LinkID=206977");
        }

        #endregion Methods
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class TermsOfServiceViewModel : MediaAppSample.Core.ViewModels.TermsOfServiceViewModel
    {
        public TermsOfServiceViewModel()
        {
        }
    }
}