using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Properties

        private ContentItemBase _FeaturedHero;
        public ContentItemBase FeaturedHero
        {
            get { return _FeaturedHero; }
            protected set { this.SetProperty(ref _FeaturedHero, value); }
        }

        #region Movie Properties

        private ContentItemList _moviesFeatured;
        /// <summary>
        /// Gets the list of featured movies.
        /// </summary>
        public ContentItemList MoviesFeatured
        {
            get { return _moviesFeatured; }
            private set { this.SetProperty(ref _moviesFeatured, value); }
        }

        private ContentItemList _movieNewReleases = new ContentItemList();
        /// <summary>
        /// Gets the list of new movie releases.
        /// </summary>
        public ContentItemList MovieNewReleases
        {
            get { return _movieNewReleases; }
            private set { this.SetProperty(ref _movieNewReleases, value); }
        }

        #endregion

        #region TV Properties

        private ContentItemList _tvFeatured;
        /// <summary>
        /// Gets the list of featured TV series
        /// </summary>
        public ContentItemList TvFeatured
        {
            get { return _tvFeatured; }
            private set { this.SetProperty(ref _tvFeatured, value); }
        }

        private ContentItemList _tvNewReleases;
        /// <summary>
        /// Gets the list of new release TV series
        /// </summary>
        public ContentItemList TvNewReleases
        {
            get { return _tvNewReleases; }
            private set { this.SetProperty(ref _tvNewReleases, value); }
        }

        private ContentItemList _tvInline;
        /// <summary>
        /// Gets the list of the TV episodes for the Inline section
        /// </summary>
        public ContentItemList SneakPeeks
        {
            get { return _tvInline; }
            private set { this.SetProperty(ref _tvInline, value); }
        }

        #endregion
        
        #region ViewModels

        private GalleryViewModel _GalleryTvViewModel = new GalleryViewModel();
        public GalleryViewModel GalleryTvViewModel
        {
            get { return _GalleryTvViewModel; }
            protected set { this.SetProperty(ref _GalleryTvViewModel, value); }
        }

        private GalleryViewModel _GalleryMoviesViewModel = new GalleryViewModel();
        public GalleryViewModel GalleryMoviesViewModel
        {
            get { return _GalleryMoviesViewModel; }
            protected set { this.SetProperty(ref _GalleryMoviesViewModel, value); }
        }

        private QueueViewModel _QueueViewModel = new QueueViewModel();
        public QueueViewModel QueueViewModel
        {
            get { return _QueueViewModel; }
            protected set { this.SetProperty(ref _QueueViewModel, value); }
        }

        #endregion

        #endregion Properties

        #region Constructors

        public MainViewModel()
        {
            this.Title = Strings.Resources.ViewTitleWelcome;

            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = true;
            this.IsRefreshVisible = true;
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
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);

                // Load app data areas in parallel
                await this.WaitAllAsync(
                    ct,
                    this.LoadFeaturedHeroAsync(ct),
                    this.LoadMoviesFeaturedAsync(ct),
                    this.LoadMoviesNewReleasesAsync(ct),
                    this.LoadTvFeaturedAsync(ct),
                    this.LoadTvNewReleasesAsync(ct),
                    this.LoadSneakPeeksAsync(ct),
                    this.QueueViewModel.RefreshAsync()
                    );

                ct.ThrowIfCancellationRequested();
                this.ShowBusyStatus("Updating voice commands and tiles...");
                await this.WaitAllAsync(
                    ct,
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

        private async Task LoadFeaturedHeroAsync(CancellationToken ct)
        {
            var model = await DataSource.Current.GetFeaturedHeroAsync(ct);
            if (model != null)
            {
                this.InvokeOnUIThread(() =>
                {
                    // Set the data on the UI thread to avoid cross threading issues
                    this.FeaturedHero = model;
                });
            }
        }

        private async Task LoadMoviesFeaturedAsync(CancellationToken ct)
        {
            var list = new ContentItemList(await DataSource.Current.GetMoviesFeaturedAsync(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.MoviesFeatured = list;
            });
        }

        private async Task LoadMoviesNewReleasesAsync(CancellationToken ct)
        {
            var list = new ContentItemList(await DataSource.Current.GetMoviesNewReleasesAsync(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.MovieNewReleases = list;
            });
        }

        private async Task LoadTvFeaturedAsync(CancellationToken ct)
        {
            var list = new ContentItemList(await DataSource.Current.GetTvFeaturedAsync(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.TvFeatured = list;
            });
        }

        private async Task LoadTvNewReleasesAsync(CancellationToken ct)
        {
            var list = new ContentItemList(await DataSource.Current.GetTvNewReleasesAsync(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.TvNewReleases = list;
            });
        }

        private async Task LoadSneakPeeksAsync(CancellationToken ct)
        {
            var list = new ContentItemList(await DataSource.Current.GetSneakPeeksAsync(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.SneakPeeks = list;
            });
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
            this.FeaturedHero = Data.SampleLocalData.SampleLocalDataSource.CreateAndAddItemToList<MovieModel>(1);
        }
    }
}