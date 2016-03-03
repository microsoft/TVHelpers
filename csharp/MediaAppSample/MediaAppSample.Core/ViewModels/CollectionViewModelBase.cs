using MediaAppSample.Core.Models;
using System.Threading.Tasks;
using Windows.ApplicationModel;

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
        
        private LoadStateEventArgs _loadState;
        public override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            _loadState = e;
            await base.OnLoadStateAsync(e, isFirstRun);
        }

        public override async Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            // Call load on each sub-ViewModel in this collection when displaying this page
            foreach(var vm in this.ViewModels)
                if (vm != null && vm.IsInitialized)
                    await vm.SaveStateAsync(e);

            await base.OnSaveStateAsync(e);
        }

        /// <summary>
        /// Sets the current ViewModel that is active and visible.
        /// </summary>
        /// <param name="vm"></param>
        public async Task SetCurrentAsync(ViewModelBase vm)
        {
            Platform.Current.Logger.Log(LogLevels.Debug, "CollectionViewModelBase.SetCurrent to {0}", vm);

            if (this.CurrentViewModel != null)
            {
                this.CurrentViewModel.PropertyChanged -= CurrentVM_PropertyChanged;
                this.ClearStatus();
            }

            this.CurrentViewModel = vm;

            if (this.CurrentViewModel != null)
            {
                this.CurrentViewModel.PropertyChanged += CurrentVM_PropertyChanged;
                this.CopyStatus(this.CurrentViewModel);
            }

            // Call load state on the sub-viewmodel once its requested to be set to curent.
            await this.CurrentViewModel.LoadStateAsync(this.View, _loadState);

            this.Commands.NavigateGoBackCommand.RaiseCanExecuteChanged();
            this.Commands.NavigateGoForwardCommand.RaiseCanExecuteChanged();
        }

        public override bool OnBackNavigationRequested()
        {
            // See if the current view model allows back navigation
            if (this.CurrentViewModel != null)
                return this.CurrentViewModel.OnBackNavigationRequested();
            else
                return base.OnBackNavigationRequested();
        }

        public override bool OnForwardNavigationRequested()
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
