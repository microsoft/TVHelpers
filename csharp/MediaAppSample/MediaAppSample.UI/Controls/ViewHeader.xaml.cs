using MediaAppSample.Core.ViewModels;

namespace MediaAppSample.UI.Controls
{
    public abstract class ViewHeaderBase : ViewControlBase<ViewModelBase>
    {
    }

    public sealed partial class ViewHeader : ViewHeaderBase
    {
        public ViewHeader()
        {
            this.InitializeComponent();
        }
    }
}
