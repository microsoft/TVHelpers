using MediaAppSample.Core.ViewModels;

namespace MediaAppSample.UI.Controls
{
    public abstract class ViewHeaderStatusBase : ViewControlBase<ViewModelBase>
    {
    }

    public sealed partial class ViewHeaderStatus : ViewHeaderStatusBase
    {
        public ViewHeaderStatus()
        {
            this.InitializeComponent();
        }
    }
}
