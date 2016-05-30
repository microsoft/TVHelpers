// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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

        #endregion

        #region Constructors

        public SettingsViewModel()
        {
            this.Title = Strings.Resources.ViewTitleSettings;

            if (DesignMode.DesignModeEnabled)
                return;

            this.ViewModels.Add(this.GeneralVM);
            this.ViewModels.Add(this.PrivacyVM);
            this.ViewModels.Add(this.TosVM);
            this.ViewModels.Add(this.AboutVM);

            if(this.DebugVM != null)
                this.ViewModels.Add(this.DebugVM);
        }

        #endregion
    }

    public partial class SettingsViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
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