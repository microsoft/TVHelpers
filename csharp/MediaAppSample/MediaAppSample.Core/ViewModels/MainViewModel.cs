using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return Strings.Resources.ViewTitleWelcome; }
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        private ModelList<ContentItemBase> _Items;

        public ModelList<ContentItemBase> Items
        {
            get { return _Items; }
            protected set { this.SetProperty(ref _Items, value); }
        }

        public CommandBase SortCommand { get; private set; }
        
        #endregion Properties

        #region Constructors

        public MainViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.RequiresAuthorization = true;
            this.IsRefreshVisible = true;
            this.SortCommand = new GenericCommand<string>("MainViewModel-SortCommand", async (propertyName) =>
            {
                try
                {
                    this.ShowBusyStatus("Sorting...");
                    await this.Items.SortAsync(propertyName);
                }
                finally
                {
                    this.ClearStatus();
                }
            });
        }

        #endregion Constructors
        
        #region Methods

        public override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
                // Load from cache else initialize new instance of list
                this.Items = await this.LoadFromCacheAsync(() => this.Items) ?? new ModelList<ContentItemBase>();
                                
                // Load data
                await this.RefreshAsync();

                // Clear primary tile
                Platform.Current.Notifications.ClearTile(this);

                // Check to see if the user should be prompted to rate the application
                await Platform.Current.Ratings.CheckForRatingsPromptAsync(this);
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        public override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return base.OnSaveStateAsync(e);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);

                this.Items.Clear();
                this.Items.AddRange(await DataSource.Current.GetItems(ct));
                
                // Save to cache
                await this.SaveToCacheAsync(() => this.Items);
                
                ct.ThrowIfCancellationRequested();
                this.ShowBusyStatus("Updating voice commands and tiles...");
                await this.WaitAllAsync(
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

        private async Task UpdateVoiceCommandsAsync()
        {
            var list = new List<string>();
            foreach (var item in this.Items)
                list.Add(item.Title);
            await Platform.Current.VoiceCommandManager.UpdatePhraseListAsync("CommandSet", "ItemName", list);
        }

        #endregion Methods
    }

    public partial class MainViewModel
    {
        public MainViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class MainViewModel : MediaAppSample.Core.ViewModels.MainViewModel
    {
        public MainViewModel()
        {
            // Sample data for design time ONLY
            this.Items = new ModelList<ContentItemBase>();
            //this.Items.Add(new ContentItemBase()
            //{
            //    ID = "0",
            //    LineOne = "Mohammed",
            //    LineTwo = "Adenwala",
            //    LineThree = "hello world!"
            //});

            //this.Items.Add(new ItemModel() { ID = 1, LineOne = "runtime one", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu", Latitude = 40, Longitude = -87 });
            //this.Items.Add(new ItemModel() { ID = 2, LineOne = "runtime two", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus", Latitude = 1, Longitude = 10 });
            //this.Items.Add(new ItemModel() { ID = 3, LineOne = "runtime three", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent", Latitude = 2, Longitude = 20 });
            //this.Items.Add(new ItemModel() { ID = 4, LineOne = "runtime four", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos", Latitude = 3, Longitude = 30 });
            //this.Items.Add(new ItemModel() { ID = 5, LineOne = "runtime five", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur", Latitude = 4, Longitude = 40 });
            //this.Items.Add(new ItemModel() { ID = 6, LineOne = "runtime six", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent", Latitude = 5, Longitude = 50 });
            //this.Items.Add(new ItemModel() { ID = 7, LineOne = "runtime seven", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat", Latitude = 6, Longitude = 60 });
            //this.Items.Add(new ItemModel() { ID = 8, LineOne = "runtime eight", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum", Latitude = 7, Longitude = 70 });
            //this.Items.Add(new ItemModel() { ID = 9, LineOne = "runtime nine", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu", Latitude = 8, Longitude = 80 });
            //this.Items.Add(new ItemModel() { ID = 10, LineOne = "runtime ten", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus", Latitude = 9, Longitude = 90 });
            //this.Items.Add(new ItemModel() { ID = 11, LineOne = "runtime eleven", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent", Latitude = 10, Longitude = 100 });
            //this.Items.Add(new ItemModel() { ID = 12, LineOne = "runtime twelve", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos", Latitude = 11, Longitude = 110 });
            //this.Items.Add(new ItemModel() { ID = 13, LineOne = "runtime thirteen", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur", Latitude = 12, Longitude = 120 });
            //this.Items.Add(new ItemModel() { ID = 14, LineOne = "runtime fourteen", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent", Latitude = 13, Longitude = 130 });
            //this.Items.Add(new ItemModel() { ID = 15, LineOne = "runtime fifteen", LineTwo = "Habitant inceptos interdum lobortis", LineThree = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat", Latitude = 14, Longitude = 140 });
            //this.Items.Add(new ItemModel() { ID = 16, LineOne = "runtime sixteen", LineTwo = "Nascetur pharetra placerat pulvinar", LineThree = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum", Latitude = 15, Longitude = 150 });
        }
    }
}