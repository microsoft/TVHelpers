using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Data;
using MediaAppSample.Core.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MediaAppSample.Core.ViewModels
{
    public partial class QueueViewModel : ViewModelBase
    {
        #region Properties

        public override string Title
        {
            get { return "Queue"; }
        }

        private QueueCollection _queue;
        /// <summary>
        /// Gets a list of content items in the queue
        /// </summary>
        public QueueCollection Queue
        {
            get { return _queue; }
            private set
            {
                if (this.SetProperty(ref _queue, value))
                {
                    // If the collection object changes, subscribe to the collection changed event so you can
                    // notify the QueueTop3 property to update to show the latest queue items.
                    if (value != null)
                        value.CollectionChanged += (o, e) => this.UpdateQueueProperties();

                    this.UpdateQueueProperties();
                };
            }
        }

        private QueueCollection _QueueTop3;
        /// <summary>
        /// Gets the top 3 times in the queue collection.
        /// </summary>
        public QueueCollection QueueTop3
        {
            get { return _QueueTop3; }
            private set { this.SetProperty(ref _QueueTop3, value); }
        }

        private QueueModel _NextQueueItem;
        /// <summary>
        /// Gets the first item in the queue or null
        /// </summary>
        public QueueModel NextQueueItem
        {
            get { return _NextQueueItem; }
            private set { this.SetProperty(ref _NextQueueItem, value); }
        }

        private CommandBase _AddToQueueCommand = null;
        public CommandBase AddToQueueCommand
        {
            get { return _AddToQueueCommand ?? (_AddToQueueCommand = new GenericCommand<ContentItemBase>("AddToQueueCommand", async (m) => await this.AddToQueueAsync(m), this.CanAddToQueue)); }
        }

        private CommandBase _RemoveFromQueueCommand = null;
        public CommandBase RemoveFromQueueCommand
        {
            get { return _RemoveFromQueueCommand ?? (_RemoveFromQueueCommand = new GenericCommand<ContentItemBase>("RemoveFromQueueCommand", async (m) => await this.RemoveFromQueueAsync(m), this.CanRemoveFromQueue)); }
        }

        #endregion Properties

        #region Constructors

        public QueueViewModel()
        {
            if (DesignMode.DesignModeEnabled)
                return;

            this.IsRefreshVisible = true;
            this.QueueTop3 = new QueueCollection();
            this.Queue = new QueueCollection();
        }

        #endregion Constructors

        #region Methods

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            if (isFirstRun)
            {
                this.Queue = await this.LoadFromCacheAsync(() => this.Queue) ?? new QueueCollection();
                await this.RefreshAsync();
            }

            await base.OnLoadStateAsync(e, isFirstRun);
        }

        protected override async Task OnRefreshAsync(CancellationToken ct)
        {
            try
            {
                this.ShowBusyStatus(Strings.Resources.TextLoading, true);

                // Load queue data
                var list = new QueueCollection();
                list.AddRange(await DataSource.Current.GetQueue(ct));
                this.Queue = list;
                await this.SaveToCacheAsync(() => this.Queue);

                ct.ThrowIfCancellationRequested();
                this.ClearStatus();
            }
            catch (OperationCanceledException)
            {
                this.ShowTimedStatus(Strings.Resources.TextCancellationRequested, 3000);
            }
            catch (Exception ex)
            {
                this.ShowTimedStatus(Strings.Resources.TextErrorGeneric);
                Platform.Current.Logger.LogError(ex, "Error during RefreshAsync");
            }
        }

        protected override Task OnSaveStateAsync(SaveStateEventArgs e)
        {
            return base.OnSaveStateAsync(e);
        }

        private void UpdateQueueProperties()
        {
            if (this.Queue != null)
            {
                this.QueueTop3 = new QueueCollection();
                this.QueueTop3.AddRange(this.Queue.Take(3));
                this.NextQueueItem = this.Queue.FirstOrDefault();
            }
            else
            {
                this.QueueTop3 = null;
                this.NextQueueItem = null;
            }
        }

        private bool CanAddToQueue(ContentItemBase item)
        {
            return !this.Queue.ContainsItem(item);
        }

        private bool CanRemoveFromQueue(ContentItemBase item)
        {
            return this.Queue.ContainsItem(item);
        }

        private async Task AddToQueueAsync(ContentItemBase item)
        {
            try
            {
                await DataSource.Current.AddToQueue(item, CancellationToken.None);
                if(!this.Queue.ContainsItem(item))
                    this.Queue.Add(item);
            }
            catch(Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during AddToQueueAsync");
            }
            finally
            {
                this.AddToQueueCommand.RaiseCanExecuteChanged();
                this.RemoveFromQueueCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task RemoveFromQueueAsync(ContentItemBase item)
        {
            try
            {
                await DataSource.Current.RemoveFromQueue(item, CancellationToken.None);
                this.Queue.RemoveItem(item);
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogError(ex, "Error during AddToQueueAsync");
            }
            finally
            {
                this.AddToQueueCommand.RaiseCanExecuteChanged();
                this.RemoveFromQueueCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion Methods
    }

    public partial class QueueViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public QueueViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class QueueViewModel : MediaAppSample.Core.ViewModels.QueueViewModel
    {
        public QueueViewModel()
        {
        }
    }
}