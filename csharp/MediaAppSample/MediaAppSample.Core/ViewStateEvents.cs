using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace MediaAppSample.Core
{
    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.LoadState"/>event
    /// </summary>
    public delegate void LoadStateEventHandler(object sender, LoadStateEventArgs e);
    /// <summary>
    /// Represents the method that will handle the <see cref="NavigationHelper.SaveState"/>event
    /// </summary>
    public delegate void SaveStateEventHandler(object sender, SaveStateEventArgs e);

    /// <summary>
    /// Class used to hold the event data required when a page attempts to load state.
    /// </summary>
    public sealed class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the navigation event args passed to the OnNavigatingTo event of the page.
        /// </summary>
        public NavigationEventArgs NavigationEventArgs { get; private set; }

        /// <summary>
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </summary>
        public IDictionary<string, object> PageState { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadStateEventArgs"/> class.
        /// </summary>
        /// <param name="navigationParameter">
        /// The parameter value passed to <see cref="Frame.Navigate(Type, Object)"/> 
        /// when this page was initially requested.
        /// </param>
        /// <param name="pageState">
        /// A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.
        /// </param>
        public LoadStateEventArgs(NavigationEventArgs e, IDictionary<string, object> pageState)
            : base()
        {
            this.NavigationEventArgs = e;
            this.PageState = pageState;
            this.Parameter = NavigationParameterSerializer.Deserialize(e.Parameter); // Deserializes the parameter from the navigation event if necessary and stores instance
        }

        /// <summary>
        /// Gets the deserialized instance of the parameter passed to this page.
        /// </summary>
        public object Parameter { get; private set; }
    }

    /// <summary>
    /// Class used to hold the event data required when a page attempts to save state.
    /// </summary>
    public sealed class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the navigation event args passed to the OnNavigatedFrom event of the page.
        /// </summary>
        public NavigationEventArgs NavigationEventArgs { get; private set; }

        /// <summary>
        /// An empty dictionary to be populated with serializable state.
        /// </summary>
        public IDictionary<string, object> PageState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        public SaveStateEventArgs(NavigationEventArgs e, IDictionary<string, object> pageState)
            : base()
        {
            this.NavigationEventArgs = e;
            this.PageState = pageState;
        }
    }
}