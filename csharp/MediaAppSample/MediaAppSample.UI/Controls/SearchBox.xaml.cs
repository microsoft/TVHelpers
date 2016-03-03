using MediaAppSample.Core;
using MediaAppSample.Core.Models;
using System;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public sealed partial class SearchBox : UserControl
    {
        public static readonly DependencyProperty IsSearchExpandedProperty = DependencyProperty.Register("IsSearchExpanded", typeof(bool), typeof(SearchBox), new PropertyMetadata(false));
        public bool IsSearchExpanded
        {
            get { return (bool)GetValue(IsSearchExpandedProperty); }
            set { SetValue(IsSearchExpandedProperty, value); }
        }

        public SearchBox()
        {
            this.InitializeComponent();
        }

        private CancellationTokenSource _cts;

        private async void searchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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
                        using (var api = new ClientApi())
                        {
                            try
                            {
                                sender.ItemsSource = await api.SearchItems(sender.Text, _cts.Token);
                            }
                            catch(OperationCanceledException)
                            {
                                // Do nothing if cancellation was requested
                            }
                            finally
                            {
                                if(_cts != null)
                                    _cts.Dispose();
                                _cts = null;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not perform search with '{0}'", sender.Text);
            }
        }

        private void searchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                var item = args.ChosenSuggestion as ItemModel;
                sender.Text = item.LineOne;
                Platform.Current.Navigation.Item(args.ChosenSuggestion as ItemModel);
            }
            else
            {
                Platform.Current.Navigation.Search(args.QueryText);
                sender.Text = string.Empty;
            }
        }

        private void searchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as ItemModel;
            if (item != null)
                sender.Text = item.LineOne;
        }
    }
}
