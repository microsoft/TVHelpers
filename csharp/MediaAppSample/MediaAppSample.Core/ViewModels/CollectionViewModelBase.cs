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

using MediaAppSample.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.Core.ViewModels
{
    /// <summary>
    /// ViewModelBase for views that need to display multiple separate sub-views which might have their own ViewModel instances. This 
    /// CollectionViewModelBase can contain multiple ViewModels and set a current view model so that the frame can show appropriate 
    /// status data specific to the current view model.
    /// </summary>
    public abstract class CollectionViewModelBase : ViewModelBase
    {
        #region Properties

        private bool _initialized = false;
        private LoadStateEventArgs _loadState;

        private ViewModelBase _CurrentViewModel;
        /// <summary>
        /// Gets access to the current selected view model.
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get { return _CurrentViewModel; }
            private set { this.SetProperty(ref _CurrentViewModel, value); }
        }

        private ModelList<ViewModelBase> _ViewModels = new ModelList<ViewModelBase>();
        /// <summary>
        /// Gets access to the collection of sub-viewmodels.
        /// </summary>
        protected ModelList<ViewModelBase> ViewModels
        {
            get { return _ViewModels; }
            private set { this.SetProperty(ref _ViewModels, value); }
        }

        #endregion

        #region Constructors

        public CollectionViewModelBase()
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            _loadState = e;
            await base.OnLoadStateAsync(e, isFirstRun);

            // Subsequent navigations to a page that uses a CollectionViewModelBase might already have an initialized page and thus not fire the appropriate 
            // event on the already loaded first view. This check is to manually pass the load state event to the current in focuse ViewModel.
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New && e.IsViewInitialized)
                await this.SetCurrentAsync(this.CurrentViewModel ?? this.ViewModels.FirstOrDefault());
        }

        protected override async Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            try
            {
                // Call load on each sub-ViewModel in this collection when displaying this page
                foreach (var vm in this.ViewModels)
                    if (vm != null && vm.IsInitialized)
                        await vm.SaveStateAsync(e);
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during CollectionViewModelBase.OnSaveStateAsync calling each individual child ViewModel.SaveStateAsync");
                throw;
            }

            await base.OnSaveStateAsync(e);
        }

        /// <summary>
        /// Sets the current ViewModel that is active and visible.
        /// </summary>
        /// <param name="vm"></param>
        public async Task SetCurrentAsync(ViewModelBase vm)
        {
            if (vm == this)
                return;

            if (this.CurrentViewModel != null)
            {
                this.CurrentViewModel.PropertyChanged -= CurrentVM_PropertyChanged;
                this.ClearStatus();
            }

            this.CurrentViewModel = vm;

            if (this.CurrentViewModel != null)
            {
                Platform.Current.Logger.Log(LogLevels.Debug, "CollectionViewModelBase.SetCurrent to {0}", vm);
                this.CurrentViewModel.PropertyChanged += CurrentVM_PropertyChanged;
                this.CopyStatus(this.CurrentViewModel);

                // Call load state on the sub-viewmodel once its requested to be set to curent.
                await this.CurrentViewModel.LoadStateAsync(this.View, _loadState);
            }

            // Update global navigation
            Platform.Current.Navigation.NavigateGoBackCommand.RaiseCanExecuteChanged();
            Platform.Current.Navigation.NavigateGoForwardCommand.RaiseCanExecuteChanged();
        }

        protected internal override bool OnBackNavigationRequested()
        {
            // See if the current view model allows back navigation
            if (this.CurrentViewModel != null && this != this.CurrentViewModel)
                return this.CurrentViewModel.OnBackNavigationRequested();
            else
                return base.OnBackNavigationRequested();
        }

        protected internal override bool OnForwardNavigationRequested()
        {
            // See if the current view model allows forward navigation
            if (this.CurrentViewModel != null)
                return this.CurrentViewModel.OnForwardNavigationRequested();
            else
                return base.OnForwardNavigationRequested();
        }

        #endregion

        #region Event Handlers

        private void CurrentVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Pass the current viewmodel's status properties on to the parent collection view models' status properties.
            switch (e.PropertyName)
            {
                case "StatusIsBlocking":
                case "StatusIsBusy":
                case "StatusProgressValue":
                case "StatusText":
                    this.CopyStatus(this.CurrentViewModel);
                    break;
            }
        }

        #endregion
    }
}
