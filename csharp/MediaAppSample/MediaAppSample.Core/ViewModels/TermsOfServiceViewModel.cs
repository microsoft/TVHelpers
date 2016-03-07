using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class TermsOfServiceViewModel : WebBrowserViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return Strings.Resources.ViewTitleTermsOfService; }
        }

        #endregion

        #region Constructors

        public TermsOfServiceViewModel(bool showNavigation = true) : base(showNavigation)
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion

        #region Methods

        public override void InitialNavigation()
        {
            this.NavigateTo("http://go.microsoft.com/fwlink/?LinkID=206977");
        }

        #endregion
    }
}