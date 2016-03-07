using MediaAppSample.Core.Models;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;
using System.Threading;

namespace MediaAppSample.Core.ViewModels
{
    public partial class GalleryViewModel : ViewModelBase
    {
        #region Properties
        
        public override string Title
        {
            get
            {
                switch (this.GalleryView)
                {
                    case GalleryViews.Movies:
                        return "Movies"; // TODO localize
                    case GalleryViews.TV:
                        return "TV"; // TODO localize
                    default:
                        return this.GalleryView.ToString();
                }
            }
        }

        private GalleryViews _GalleryView;
        public GalleryViews GalleryView
        {
            get { return _GalleryView; }
            private set
            {
                if(this.SetProperty(ref _GalleryView, value))
                    this.NotifyPropertyChanged(() => this.Title);
            }
        }

        #endregion Properties

        #region Constructors

        public GalleryViewModel(GalleryViews galleryView = GalleryViews.Movies)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.IsRefreshVisible = true;
            this.GalleryView = galleryView;
        }

        #endregion Constructors

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override Task OnRefreshAsync(CancellationToken ct)
        {
            return base.OnRefreshAsync(ct);
        }

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return base.OnSaveStateAsync(e);
        }

        #endregion Methods
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