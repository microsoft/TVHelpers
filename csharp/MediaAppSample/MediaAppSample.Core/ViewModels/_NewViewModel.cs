using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class NewViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return Strings.Resources.ApplicationName; }
        }

        #endregion

        #region Constructors

        public NewViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;
        }

        #endregion        
        
        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
                await this.RefreshAsync();
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);

                // DO WORK HERE
                await Task.CompletedTask;

                ct.ThrowIfCancellationRequested();
                this.ClearStatus();
            }
            catch (OperationCanceledException)
            {
                this.ShowTimedStatus(Strings.Resources.TextCancellationRequested, 3000);
            }
            catch (Exception ex)
            {
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
                Platform.Current.Logger.LogError(ex, "Error during RefreshAsync");
            }
        }

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return base.OnSaveStateAsync(e);
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class NewViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public NewViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class NewViewModel : MediaAppSample.Core.ViewModels.NewViewModel
    {
        public NewViewModel()
        {
        }
    }
}