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

using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private NotifyTaskCompletion<IEnumerable<TrailerModel>> _TrailerItemsTask;
        public NotifyTaskCompletion<IEnumerable<TrailerModel>> TrailerItemsTask
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

        private NotifyTaskCompletion<ContentItemBase> _ResumeContentItemTask;
        public NotifyTaskCompletion<ContentItemBase> ResumeContentItemTask
        {
            get { return _ResumeContentItemTask; }
            private set { this.SetProperty(ref _ResumeContentItemTask, value); }
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
                    case ItemTypes.Trailer:
                        return Strings.Resources.TextRelatedMovies;

                    case ItemTypes.TvSeries:
                    case ItemTypes.TvEpisode:
                        return Strings.Resources.TextRelatedTV;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private int _NumReviewsShown = 1;
        public int NumReviewsShown
        {
            get { return _NumReviewsShown; }
            set { this.SetProperty(ref _NumReviewsShown, value); }
        }

        private Commands.CommandBase _LoadMoreReviewsCommand = null;
        public Commands.CommandBase LoadMoreReviewsCommand
        {
            get { return _LoadMoreReviewsCommand ?? (_LoadMoreReviewsCommand = new Commands.GenericCommand("LoadMoreReviewsCommand", () => { this.NumReviewsShown = this.ReviewsTask.Result.Count(); })); }
        }

        private CommandBase _pinTileCommand = null;
        public CommandBase PinTileCommand
        {
            get { return _pinTileCommand ?? (_pinTileCommand = new PinTileCommand()); }
        }

        public bool IsTvSeries
        {
            get { return this.Item?.ItemType == ItemTypes.TvSeries; }
        }

        public bool IsTvEpisode
        {
            get { return this.Item?.ItemType == ItemTypes.TvEpisode; }
        }

        public bool IsMovie
        {
            get { return this.Item?.ItemType == ItemTypes.Movie; }
        }

        private string[] _SortOptions;
        public string[] SortOptions
        {
            get { return _SortOptions; }
            private set { this.SetProperty(ref _SortOptions, value); }
        }

        private string[] _SeasonOptions;
        public string[] SeasonOptions
        {
            get { return _SeasonOptions; }
            private set { this.SetProperty(ref _SeasonOptions, value); }
        }

        private string _SelectedSeason;
        public string SelectedSeason
        {
            get { return _SelectedSeason; }
            set
            {
                if (this.SetProperty(ref _SelectedSeason, value))
                    this.NotifyPropertyChanged(() => this.TvEpisodes);
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

        private ModelList<TvEpisodeModel> _TvEpisodes = new ModelList<TvEpisodeModel>();
        public ModelList<TvEpisodeModel> TvEpisodes
        {
            get
            {
                if (!string.IsNullOrEmpty(this.SelectedSeason) && !this.SelectedSeason.Equals(Strings.Resources.TextSeasonsDefault, StringComparison.CurrentCultureIgnoreCase))
                    return new ModelList<TvEpisodeModel>(_TvEpisodes.Where(w => w.SeasonNumber.ToString().Equals(this.SelectedSeason, StringComparison.CurrentCultureIgnoreCase)));
                else
                    return _TvEpisodes;
            }
            private set { this.SetProperty(ref _TvEpisodes, value); }
        }

        #endregion

        #region Constructors

        public DetailsViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.IsRefreshVisible = true;
            this.SortOptions = new string[] { Strings.Resources.TextAZ, Strings.Resources.TextYear };
        }

        #endregion

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
                if (this.ViewParameter is string && !this.ViewParameter.ToString().Equals(this.Item?.ID, StringComparison.CurrentCultureIgnoreCase))
                {
                    await this.RefreshAsync();

                    if (this.Item != null)
                    {
                        var jump = new Services.JumpItemInfo();
                        jump.Arguments = this.Item.ID;
                        jump.GroupName = Strings.Resources.TextRecent;
                        jump.Name = this.Item.Title;
                        jump.Logo = new Uri(this.Item.ImageThumbLandscapeSmall, UriKind.Absolute);
                        await Platform.Current.Jumplist.AddItemAsync(jump);
                    }
                }
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);

                string id = this.ViewParameter.ToString();

                // Check if page loaded without a specific content item provided
                // if true, obtain fake "fetured" content for item type.  A real app would retrieve this from a service.
                ItemTypes itemType;
                var checkForEnum = Enum.TryParse<ItemTypes>(id, out itemType);
                if (checkForEnum)
                {
                    // Update item to be an ID for a content item
                    id = await GetFeaturedItemID(itemType);
                }

                this.Item = await DataSource.Current.GetItemAsync(id, ct);

                if(this.Item == null)
                    this.Item = (await DataSource.Current.SearchAsync(id, ct)).FirstOrDefault();

                this.Title = this.Item?.Title ?? Strings.Resources.TextNotApplicable;
                this.UpdateFilters();

                if (this.Item != null)
                {
                    this.ResumeContentItemTask = new NotifyTaskCompletion<ContentItemBase>(DataSource.Current.GetResumeItem(this.Item.ID, this.Item.ItemType, ct));
                    this.RelatedItemsTask = new NotifyTaskCompletion<IEnumerable<ContentItemBase>>(DataSource.Current.GetRelatedAsync(id, ct));
                    if(this.IsMovie)
                        this.TrailerItemsTask = new NotifyTaskCompletion<IEnumerable<TrailerModel>>(DataSource.Current.GetTrailersAsync(id, ct));
                    this.CreatorsTask = new NotifyTaskCompletion<IEnumerable<PersonModel>>(DataSource.Current.GetCrewAsync(id, ct));
                    this.CastsTask = new NotifyTaskCompletion<IEnumerable<PersonModel>>(DataSource.Current.GetCastAsync(id, ct));
                    this.RatingsTask = new NotifyTaskCompletion<IEnumerable<RatingModel>>(DataSource.Current.GetRatingsAsync(id, ct));
                    this.ReviewsTask = new NotifyTaskCompletion<IEnumerable<ReviewModel>>(DataSource.Current.GetReviewsAsync(id, ct));
                }
                
                this.ClearStatus();
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while trying to load data for ID '{0}'", this.ViewParameter);
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
            }

            await base.OnRefreshAsync(ct);
        }

        // Fake placeholder method used to provide a feaured item if the page is loaded without an actual item.
        // This could happen by opening the SplitView and selecting Movies or TV Series.
        private async Task<string> GetFeaturedItemID(ItemTypes item)
        {
            if (item == ItemTypes.Movie)
            {
                return await Task.FromResult("movie01");
            }
            else //  else TVSeries
            {
                return await Task.FromResult("series19");
            }
        }

        private void UpdateFilters()
        {
            var tv = this.Item as TvSeriesModel;

            if (this.IsTvSeries && tv?.Seasons != null)
            {
                var list = tv.Seasons.Select(s => s.SeasonNumber).Distinct().ConvertToArray<int, string>();
                list.Insert(0, Strings.Resources.TextSeasonsDefault);
                this.SeasonOptions = list.ToArray();

                this.TvEpisodes.Clear();
                this.TvEpisodes.AddRange(tv.Seasons.SelectMany(s => s.Episodes));
            }
            else
            {
                this.SeasonOptions = null;
                this.TvEpisodes.Clear();
            }
        }

        private async void SortItems()
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextSorting);
                await this.TvEpisodes.SortAsync("Year");
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Failed to sort gallery.");
            }
            finally
            {
                this.ClearStatus();
            }
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