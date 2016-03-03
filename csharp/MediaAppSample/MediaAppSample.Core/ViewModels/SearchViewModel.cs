using MediaAppSample.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class SearchViewModel : ViewModelBase
    {
        #region Properties

        private string _title = Strings.Search.ButtonTextSearch;

        public override string Title
        {
            get { return _title; }
        }

        private ModelList<ItemModel> _Results = new ModelList<ItemModel>();
        /// <summary>
        /// List of search results.
        /// </summary>
        public ModelList<ItemModel> Results
        {
            get { return _Results; }
            private set { this.SetProperty(ref _Results, value); }
        }

        private string _SearchText;
        /// <summary>
        /// Search status text.
        /// </summary>
        public string SearchText
        {
            get { return _SearchText; }
            private set { this.SetProperty(ref _SearchText, value); }
        }

        #endregion Properties
        
        #region Constructors

        public SearchViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.IsRefreshVisible = true;
        }

        #endregion Constructors

        #region Methods

        public override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            string param = null;
            if (e.NavigationEventArgs.Parameter is string)
                param = e.NavigationEventArgs.Parameter.ToString().Trim();

            if (this.SearchText != param)
            {
                this.SearchText = param;
                await this.RefreshAsync();
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {            
                if (!string.IsNullOrWhiteSpace(this.SearchText))
                {
                    var title = string.Format(Strings.Search.TextSearching, this.SearchText);
                    this.ShowBusyStatus(title, true);
                    this.SetTitle(title);
                    Platform.Current.Analytics.Event("Search", this.SearchText);
                    using (var api = new ClientApi())
                    {
                        var searchResults = await api.SearchItems(this.SearchText, ct);
                        this.Results.Clear();
                        this.Results.AddRange(searchResults);
                    }
                    this.SetTitle(string.Format(Strings.Search.TextSearchResultsCount, this.Results.Count, this.SearchText));
                }
                else
                {
                    this.Results.Clear();
                    this.SetTitle(Strings.Search.ButtonTextSearch);
                }
                this.ClearStatus();
            }
            catch (OperationCanceledException)
            {
                this.ShowTimedStatus(Strings.Resources.TextCancellationRequested, 3000);
            }
            catch (Exception ex)
            {
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
                Platform.Current.Logger.LogError(ex, "Error during SearchViewModel.RefreshAsync with search text '{0}'", this.SearchText);
            }
        }

        public override bool OnBackNavigationRequested()
        {
            return base.OnBackNavigationRequested();
        }

        private void SetTitle(string title)
        {
            _title = title;
            this.NotifyPropertyChanged(() => this.Title);
        }

        #endregion Methods
    }

    public partial class SearchViewModel
    {
        public SearchViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class SearchViewModel : MediaAppSample.Core.ViewModels.SearchViewModel
    {
        public SearchViewModel()
        {
            this.Results.Add(new ItemModel()
            {
                ID = 0,
                LineOne = "Mohammed",
                LineTwo = "Adenwala",
                LineThree = "hello world!"
            });

            this.Results.Add(new ItemModel() { ID = 1, LineOne = "runtime one", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            this.Results.Add(new ItemModel() { ID = 2, LineOne = "runtime two", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
        }
    }
}