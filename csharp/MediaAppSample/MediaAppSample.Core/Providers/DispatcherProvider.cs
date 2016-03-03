using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Contoso.Core.Providers
{
    public partial class PlatformCoreBase
    {
        /// <summary>
        /// Gets access to the dispatcher of the platform currently executing. This allows code to be run in the background or force to execute on a UI thread.
        /// </summary>
        public DispatcherProvider Dispatcher
        {
            get { return this.GetAdapter<DispatcherProvider>(); }
            protected set { this.Register<DispatcherProvider>(value); }
        }
    }

    /// <summary>
    /// Interface to access the dispatcher on the running platform and be able to execute code on a background thread or ensure code executes on the UI thread.
    /// </summary>
    public sealed class DispatcherProvider : ProviderBase
    {
        #region Variables

        private Windows.UI.Core.CoreDispatcher _dispatcher = null;

        #endregion

        internal DispatcherProvider()
        {
        }

        #region Methods

        /// <summary>
        /// Runs a function on the currently executing platform's UI thread.
        /// </summary>
        /// <param name="action">Code to be executed on the UI thread</param>
        /// <param name="priority">Priority to indicate to the system when to prioritize the execution of the code</param>
        /// <returns>Task representing the code to be executing</returns>
        public void InvokeOnUIThread(Action action, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (_dispatcher == null)
            {
                var coreWindow1 = Windows.UI.Core.CoreWindow.GetForCurrentThread();
                if (coreWindow1 != null)
                    _dispatcher = coreWindow1.Dispatcher;

                if (_dispatcher == null)
                {
                    var coreWindow2 = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow;
                    if (coreWindow2 != null)
                        _dispatcher = coreWindow2.Dispatcher;
                }
            }

            if (_dispatcher == null || _dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                // Execute asynchronously on the thread the Dispatcher is associated with.
                var task = _dispatcher.RunAsync(priority, () => action());
            }
        }

        public Task InvokeOnUIThreadAsync(Action action, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            Action newAction = new Action(() =>
            {
                try
                {
                    action();
                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            this.InvokeOnUIThread(newAction);
            return tcs.Task;
        }

        #endregion
    }
}
