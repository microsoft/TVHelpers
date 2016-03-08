//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace MediaAppSample.UI.Controls
{
    public static class DelayTimer
    {
        private static List<DispatcherTimer> timers = new List<DispatcherTimer>();

        /// <summary>
        /// Create a new single-use delay timer that will fire a function once after a defined number of milliseconds.
        /// </summary>
        /// <param name="msDelay">The delay before the timer fires in milliseconds.</param>
        /// <param name="delayedFunc">The function to fire when the timer expires (sender, params).</param>
        public static void Start(int msDelay, Action<object, object> delayedFunc)
        {
            DispatcherTimer dt = new DispatcherTimer();
            timers.Add(dt);

            dt.Interval = new TimeSpan(0, 0, 0, 0, msDelay);
            dt.Tick += new EventHandler<object>(delayedFunc);
            dt.Tick += StopAndRemoveTimer;
            dt.Start();
        }

        static void StopAndRemoveTimer(object sender, object e)
        {
            DispatcherTimer dt = sender as DispatcherTimer;
            dt.Stop();
            timers.Remove(dt);
        }

        public static void StartAndKeepGoing(int msDelay, Action<object, object> delayedFunc)
        {
            DispatcherTimer dt = new DispatcherTimer();
            timers.Add(dt);

            dt.Interval = new TimeSpan(0, 0, 0, 0, msDelay);
            dt.Tick += new EventHandler<object>(delayedFunc);
            dt.Start();
        }
    }
}
