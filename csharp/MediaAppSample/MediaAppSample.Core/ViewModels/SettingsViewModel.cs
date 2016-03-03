using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public enum SettingsViews
    {
        General,
        TermsOfService,
        PrivacyPolicy,
        About
    }

    public partial class SettingsViewModel : CollectionViewModelBase
    {
        #region Properties
        
        public override string Title
        {
            get
            {
                return Strings.Resources.ViewTitleSettings;
            }
        }

        private GeneralSettingsViewModel _GeneralSettingsViewModel = new GeneralSettingsViewModel();
        public GeneralSettingsViewModel GeneralVM
        {
            get { return _GeneralSettingsViewModel; }
            private set { this.SetProperty(ref _GeneralSettingsViewModel, value); }
        }

        private PrivacyPolicyViewModel _PrivacyPolicyViewModel = new PrivacyPolicyViewModel(false);
        public PrivacyPolicyViewModel PrivacyVM
        {
            get { return _PrivacyPolicyViewModel; }
            private set { this.SetProperty(ref _PrivacyPolicyViewModel, value); }
        }

        private TermsOfServiceViewModel _TermsOfServiceViewModel = new TermsOfServiceViewModel(false);
        public TermsOfServiceViewModel TosVM
        {
            get { return _TermsOfServiceViewModel; }
            private set { this.SetProperty(ref _TermsOfServiceViewModel, value); }
        }

        private AboutViewModel _AboutVM = new AboutViewModel();
        public AboutViewModel AboutVM
        {
            get { return _AboutVM; }
            private set { this.SetProperty(ref _AboutVM, value); }
        }

#if !DEBUG
        private DebuggingViewModel _DebuggingViewModel = null;
#else
        private DebuggingViewModel _DebuggingViewModel = new DebuggingViewModel();
#endif
        public DebuggingViewModel DebugVM
        {
            get { return _DebuggingViewModel; }
            private set { this.SetProperty(ref _DebuggingViewModel, value); }
        }


        private ItemViewModel _Item1 = new ItemViewModel(1);
        public ItemViewModel Item1
        {
            get { return _Item1; }
            private set { this.SetProperty(ref _Item1, value); }
        }


        private ItemViewModel _Item2 = new ItemViewModel(2);
        public ItemViewModel Item2
        {
            get { return _Item2; }
            private set { this.SetProperty(ref _Item2, value); }
        }

        #endregion

        #region Constructors

        public SettingsViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.ViewModels.Add(this.GeneralVM);
            this.ViewModels.Add(this.PrivacyVM);
            this.ViewModels.Add(this.TosVM);
            this.ViewModels.Add(this.AboutVM);

            if(this.DebugVM != null)
                this.ViewModels.Add(this.DebugVM);

            this.ViewModels.Add(this.Item1);
            this.ViewModels.Add(this.Item2);
        }

        #endregion
    }

    public partial class SettingsViewModel
    {
        public SettingsViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class SettingsViewModel : MediaAppSample.Core.ViewModels.SettingsViewModel
    {
        public SettingsViewModel()
        {
        }
    }
}