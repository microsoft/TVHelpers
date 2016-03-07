using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public static class FrameExtensions
{
    private static DependencyProperty ChildFrameProperty = DependencyProperty.RegisterAttached("_FrameChildFrame", typeof(Frame), typeof(Frame), null);

    public static Frame GetChildFrame(this Frame frame)
    {
        return frame.GetValue(ChildFrameProperty) as Frame;
    }

    public static void SetChildFrame(this Frame frame, Frame subFrame)
    {
        frame.SetValue(ChildFrameProperty, subFrame);
    }
}
