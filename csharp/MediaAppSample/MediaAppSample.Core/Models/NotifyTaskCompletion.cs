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
