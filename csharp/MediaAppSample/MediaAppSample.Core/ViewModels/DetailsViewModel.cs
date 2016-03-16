using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class DetailsViewModel : ViewModelBase
    {
        #region Properties

        private ContentItemBase _Item;
        public ContentItemBase Item
        {
            get { return _Item; }
            private set { this.SetProperty(ref _Item, value); }
        }

        #endregion

        #region Constructors

        public DetailsViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.IsRefreshVisible = true;
        }

        #endregion

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
                if (this.ViewParameter is string && !this.ViewParameter.ToString().Equals(this.Item?.ID, StringComparison.CurrentCultureIgnoreCase))
                    await this.RefreshAsync();
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);
                this.Item = await DataSource.Current.GetItemAsync(this.ViewParameter.ToString(), ct);
                this.Title = this.Item?.Title;
                this.ClearStatus();
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while trying to load data for ID '{0}'", this.ViewParameter);
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
            }

            await base.OnRefreshAsync(ct);
        }

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return base.OnSaveStateAsync(e);
        }

        #endregion Methods
    }


    /// <summary>
    /// 
    /// </summary>
    public partial class DetailsViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public DetailsViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class DetailsViewModel : MediaAppSample.Core.ViewModels.DetailsViewModel
    {
        public DetailsViewModel()
        {
        }
    }
}