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

        private MovieModel _movieHero = null;
        /// <summary>
        /// Gets MovieModel representing the hero movie.
        /// </summary>
        public MovieModel MovieHero
        {
            get { return _movieHero; }
            private set { this.SetProperty(ref _movieHero, value); }
        }

        private ContentItemCollection<MovieModel> _moviesFeatured = new ContentItemCollection<MovieModel>();
        /// <summary>
        /// Gets the list of featured movies.
        /// </summary>
        public ContentItemCollection<MovieModel> MoviesFeatured
        {
            get { return _moviesFeatured; }
            private set { this.SetProperty(ref _moviesFeatured, value); }
        }

        private ContentItemCollection<MovieModel> _movieNewReleases = new ContentItemCollection<MovieModel>();
        /// <summary>
        /// Gets the list of new movie releases.
        /// </summary>
        public ContentItemCollection<MovieModel> MovieNewReleases
        {
            get { return _movieNewReleases; }
            private set { this.SetProperty(ref _movieNewReleases, value); }
        }

        private ContentItemCollection<MovieModel> _movieTrailers = new ContentItemCollection<MovieModel>();
        /// <summary>
        /// Gets the list of movie trailers.
        /// </summary>
        public ContentItemCollection<MovieModel> MovieTrailers
        {
            get { return _movieTrailers; }
            private set { this.SetProperty(ref _movieTrailers, value); }
        }

        #endregion

        #region TV Properties

        private TvSeriesModel _tvHero = null;
        /// <summary>
        /// Gets tv series that is the TV hero
        /// </summary>
        public TvSeriesModel TvHero
        {
            get { return _tvHero; }
            private set { this.SetProperty(ref _tvHero, value); }
        }

        private ContentItemCollection<TvSeriesModel> _tvFeatured = new ContentItemCollection<TvSeriesModel>();
        /// <summary>
        /// Gets the list of featured TV series
        /// </summary>
        public ContentItemCollection<TvSeriesModel> TvFeatured
        {
            get { return _tvFeatured; }
            private set { this.SetProperty(ref _tvFeatured, value); }
        }

        private ContentItemCollection<TvSeriesModel> _tvNewReleases = new ContentItemCollection<TvSeriesModel>();
        /// <summary>
        /// Gets the list of new release TV series
        /// </summary>
        public ContentItemCollection<TvSeriesModel> TvNewReleases
        {
            get { return _tvNewReleases; }
            private set { this.SetProperty(ref _tvNewReleases, value); }
        }

        private ContentItemCollection<TvEpisodeModel> _tvInline = new ContentItemCollection<TvEpisodeModel>();
        /// <summary>
        /// Gets the list of the TV episodes for the Inline section
        /// </summary>
        public ContentItemCollection<TvEpisodeModel> TvInline
        {
            get { return _tvInline; }
            private set { this.SetProperty(ref _tvInline, value); }
        }

        private ContentItemCollection<TvEpisodeModel> _TvEpisodes = new ContentItemCollection<TvEpisodeModel>();
        /// <summary>
        /// Gets a list of all the TV epsisodes.
        /// </summary>
        public ContentItemCollection<TvEpisodeModel> TvEpisodes
        {
            get { return _TvEpisodes; }
            private set { this.SetProperty(ref _TvEpisodes, value); }
        }

        #endregion
        
        private double _DeviceWindowHeight;
        public double DeviceWindowHeight
        {
            get { return _DeviceWindowHeight; }
            set { this.SetProperty(ref _DeviceWindowHeight, value); }
        }
        
        private double _DeviceWindowWidth;
        public double DeviceWindowWidth
        {
            get { return _DeviceWindowWidth; }
            set { this.SetProperty(ref _DeviceWindowWidth, value); }
        }

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
                    this.LoadMovieHeroAsync(ct),
                    this.LoadMoviesFeaturedAsync(ct),
                    this.LoadMoviesNewReleasesAsync(ct),
                    this.LoadMovieTrailersAsync(ct),
                    this.LoadTvHeroAsync(ct),
                    this.LoadTvFeaturedAsync(ct),
                    this.LoadTvNewReleasesAsync(ct),
                    this.LoadTvInline(ct),
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
            var model = await DataSource.Current.GetFeaturedHero(ct);
            if (model != null)
            {
                this.InvokeOnUIThread(() =>
                {
                    // Set the data on the UI thread to avoid cross threading issues
                    this.FeaturedHero = model;
                });
            }
        }


        private async Task LoadMovieTrailersAsync(CancellationToken ct)
        {
            var list = new ContentItemCollection<MovieModel>(await DataSource.Current.GetMoviesTrailers(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues 
                this.MovieTrailers = list;
            });
        }

        private async Task LoadMovieHeroAsync(CancellationToken ct)
        {
            var model = await DataSource.Current.GetMovieHero(ct);
            if (model != null)
            {
                this.InvokeOnUIThread(() =>
                {
                    // Set the data on the UI thread to avoid cross threading issues
                    this.MovieHero = model;
                });
            }
        }

        private async Task LoadMoviesFeaturedAsync(CancellationToken ct)
        {
            var list = new ContentItemCollection<MovieModel>(await DataSource.Current.GetMoviesFeatured(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.MoviesFeatured = list;
            });
        }

        private async Task LoadTvFeaturedAsync(CancellationToken ct)
        {
            var list = new ContentItemCollection<TvSeriesModel>(await DataSource.Current.GetTvFeatured(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.TvFeatured = list;
            });
        }

        private async Task LoadMoviesNewReleasesAsync(CancellationToken ct)
        {
            var list = new ContentItemCollection<MovieModel>(await DataSource.Current.GetMoviesNewReleases(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.MovieNewReleases = list;
            });
        }

        private async Task LoadTvInline(CancellationToken ct)
        {
            var list = new ContentItemCollection<TvEpisodeModel>(await DataSource.Current.GetTvInline(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.TvInline = list;
            });
        }

        private async Task LoadTvHeroAsync(CancellationToken ct)
        {
            var model = await DataSource.Current.GetTvHero(ct);
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.TvHero = model;
            });
        }

        private async Task LoadTvNewReleasesAsync(CancellationToken ct)
        {
            var list = new ContentItemCollection<TvSeriesModel>(await DataSource.Current.GetTvNewReleases(ct));
            this.InvokeOnUIThread(() =>
            {
                // Set the data on the UI thread to avoid cross threading issues
                this.TvNewReleases = list;
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
            this.DeviceWindowHeight = 600;
            this.DeviceWindowWidth = 800;
            this.FeaturedHero = Data.SampleLocalData.SampleLocalDataSource.CreateAndAddItemToList<MovieModel>(1);
        }
    }
}