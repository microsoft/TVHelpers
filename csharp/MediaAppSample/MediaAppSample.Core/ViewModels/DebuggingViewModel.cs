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

using MediaAppSample.Core.Commands;
using MediaAppSample.Core.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;

namespace MediaAppSample.Core.ViewModels
{
    public partial class DebuggingViewModel : ViewModelBase
    {
        #region Properties
        
        public ICommand TestAppCrashCommand { get; private set; }

        private ModelList<BackgroundTaskRunInfo> _BackgroundTasksInfo = new ModelList<BackgroundTaskRunInfo>();
        public ModelList<BackgroundTaskRunInfo> BackgroundTasksInfo
        {
            get { return _BackgroundTasksInfo; }
            private set { this.SetProperty(ref _BackgroundTasksInfo, value); }
        }
        
        #endregion Properties

        #region Constructors

        public DebuggingViewModel()
        {
            this.Title = "Debugging";

            if (DesignMode.DesignModeEnabled)
                return;

            this.TestAppCrashCommand = new GenericCommand("TestAppCrashCommand", () => { throw new Exception("Test crash thrown!"); });
        }

        #endregion Constructors

        #region Methods

        protected override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
        {
            this.Refresh();
            return base.OnLoadStateAsync(e, isFirstRun);
        }

        private void Refresh()
        {
            this.BackgroundTasksInfo.Clear();
            foreach (var registration in BackgroundTaskRegistration.AllTasks)
            {
                string key = "TASK_" + registration.Value.Name;
                if (Platform.Current.Storage.ContainsSetting(key, Windows.Storage.ApplicationData.Current.LocalSettings))
                {
                    var info = Platform.Current.Storage.LoadSetting<BackgroundTaskRunInfo>(key, Windows.Storage.ApplicationData.Current.LocalSettings);
                    if (info != null)
                    {
                        info.TaskName = registration.Value.Name;
                        this.BackgroundTasksInfo.Add(info);
                    }
                }
            }
        }

        #endregion Methods
    }

    public partial class DebuggingViewModel
    {
        /// <summary>
        /// Self-reference back to this ViewModel. Used for designtime datacontext on pages to reference itself with the same "ViewModel" accessor used 
        /// by x:Bind and it's ViewModel property accessor on the View class. This allows you to do find-replace on views for 'Binding' to 'x:Bind'.
        [Newtonsoft.Json.JsonIgnore()]
        [System.Runtime.Serialization.IgnoreDataMember()]
        public DebuggingViewModel ViewModel { get { return this; } }
    }
}

namespace MediaAppSample.Core.ViewModels.Designer
{
    public sealed class DebuggingViewModel : MediaAppSample.Core.ViewModels.DebuggingViewModel
    {
        public DebuggingViewModel()
        {
        }
    }
}