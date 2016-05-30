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

        public string WelcomeMessage
        {
            get
            {
                if (Platform.Current.AuthManager.IsAuthenticated())
                    return string.Format(Strings.Account.TextWelcomeAuthenticated, Platform.Current.AuthManager.User?.FirstName);
                else
                    return Strings.Account.TextWelcomeUnauthenticated;
            }
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

            // Watch for user auth changes to update the welcome message
            Platform.Current.AuthManager.UserAuthenticatedStatusChanged += AuthenticationManager_UserAuthenticated;

            // If the view parameter contains any navigation requests, forward on to the global navigation service
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New && e.Parameter is NavigationRequest)
                Platform.Current.Navigation.NavigateTo(e.Parameter as NavigationRequest);

            return base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            Platform.Current.AuthManager.UserAuthenticatedStatusChanged -= AuthenticationManager_UserAuthenticated;
            return base.OnSaveStateAsync(e);
        }

        private void AuthenticationManager_UserAuthenticated(object sender, bool e)
        {
            // Subscribes to user authentication changed event from AuthorizationManager
            this.InvokeOnUIThread(() =>
            {
                this.NotifyPropertyChanged(() => this.WelcomeMessage);
            });
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