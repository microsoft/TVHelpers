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

using System.Threading;
using Windows.UI.Xaml;

namespace MediaAppSample.Core
{
    /// <summary>
    /// AgentSync is used to determine if the main app or a background task is running. You can use this to
    /// prevent execution of a background task if the main app is in the foreground.
    /// </summary>
    public class AgentSync
    {
        private static Mutex _agentMutex = new Mutex(false, "AgentSyncMutex");
        private static bool _applicationRunning;

        /// <summary>
        /// Initializes the AgentSync so that it can keep track of the application running status.
        /// </summary>
        /// <param name="app"></param>
        public static void Init(Application app)
        {
            app.Resuming += App_Resuming;
            app.Suspending += App_Suspending;
            ApplicationIsLaunched();
        }

        private static void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            ApplicationIsSuspended();
        }

        private static void App_Resuming(object sender, object e)
        {
            ApplicationIsLaunched();
        }

        /// <summary>
        /// Markes the agent sync as running.
        /// </summary>
        private static void ApplicationIsLaunched()
        {
            if (_applicationRunning)
                return;
            lock (_agentMutex)
            {
                if (_applicationRunning)
                    return;
                _agentMutex.WaitOne();
                _applicationRunning = true;
            }
        }

        /// <summary>
        /// Marks the agent sync as NOT running
        /// </summary>
        private static void ApplicationIsSuspended()
        {
            lock (_agentMutex)
            {
                if (!_applicationRunning)
                    return;
                _agentMutex.ReleaseMutex();
                _applicationRunning = false;
            }
        }

        /// <summary>
        /// Indicates whether or not the main application is currently running.
        /// </summary>
        /// <returns>True if the application is launched/running else false.</returns>
        public static bool IsApplicationLaunched()
        {
            lock (_agentMutex)
            {
                if (_agentMutex.WaitOne(1))
                {
                    _agentMutex.ReleaseMutex();
                    return false;
                }
                return true;
            }
        }
    }

}
