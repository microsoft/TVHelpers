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

//using MediaAppSample.Core;
//using MediaAppSample.Core.Models;
//using MediaAppSample.Core.Services;
//using System;
//using System.Collections.Generic;

//namespace MediaAppSample.UI.Services
//{
//    public sealed class HockeyAppService : AnalyticsServiceBase
//    {
//        public HockeyAppService(string key)
//        {
//            if (string.IsNullOrWhiteSpace(key))
//                throw new ArgumentNullException("key");

//            //Api.StartSession(key);
//        }

//        protected override void Initialize()
//        {
//            base.Initialize();
//        }

//        public override void NewPageView(Type pageType)
//        {
//        }

//        public override void Event(string eventName, Dictionary<string, string> metrics = null)
//        {
//            Microsoft.HockeyApp.HockeyClient.Current.
//            //List<Parameter> list = null;
//            //if (metrics != null)
//            //{
//            //    list = new List<Parameter>();
//            //    foreach (var metric in metrics)
//            //        list.Add(new Parameter(metric.Key, metric.Value.ToString()));
//            //}

//            //if (list != null && list.Count > 0)
//            //    Api.LogEvent(eventName, list);
//            //else
//            //    Api.LogEvent(eventName);
//        }

//        public override void Error(Exception ex, string message = null)
//        {
//            //Api.LogError(message, ex);
//        }

//        public override void SetCurrentLocation(ILocationModel loc)
//        {
//            //if (loc != null)
//            //    Api.SetLocation(loc.Latitude, loc.Longitude, 0);
//        }

//        public override void SetUsername(string username)
//        {
//            //Api.SetUserId(username);
//        }

//        public override void Dispose()
//        {
//            //Api.EndSession().AsTask().Wait();
//        }
//    }
//}