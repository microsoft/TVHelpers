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

using MediaAppSample.Core;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Core;

namespace MediaAppSample.UI.Views
{

    public abstract class MediaViewBase : ViewBase<MediaViewModel>
    {
    }

    public sealed partial class MediaView : MediaViewBase
    {
        public MediaView()
        {
            this.InitializeComponent();
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (e.NavigationEventArgs.NavigationMode == NavigationMode.New || this.ViewModel == null)
                this.SetViewModel(new MediaViewModel());

            await base.OnLoadStateAsync(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            MediaPlayerHelper.CleanUpMediaPlayerSource(MediaElement.MediaPlayer);

            base.OnNavigatingFrom(e);
        }
    }

    /// <summary>
    /// Allows for disposal of the underlying MediaSources attached to a MediaPlayer, regardless
    /// of if a MediaSource or MediaPlaybackItem was passed to the MediaPlayer.
    ///
    /// It is left to the app to implement a clean-up of the other possible IMediaPlaybackSource
    /// type, which is a MediaPlaybackList.
    ///
    /// </summary>
    public static class MediaPlayerHelper
    {
        public static void CleanUpMediaPlayerSource(Windows.Media.Playback.MediaPlayer mp)
        {
            if (mp?.Source != null)
            {
                var source = mp.Source as Windows.Media.Core.MediaSource;
                source?.Dispose();

                var item = mp.Source as Windows.Media.Playback.MediaPlaybackItem;
                item?.Source?.Dispose();

                //var itemList = mp.Source as Windows.Media.Playback.MediaPlaybackList;
                //foreach (var playbackItem in itemList.Items)
                //{
                //    playbackItem?.Source?.Dispose();
                //}
            }
        }
    }
}
