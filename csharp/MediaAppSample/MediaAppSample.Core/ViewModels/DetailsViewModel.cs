using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
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
            private set
            {
                if (this.SetProperty(ref _Item, value))
                    this.NotifyPropertyChanged(() => this.RelatedTitle);
            }
        }

        private NotifyTaskCompletion<IEnumerable<ContentItemBase>> _RelatedItemsTask;
        public NotifyTaskCompletion<IEnumerable<ContentItemBase>> RelatedItemsTask
        {
            get { return _RelatedItemsTask; }
            private set { this.SetProperty(ref _RelatedItemsTask, value); }
        }

        private NotifyTaskCompletion<IEnumerable<ContentItemBase>> _TrailerItemsTask;
        public NotifyTaskCompletion<IEnumerable<ContentItemBase>> TrailerItemsTask
        {
            get { return _TrailerItemsTask; }
            private set { this.SetProperty(ref _TrailerItemsTask, value); }
        }

        private NotifyTaskCompletion<IEnumerable<RatingModel>> _RatingsTask;
        public NotifyTaskCompletion<IEnumerable<RatingModel>> RatingsTask
        {
            get { return _RatingsTask; }
            private set { this.SetProperty(ref _RatingsTask, value); }
        }

        private NotifyTaskCompletion<IEnumerable<ReviewModel>> _ReviewsTask;
        public NotifyTaskCompletion<IEnumerable<ReviewModel>> ReviewsTask
        {
            get { return _ReviewsTask; }
            private set { this.SetProperty(ref _ReviewsTask, value); }
        }
        
        private NotifyTaskCompletion<IEnumerable<PersonModel>> _CastsTask;
        public NotifyTaskCompletion<IEnumerable<PersonModel>> CastsTask
        {
            get { return _CastsTask; }
            private set { this.SetProperty(ref _CastsTask, value); }
        }
        
        private NotifyTaskCompletion<IEnumerable<PersonModel>> _CreatorsTask;
        public NotifyTaskCompletion<IEnumerable<PersonModel>> CreatorsTask
        {
            get { return _CreatorsTask; }
            private set { this.SetProperty(ref _CreatorsTask, value); }
        }

        
        public string RelatedTitle
        {
            get
            {
                if (this.Item == null)
                    return Strings.Resources.TextLoading;

                switch (this.Item.ItemType)
                {
                    case ItemTypes.Movie:
                        return Strings.Resources.TextRelatedMovies;

                    case ItemTypes.TvSeries:
                    case ItemTypes.TvEpisode:
                        return Strings.Resources.TextRelatedTV;

                    default:
                        throw new NotImplementedException();
                }
            }
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

                string id = this.ViewParameter.ToString();

                this.RelatedItemsTask = new NotifyTaskCompletion<IEnumerable<ContentItemBase>>(DataSource.Current.GetRelatedAsync(id, ct));
                this.TrailerItemsTask = new NotifyTaskCompletion<IEnumerable<ContentItemBase>>(DataSource.Current.GetTrailersAsync(id, ct));
                this.CreatorsTask = new NotifyTaskCompletion<IEnumerable<PersonModel>>(DataSource.Current.GetCrewAsync(id, ct));
                this.CastsTask = new NotifyTaskCompletion<IEnumerable<PersonModel>>(DataSource.Current.GetCastAsync(id, ct));
                this.RatingsTask = new NotifyTaskCompletion<IEnumerable<RatingModel>>(DataSource.Current.GetRatingsAsync(id, ct));
                this.ReviewsTask = new NotifyTaskCompletion<IEnumerable<ReviewModel>>(DataSource.Current.GetReviewsAsync(id, ct));

                this.Item = await DataSource.Current.GetItemAsync(id, ct);
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