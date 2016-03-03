//using Contoso.Core;
//using Contoso.Core.Models;
//using Contoso.Core.Services;
//using System;
//using System.Collections.Generic;

//namespace Contoso.UI.Services
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