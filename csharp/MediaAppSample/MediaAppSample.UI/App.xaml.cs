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

using MediaAppSample.Core;
using MediaAppSample.UI.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Platform = MediaAppSample.Core.Platform;

namespace MediaAppSample.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        #region Constructor

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            AgentSync.Init(this);

            this.InitializeComponent();
            this.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;

            // Initalize the platform object which is the singleton instance to access various services
            Platform.Current.Navigation = new NavigationManager();
            //Platform.Current.Analytics = new FlurryAnalyticsService("M76D4BWBDRTWTVJZZ27P");

            Platform.Current.Logger.CurrentLevel = LogLevels.Information;
            
            this.RequestedTheme = ApplicationTheme.Dark;

            // TODO Microsoft.HockeyApp.HockeyClient.Current.Configure("Your-App-ID");
        }

        #endregion

        #region App Launching/Resuming

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Platform.Current.Analytics.Event("App - OnLaunched - LaunchKind: {0}", e.Kind.ToString());
            await this.InitializeAsync(e, e.PrelaunchActivated);

            var t = Windows.System.Threading.ThreadPool.RunAsync(async (o) =>
            {
                try
                {
                    // Install the VCD. Since there's no simple way to test that the VCD has been imported, or that it's your most recent
                    // version, it's not unreasonable to do this upon app load.
                    var vcd = await Package.Current.InstalledLocation.GetFileAsync(@"Resources\VCD.xml");
                    await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcd);

                }
                catch (Exception ex)
                {
                    Platform.Current.Logger.LogError(ex, "Installing voice commands failed!");
//#if DEBUG
//                    throw;
//#endif
                }
            });
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            Platform.Current.Analytics.Event("App - OnActivated - LaunchKind: {0}", e.Kind.ToString());
            await this.InitializeAsync(e);
            base.OnActivated(e);
        }

        protected override async void OnSearchActivated(SearchActivatedEventArgs e)
        {
            Platform.Current.Analytics.Event("App - OnSearchActivated - LaunchKind: {0}", e.Kind.ToString());
            await this.InitializeAsync(e);
            base.OnSearchActivated(e);
        }

        private async Task InitializeAsync(IActivatedEventArgs e, bool preLaunchActivated = false)
        {
            try
            {
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    //this.DebugSettings.EnableFrameRateCounter = true;
                    //this.DebugSettings.EnableRedrawRegions = true;
                    //this.DebugSettings.IsOverdrawHeatMapEnabled = true;
                    //this.DebugSettings.IsBindingTracingEnabled = true;
                    //this.DebugSettings.IsTextPerformanceVisualizationEnabled = true;
                }

                if(e.PreviousExecutionState != ApplicationExecutionState.Running)
                    this.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
#endif

                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    // No need to run any of this logic if the app is already running

                    // Ensure unobserved task exceptions (unawaited async methods returning Task or Task<T>) are handled
                    TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                    // Determine if the app is a new instance or being restored after app suspension
                    if (e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser || e.PreviousExecutionState == ApplicationExecutionState.NotRunning)
                        await Platform.Current.AppInitializingAsync(InitializationModes.New);
                    else
                        await Platform.Current.AppInitializingAsync(InitializationModes.Restore);
                }



                Frame rootFrame = Window.Current.Content as Frame;

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new ApplicationFrame();

                    // Associate the frame with a SuspensionManager key                                
                    SuspensionManager.RegisterFrame(rootFrame, "RootFrame");

                    // Set the default language
                    rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    
                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;

                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        try
                        {
                            // Restore the saved session state only when appropriate
                            await SuspensionManager.RestoreAsync();
                        }
                        catch (SuspensionManagerException)
                        {
                            // Something went wrong restoring state.
                            // Assume there is no state and continue
                        }
                    }
                }

                if (preLaunchActivated == false)
                {
                    // Manage activation and process arguments
                    Platform.Current.Navigation.HandleActivation(e, rootFrame);
                    
                    // Ensure the current window is active
                    Window.Current.Activate();

                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 200));
                }
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogErrorFatal(ex, "Error during App InitializeAsync(e)");
                throw ex;
            }
        }

        #endregion

        #region App Suspending

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                Platform.Current.AppSuspending();
                await SuspensionManager.SaveAsync();
            }
            catch(SuspensionManagerException ex)
            {
                Platform.Current.Logger.LogErrorFatal(ex, "Suspension manager failed during App OnSuspending!");
            }
            catch (Exception ex)
            {
                Platform.Current.Logger.LogErrorFatal(ex, "Error during App OnSuspending");
                throw ex;
            }
            deferral.Complete();
        }

        #endregion

        #region Handling Exceptions

        /// <summary>
        /// Invoked when an unhandled exception in the application occurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = Platform.Current.AppUnhandledException(e.Exception);
         }

        /// <summary>
        /// Invoked when the task schedule sees an exception occur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Details about the task exception.</param>
        private void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Platform.Current.AppUnhandledException(e.Exception);
        }

        /// <summary>
        /// Invoked when any bindings fail.
        /// </summary>
        /// <param name="sender">Object which failed with binding</param>
        /// <param name="e">Details about the binding failure</param>
        private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
        {
            Platform.Current.Logger.Log(LogLevels.Error, "Binding Failed: {0}", e.Message);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        #endregion
    }
}
