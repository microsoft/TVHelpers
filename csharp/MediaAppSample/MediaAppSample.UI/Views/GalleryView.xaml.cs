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
using MediaAppSample.Core.Models;
using MediaAppSample.Core.ViewModels;
using System.Threading.Tasks;

namespace MediaAppSample.UI.Views
{
    public abstract class GalleryViewBase : ViewBase<GalleryViewModel>
    {
    }

    public sealed partial class GalleryView : GalleryViewBase
    {
        public GalleryView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        protected override async Task OnLoadStateAsync(LoadStateEventArgs e)
        {
            if (this.ViewModel == null)
            {
                if (e.Parameter is string)
                {
                    switch (e.Parameter.ToString())
                    {
                        case nameof(ItemTypes.TvSeries):
                        case nameof(ItemTypes.TvEpisode):
                            this.SetViewModel(Platform.Current.ViewModel.GalleryTvViewModel);
                            break;

                        default:
                            this.SetViewModel(Platform.Current.ViewModel.GalleryMoviesViewModel);
                            break;
                    }
                }
                else
                    this.SetViewModel(Platform.Current.ViewModel.GalleryMoviesViewModel);
            }

            await base.OnLoadStateAsync(e);
        }
    }
}
