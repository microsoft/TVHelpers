using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class GalleryViewModel : ViewModelBase
    {
        #region Properties
        
        public override string Title
        {
            get
            {
                switch (this.GalleryType)
                {
                    case GalleryTypes.Movies:
                        return "Movies"; // TODO localize
                    case GalleryTypes.TV:
                        return "TV"; // TODO localize
                    default:
                        return this.GalleryType.ToString();
                }
            }
        }

        private GalleryTypes _GalleryType;
        public GalleryTypes GalleryType
        {
            get { return _GalleryType; }
            private set
            {
                if(this.SetProperty(ref _GalleryType, value))
                    this.NotifyPropertyChanged(() => this.Title);
            }
        }

        private ContentItemList _Items = new ContentItemList();
        public ContentItemList Items
        {
            get
            {
                if (!string.IsNullOrEmpty(this.SelectedGenre))
                    return new ContentItemList(_Items.Where(w => w.Genre.Equals(this.SelectedGenre, StringComparison.CurrentCultureIgnoreCase)));
                else
                    return _Items;
            }
            private set { this.SetProperty(ref _Items, value); }
        }

        private string[] _SortOptions;
        public string[] SortOptions
        {
            get { return _SortOptions; }
            private set { this.SetProperty(ref _SortOptions, value); }
        }

        private string[] _GenreOptions;
        public string[] GenreOptions
        {
            get { return _GenreOptions; }
            private set { this.SetProperty(ref _GenreOptions, value); }
        }

        private string _SelectedGenre;
        public string SelectedGenre
        {
            get { return _SelectedGenre; }
            set
            {
                if (this.SetProperty(ref _SelectedGenre, value))
                    this.NotifyPropertyChanged(() => this.Items);
            }
        }

        private string _SelectedSort;
        public string SelectedSort
        {
            get { return _SelectedSort; }
            set
            {
                if (this.SetProperty(ref _SelectedSort, value))
                    this.SortItems();
            }
        }

        #endregion Properties

        #region Constructors

        public GalleryViewModel(GalleryTypes galleryView = GalleryTypes.Movies)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.IsRefreshVisible = true;
            this.GalleryType = galleryView;

            this.SortOptions = new string[] { "A-Z", "Year" };
        }

        #endregion Constructors

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
                
                var data = await DataSource.Current.GetMoviesAsync(ct);
                this.Items.Clear();
                this.Items.AddRange(data);
                this.GenreOptions = this.Items.Select(s => s.Genre).Distinct().ToArray();

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

        private async void SortItems()
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextSorting);
                await this.Items.SortAsync("Year");
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to sort gallery.");
            }
            finally
            {
                this.ClearStatus();
            }
        }

        #endregion
    }

    public partial class GalleryViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public GalleryViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class GalleryViewModel : MediaAppSample.Core.ViewModels.GalleryViewModel
    {
        public GalleryViewModel()
        {
        }
    }
}