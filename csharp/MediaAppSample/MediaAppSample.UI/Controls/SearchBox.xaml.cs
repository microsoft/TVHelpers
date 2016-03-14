using MediaAppSample.Core;
using MediaAppSample.Core.Data;
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
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Could not perform search with '{0}'", sender.Text);
            }
        }

        private void searchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                var item = args.ChosenSuggestion as ContentItemBase;
                sender.Text = item.Title;
                Platform.Current.Navigation.NavigateTo(args.ChosenSuggestion as ContentItemBase);
            }
            else
            {
                Platform.Current.Navigation.Search(args.QueryText);
                sender.Text = string.Empty;
            }
        }

        private void searchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as ContentItemBase;
            if (item != null)
                sender.Text = item.Title;
        }
    }
}
