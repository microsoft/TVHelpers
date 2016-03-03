using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public static class FrameExtensions
{
    private static DependencyProperty FrameSubFrameProperty = DependencyProperty.RegisterAttached("_FrameSubFrame", typeof(Frame), typeof(Frame), null);

    public static Frame GetSubFrame(this Frame frame)
    {
        return frame.GetValue(FrameSubFrameProperty) as Frame;
    }

    public static void SetSubFrame(this Frame frame, Frame subFrame)
    {
        frame.SetValue(FrameSubFrameProperty, subFrame);
    }
}
