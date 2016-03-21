using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.Core.ViewModels
{
    public partial class SearchViewModel : ViewModelBase
    {
        #region Properties

        private ModelList<ContentItemBase> _Results = new ModelList<ContentItemBase>();
        /// <summary>
        /// List of search results.
        /// </summary>
        public ModelList<ContentItemBase> Results
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
            set { this.SetProperty(ref _SearchText, value); }
        }

        #endregion Properties
        
        #region Constructors

        public SearchViewModel()
        {
            this.Title = Strings.Search.ButtonTextSearch;

            if (DesignMode.DesignModeEnabled)
                return;

            this.IsRefreshVisible = true;
            this.PreservePropertyState(() => this.SearchText);
        }

        #endregion Constructors

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (e.NavigationEventArgs.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.New)
            {
                string param = null;

                // Use any page parameters as the initial search query
                if (e.NavigationEventArgs.Parameter is string)
                    param = e.NavigationEventArgs.Parameter.ToString().Trim();

                if (this.SearchText != param)
                {
                    // Perform the search if there is a new search to perform
                    this.SearchText = param;
                    await this.RefreshAsync();
                }
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        /// <summary>
        /// Refreshes the search results
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Awaitable task.</returns>
        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {            
                if (!string.IsNullOrWhiteSpace(this.SearchText))
                {
                    // Show the busy status
                    this.Title = string.Format(Strings.Search.TextSearching, this.SearchText);
                    this.ShowBusyStatus(this.Title, true);

                    // Call the API to perform the search
                    Platform.Current.Analytics.Event("Search", this.SearchText);
                    var searchResults = await DataSource.Current.SearchAsync(this.SearchText, ct);
                    this.Results.Clear();
                    this.Results.AddRange(searchResults);

                    // Update the page title
                    this.Title = string.Format(Strings.Search.TextSearchResultsCount, this.Results.Count, this.SearchText);
                }
                else
                {
                    // No results, clear page
                    this.Results.Clear();
                    this.Title = Strings.Search.ButtonTextSearch;
                }

                // Clear busy status
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

        protected internal override bool OnBackNavigationRequested()
        {
            return base.OnBackNavigationRequested();
        }
        
        private CancellationTokenSource _cts;
        public async void searchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    if (!string.IsNullOrWhiteSpace(sender.Text))
                    {
                        if (_cts != null)
                        {
                            _cts.Cancel();
                            _cts.Dispose();
                            _cts = null;
                        }

                        _cts = new CancellationTokenSource();

                        try
                        {
                            sender.ItemsSource = await DataSource.Current.SearchAsync(sender.Text, _cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            // Do nothing if cancellation was requested
                        }
                        finally
                        {
                            if (_cts != null)
                                _cts.Dispose();
                            _cts = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not perform search with '{0}'", sender.Text);
            }
        }

        public async void searchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null)
                {
                    var item = args.ChosenSuggestion as ContentItemBase;
                    sender.Text = item.Title;
                    Platform.Current.Navigation.NavigateTo(args.ChosenSuggestion as ContentItemBase);
                }
                else
                {
                    this.SearchText = args.QueryText;
                    await this.RefreshAsync();
                    sender.Text = string.Empty;
                }
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not submit query with '{0}'", args.QueryText);
            }
        }

        public void searchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as ContentItemBase;
            if (item != null)
                sender.Text = item.Title;
        }

        #endregion Methods
    }

    public partial class SearchViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public SearchViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class SearchViewModel : MediaAppSample.Core.ViewModels.SearchViewModel
    {
        public SearchViewModel()
        {
            //this.Results.Add(new ContentItemBase()
            //{
            //    ID = 0,
            //    LineOne = "Mohammed",
            //    LineTwo = "Adenwala",
            //    LineThree = "hello world!"
            //});

            //this.Results.Add(new ContentItemBase() { ID = 1, LineOne = "runtime one", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            //this.Results.Add(new ContentItemBase() { ID = 2, LineOne = "runtime two", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
        }
    }
}