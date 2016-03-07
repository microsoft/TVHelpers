using MediaAppSample.Core.ViewModels;

namespace MediaAppSample.UI.Controls
{
    public abstract class BusyPanelBase : ViewControlBase<ViewModelBase>
    {
    }

    public sealed partial class BusyPanel : BusyPanelBase
    {
        public BusyPanel()
        {
            this.InitializeComponent();
        }
    }
}
