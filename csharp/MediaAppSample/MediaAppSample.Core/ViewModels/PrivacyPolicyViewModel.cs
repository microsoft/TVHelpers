using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class PrivacyPolicyViewModel : WebBrowserViewModel
    {
        #region Properties

        public override string Title
        {
            get { return Strings.Resources.ViewTitlePrivacyPolicy; }
        }

        #endregion Properties

        #region Constructors

        public PrivacyPolicyViewModel(bool showNavigation = true) : base(showNavigation)
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion Constructors

        #region Methods

        public override void InitialNavigation()
        {
            this.NavigateTo("http://go.microsoft.com/fwlink/?LinkId=521839");
        }

        #endregion Methods
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class PrivacyPolicyViewModel : MediaAppSample.Core.ViewModels.PrivacyPolicyViewModel
    {
        public PrivacyPolicyViewModel()
        {
        }
    }
}