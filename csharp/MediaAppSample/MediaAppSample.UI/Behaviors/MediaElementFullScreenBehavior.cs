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

using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MediaAppSample.UI.Behaviors
{
    /// <summary>
    /// Enables a control to execute its associated command when an enter key press is detected within the control.
    /// </summary>
    public class MediaElementFullScreenBehavior : Behavior<MediaElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.DoubleTapped += AssociatedObject_DoubleTapped;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DoubleTapped -= AssociatedObject_DoubleTapped;
            base.OnDetaching();
        }

        private void AssociatedObject_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            AssociatedObject.IsFullWindow = !AssociatedObject.IsFullWindow;
        }
    }
}
