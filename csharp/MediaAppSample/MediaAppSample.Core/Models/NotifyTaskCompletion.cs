using System;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Task wrapper for async operations to make them UI bindable. See https://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : ModelBase
    {
        #region Properties

        public Task<TResult> Task { get; private set; }
        public TResult Result
        {
            get
            {
                return (this.Task.Status == TaskStatus.RanToCompletion) ? this.Task.Result : default(TResult);
            }
        }
        public TaskStatus Status { get { return this.Task.Status; } }
        public bool IsCompleted { get { return this.Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !this.Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted { get { return this.Task.Status == TaskStatus.RanToCompletion; } }
        public bool IsCanceled { get { return this.Task.IsCanceled; } }
        public bool IsFaulted { get { return this.Task.IsFaulted; } }
        public AggregateException Exception { get { return this.Task.Exception; } }
        public Exception InnerException { get { return (this.Exception == null) ? null : this.Exception.InnerException; } }
        public string ErrorMessage { get { return (this.InnerException == null) ? null : this.InnerException.Message; } }

        #endregion

        #region Constructor

        public NotifyTaskCompletion(Task<TResult> task)
        {
            if (task == null)
                throw new NullReferenceException("task");

            this.Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        #endregion

        #region Methods

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }

            this.NotifyPropertyChanged(() => this.Status);
            this.NotifyPropertyChanged(() => this.IsCompleted);
            this.NotifyPropertyChanged(() => this.IsNotCompleted);

            if (task.IsCanceled)
            {
                this.NotifyPropertyChanged(() => this.IsCanceled);
            }
            else if (task.IsFaulted)
            {
                this.NotifyPropertyChanged(() => this.IsFaulted);
                this.NotifyPropertyChanged(() => this.Exception);
                this.NotifyPropertyChanged(() => this.InnerException);
                this.NotifyPropertyChanged(() => this.ErrorMessage);
            }
            else
            {
                this.NotifyPropertyChanged(() => this.IsSuccessfullyCompleted);
                this.NotifyPropertyChanged(() => this.Result);
            }
        }

        #endregion
    }
}
