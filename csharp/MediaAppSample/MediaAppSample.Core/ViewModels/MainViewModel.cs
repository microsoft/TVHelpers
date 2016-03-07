using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets the title to be displayed on the view consuming this ViewModel.
        /// </summary>
        public override string Title
        {
            get { return Strings.Resources.ViewTitleWelcome; }
        }

        private GalleryViewModel _GalleryTvViewModel;
        public GalleryViewModel GalleryTvViewModel
        {
            get { return _GalleryTvViewModel; }
            private set { this.SetProperty(ref _GalleryTvViewModel, value); }
        }

        private GalleryViewModel _GalleryMoviesViewModel;
        public GalleryViewModel GalleryMoviesViewModel
        {
            get { return _GalleryMoviesViewModel; }
            private set { this.SetProperty(ref _GalleryMoviesViewModel, value); }
        }

        private QueueViewModel _QueueViewModel;
        public QueueViewModel QueueViewModel
        {
            get { return _QueueViewModel; }
            private set { this.SetProperty(ref _QueueViewModel, value); }
        }
        
        #endregion Properties

        #region Constructors

        public MainViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = true;
            this.IsRefreshVisible = true;

            this.GalleryMoviesViewModel = new GalleryViewModel(GalleryViews.Movies);
            this.GalleryTvViewModel = new GalleryViewModel(GalleryViews.TV);
            this.QueueViewModel = new QueueViewModel();
        }

        #endregion Constructors

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {           
                // Load data
                await this.RefreshAsync();

                // Clear primary tile
                Platform.Current.Notifications.ClearTile(this);
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {
                // SAMPLE CODE -- YOU CAN DELETE ALL THIS INSIDE THE TRY CATCH

                this.ShowBusyStatus(Strings.Resources.TextLoading, true);
                //using (var api = new ClientApi())
                //{
                //    //this.Items.Clear();
                //    this.Items.AddRange(await api.GetItems(ct));
                //}

                //// Save to cache
                //await this.SaveToCacheAsync(() => this.Items);
                
                ct.ThrowIfCancellationRequested();
                this.ShowBusyStatus("Updating voice commands and tiles...");
                await this.WaitAllAsync(
                    this.UpdateVoiceCommandsAsync(),
                    Platform.Current.Notifications.CreateOrUpdateTileAsync(this)
                    );
                this.ClearStatus();
            }
            catch (OperationCanceledException)
            {
                this.ShowTimedStatus(Strings.Resources.TextCancellationRequested, 3000);
            }
            catch (Exception ex)
            {
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
                Platform.Current.Logger.LogError(ex, "Error during MainViewModel.RefreshAsync");
            }
        }

        /// <summary>
        /// Updates the Cortana voice commands for this application
        /// </summary>
        /// <returns></returns>
        private Task UpdateVoiceCommandsAsync()
        {
            //var list = new List<string>();
            //foreach (var item in this.Items)
            //    list.Add(item.LineOne);
            //await Platform.Current.VoiceCommandManager.UpdatePhraseListAsync("CommandSet", "ItemName", list);
            return Task.CompletedTask;
        }

        #endregion Methods
    }

    public partial class MainViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public MainViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class MainViewModel : MediaAppSample.Core.ViewModels.MainViewModel
    {
        public MainViewModel()
        {
        }
    }
}