using Contoso.Core.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace Contoso.Core.Providers
{
    public partial class PlatformCoreBase
    {
        /// <summary>
        /// Gets access to displaying dialog messages of the platform currently executing.
        /// </summary>
        public DialogProvider Dialogs
        {
            get { return this.GetAdapter<DialogProvider>(); }
            protected set { this.Register<DialogProvider>(value); }
        }
    }
}

namespace Contoso.Core.Providers
{
    /// <summary>
    /// Interface providing acccess to display dialog messages on the executing platform.
    /// </summary>
    public sealed class DialogProvider : ProviderBase
    {
        private CoreDispatcher _dispatcher = null;

        internal DialogProvider()
        {
        }

        #region Methods

        public Task<int> ShowMessageBoxAsync(string message, IList<string> buttonNames = null, int defaultIndex = 0)
        {
            return this.ShowMessageBoxAsync(message, GeneralStrings.ApplicationName, buttonNames, defaultIndex);
        }

        public async Task<int> ShowMessageBoxAsync(string message, string title, IList<string> buttonNames = null, int defaultIndex = 0)
        {
            if (_dispatcher == null)
            {
                var coreWindow = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow;
                if (coreWindow != null)
                    _dispatcher = coreWindow.Dispatcher;
            }

            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("The specified message cannot be null or empty.", "message");

            if (string.IsNullOrWhiteSpace(title))
                title = GeneralStrings.ApplicationName;

            int result = defaultIndex;
            MessageDialog dialog = new MessageDialog(message, title);

            if (buttonNames != null && buttonNames.Count > 0)
                foreach (string button in buttonNames)
                    dialog.Commands.Add(new UICommand(button, new UICommandInvokedHandler((o) => result = buttonNames.IndexOf(button))));
            else
                dialog.Commands.Add(new UICommand(GeneralStrings.TextOk, new UICommandInvokedHandler((o) => result = 0)));

            dialog.DefaultCommandIndex = (uint)defaultIndex;

            if (_dispatcher == null || _dispatcher.HasThreadAccess)
            {
                await dialog.ShowAsync();
                return result;
            }
            else
            {
                var tcs = new TaskCompletionSource<int>();

                // Execute asynchronously on the thread the Dispatcher is associated with.
                await _dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                {
                    await dialog.ShowAsync();
                    tcs.TrySetResult(result);
                });
                return tcs.Task.Result;
            }
        }

        #endregion
    }
}