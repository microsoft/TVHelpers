using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using MediaAppSample.Core.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class ItemViewModel : ViewModelBase
    {
        #region Properties

        private string _Title = Strings.Resources.TextNotApplicable;
        public override string Title
        {
            get { return _Title; }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            private set { this.SetProperty(ref _ID, value); }
        }
        
        private ItemModel _Item;
        public ItemModel Item
        {
            get { return _Item; }
            protected set
            {
                if (this.SetProperty(ref _Item, value))
                {
                    this.ID = _Item != null ? _Item.ID : int.MinValue;
                    this.NotifyPropertyChanged(() => this.Title);
                }
            }
        }
        
        private bool _IsDownloadEnabled = true;
        public bool IsDownloadEnabled
        {
            get { return _IsDownloadEnabled; }
            private set { this.SetProperty(ref _IsDownloadEnabled, value); }
        }

        public PinTileCommand PinTileCommand { get; private set; }
        public UnpinTileCommand UnpinTileCommand { get; private set; }
        public CommandBase DownloadCommand { get; private set; }

        #endregion

        #region Constructors

        public ItemViewModel(int id = int.MinValue)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.ID = id;
            this.RequiresAuthorization = true;
            this.IsRefreshVisible = true;
            this.PinTileCommand = new PinTileCommand();
            this.UnpinTileCommand = new UnpinTileCommand();
            this.PinTileCommand.OnSuccessAction = () => this.UnpinTileCommand.RaiseCanExecuteChanged();
            this.UnpinTileCommand.OnSuccessAction = () => this.PinTileCommand.RaiseCanExecuteChanged();
            this.DownloadCommand = new GenericCommand("DownloadCommand", async () => await this.DownloadAsync(), () => this.IsDownloadEnabled);
        }

        #endregion

        #region Methods

        public override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            this.View.GotFocus += View_GotFocus;
            try
            {
                int parameterID = this.ID;

                if(e.Parameter is ItemModel)
                {
                    this.Item = e.Parameter as ItemModel;
                    parameterID = this.Item.ID;
                }
                else if(e.Parameter is int)
                {
                    parameterID = (int)e.Parameter;
                }
                else if (e.Parameter is string)
                {
                    if (!int.TryParse(e.Parameter.ToString(), out parameterID))
                    {
                        parameterID = int.MinValue;
                        try
                        {
                            this.ShowBusyStatus(string.Format("Searching '{0}'...", e.Parameter));
                            using (var api = new ClientApi())
                            {
                                this.Item = (await api.SearchItems(e.Parameter.ToString(), CancellationToken.None)).FirstOrDefault();
                                parameterID = this.Item != null ? parameterID = this.Item.ID : int.MinValue;
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            this.ClearStatus();
                        }
                    }
                }

                if (parameterID == int.MinValue)
                    throw new ArgumentException("No valid model or ID was passed to the ViewModel!");

                if (parameterID != this.ID || this.Item == null)
                {
                    this.ID = parameterID;
                    this.Item = null;
                    await this.RefreshAsync();
                }
                else
                    await this.RefreshUIAsync(isFirstRun);
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during {0} LoadState {1}", this.GetType().Name, e.NavigationEventArgs.NavigationMode);
                throw;
            }
            finally
            {
                this.ClearStatus();
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        public override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            this.View.GotFocus -= View_GotFocus;
            return base.OnSaveStateAsync(e);
        }

        private void View_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.PinTileCommand.RaiseCanExecuteChanged();
            this.UnpinTileCommand.RaiseCanExecuteChanged();
        }

        protected override async Task OnRefreshAsync(CancellationToken token)
        {
            bool isFirstRun = (this.Item == null);

            try
            {
                this.SetTitle(Strings.Resources.TextLoading);
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);
                using (var api = new ClientApi())
                {
                    this.Item = await api.GetItemByID(this.ID, token);
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
                Platform.Current.Logger.LogError(ex, "Error during ItemViewModel.RefreshAsync for item with ID {0}", this.ID);
                Platform.Current.Logger.LogError(ex, "Error during MainViewModel.RefreshAsync");
            }
            finally
            {
                await this.RefreshUIAsync(isFirstRun);
            }
        }

        private void SetTitle(string title)
        {
            _Title = title;
            this.NotifyPropertyChanged(() => this.Title);
        }

        private async Task RefreshUIAsync(bool isFirstRun = false)
        {
            this.SetTitle(this.Item?.LineOne ?? Strings.Resources.TextNotApplicable);
            this.PinTileCommand.RaiseCanExecuteChanged();
            
            if (this.Item != null)
            {
                this.Item.SetDistanceAway(Platform.Current.Geolocation.CurrentLocation);

                // Check if tile exists, clear old notifications, update for new notifications
                if (Platform.Current.Notifications.HasTile(this.Item))
                {
                    Platform.Current.Notifications.ClearTile(this.Item);
                    await Platform.Current.Notifications.CreateOrUpdateTileAsync(this.Item);
                }
                
                if (isFirstRun)
                {
                    var t = Platform.Current.JumpListManager.AddItemAsync(new JumpItemInfo()
                    {
                        Name = this.Item.LineOne,
                        Description = this.Item.LineTwo,
                        Arguments = Platform.Current.GenerateModelArguments(this.Item)
                    });
                }
            }
        }

        private async Task DownloadAsync()
        {
            try
            {
                this.IsDownloadEnabled = false;

                this.ShowBusyStatus("Downloading...");

                for (double p = 0; p <= 100; p++)
                {
                    await Task.Delay(100);
                    this.StatusProgressValue = p;
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error while downloading!");
                this.ShowTimedStatus("Couldn't download the data. Try again later.");
            }
            finally
            {
                this.IsDownloadEnabled = true;
                this.ClearStatus();
            }
        }

        #endregion
    }

    public partial class ItemViewModel
    {
        public ItemViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class ItemViewModel : MediaAppSample.Core.ViewModels.ItemViewModel
    {
        public ItemViewModel()
            : base()
        {
            this.Item = new ItemModel()
            {
                ID = 0,
                LineOne = "PinLine1",
                LineTwo = "PinLine2",
                LineThree = "PinLine3",
                LineFour = "PinLine4",
            };
        }
    }
}