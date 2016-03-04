using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
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

        private string _ID;
        public string ContentID
        {
            get { return _ID; }
            private set { this.SetProperty(ref _ID, value); }
        }
        
        private ContentItemBase _Item;
        public ContentItemBase Item
        {
            get { return _Item; }
            protected set
            {
                if (this.SetProperty(ref _Item, value))
                {
                    this.ContentID = _Item?.ContentID;
                    this.NotifyPropertyChanged(() => this.Title);
                }
            }
        }

        public PinTileCommand PinTileCommand { get; private set; }
        public UnpinTileCommand UnpinTileCommand { get; private set; }

        #endregion

        #region Constructors

        public ItemViewModel(string id = null)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.ContentID = id;
            this.RequiresAuthorization = true;
            this.IsRefreshVisible = true;
            this.PinTileCommand = new PinTileCommand();
            this.UnpinTileCommand = new UnpinTileCommand();
            this.PinTileCommand.OnSuccessAction = () => this.UnpinTileCommand.RaiseCanExecuteChanged();
            this.UnpinTileCommand.OnSuccessAction = () => this.PinTileCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Methods

        public override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            this.View.GotFocus += View_GotFocus;
            try
            {
                string parameterID = null;

                if(e.Parameter is ContentItemBase)
                {
                    this.Item = e.Parameter as ContentItemBase;
                    parameterID = this.Item.ContentID;
                }
                else if (e.Parameter is string)
                {
                    try
                    {
                        this.ShowBusyStatus(string.Format("Searching '{0}'...", e.Parameter));
                        this.Item = (await DataSource.Current.SearchItems(e.Parameter.ToString(), CancellationToken.None)).FirstOrDefault();
                        parameterID = this.Item?.ContentID;
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

                if (parameterID != this.ContentID || this.Item == null)
                {
                    this.ContentID = parameterID;
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
                this.Item = await DataSource.Current.GetItemByID(this.ContentID, token);
                this.ClearStatus();
            }
            catch (OperationCanceledException)
            {
                this.ShowTimedStatus(Strings.Resources.TextCancellationRequested, 3000);
            }
            catch (Exception ex)
            {
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
                Platform.Current.Logger.LogError(ex, "Error during ItemViewModel.RefreshAsync for item with ID {0}", this.ContentID);
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
            this.SetTitle(this.Item?.Title ?? Strings.Resources.TextNotApplicable);
            this.PinTileCommand.RaiseCanExecuteChanged();
            
            if (this.Item != null)
            {
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
                        Name = this.Item.Title,
                        Description = this.Item.Description,
                        Arguments = Platform.Current.GenerateModelArguments(this.Item)
                    });
                }
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
            //this.Item = new ItemModel()
            //{
            //    ID = 0,
            //    LineOne = "PinLine1",
            //    LineTwo = "PinLine2",
            //    LineThree = "PinLine3",
            //    LineFour = "PinLine4",
            //};
        }
    }
}