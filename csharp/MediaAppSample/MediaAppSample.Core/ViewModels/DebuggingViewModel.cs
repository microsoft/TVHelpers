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

        public override string Title
        {
            get { return "Debugging"; }
        }

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
            if (DesignMode.DesignModeEnabled)
                return;

            this.TestAppCrashCommand = new GenericCommand("TestAppCrashCommand", () => { throw new Exception("Test crash thrown!"); });
        }

        #endregion Constructors

        #region Methods

        public override Task OnLoadStateAsync(LoadStateEventArgs e, bool isFirstRun)
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