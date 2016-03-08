using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaAppSample.UI.Controls
{
    public class ImageControl : Control
    {
        public static DependencyProperty _DecodePixelHeightProperty =
            DependencyProperty.Register(
            "DecodePixelHeight",
            typeof(int),
            typeof(ImageControl),
            new PropertyMetadata(0)
            );

        public static DependencyProperty _DecodePixelWidthProperty =
            DependencyProperty.Register(
            "DecodePixelWidth",
            typeof(int),
            typeof(ImageControl),
            new PropertyMetadata(0)
            );

        public static DependencyProperty _DownloadProgressProperty =
            DependencyProperty.Register(
            "DownloadProgress",
            typeof(int),
            typeof(ImageControl),
            new PropertyMetadata(0, new PropertyChangedCallback(HandleDownloadProgressChanged))
            );

        public static DependencyProperty _StretchProperty =
            DependencyProperty.Register(
            "Stretch",
            typeof(Stretch),
            typeof(ImageControl),
            new PropertyMetadata(Stretch.None)
            );

        public static DependencyProperty _SourceProperty =
            DependencyProperty.Register(
            "Source",
            typeof(Uri),
            typeof(ImageControl),
            new PropertyMetadata(null, new PropertyChangedCallback(HandleUriSourceChanged))
            );

        public int DownloadProgress
        {
            get
            {
                return (int)this.GetValue(_DownloadProgressProperty);
            }
            set
            {
                this.SetValue(_DownloadProgressProperty, value);
            }
        }

        public Uri Source
        {
            get
            {
                return this.GetValue(_SourceProperty) as Uri;
            }
            set
            {
                this.SetValue(_SourceProperty, value);
            }
        }

        public Stretch Stretch
        {
            get
            {
                return (Stretch)this.GetValue(_StretchProperty);
            }
            set
            {
                this.SetValue(_StretchProperty, value);
            }
        }

        public ImageControl()
        {
            DefaultStyleKey = typeof(ImageControl);
            IsTabStop = false;
        }

        private void AttachToBitmapImage(BitmapImage bitmapImage)
        {
            if (bitmapImage != null)
            {
                bitmapImage.DownloadProgress += BitmapImage_DownloadProgress;
                bitmapImage.ImageFailed += BitmapImage_ImageFailed;
                DownloadProgress = 0;
                _bitmapImage.UriSource = Source;
            }
        }

        private void BitmapImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "ImageFailed", true);
        }

        private void BitmapImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            DownloadProgress = e.Progress;
        }

        private void DetachFromBitmapImage(BitmapImage bitmapImage)
        {
            if (bitmapImage != null)
            {
                bitmapImage.DownloadProgress -= BitmapImage_DownloadProgress;
                bitmapImage.ImageFailed -= BitmapImage_ImageFailed;
                _bitmapImage.UriSource = null;
            }
        }

        private static void HandleDownloadProgressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ic = sender as ImageControl;
            int progress = (int)e.NewValue;
            if (progress == 0 || progress == 100)
            {
                ic.UpdateVisualState(true);
            }
        }


        private static void HandleUriSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ic = sender as ImageControl;
            if (ic._bitmapImage != null)
            {
                var oldUri = e.OldValue as Uri;
                var newUri = e.NewValue as Uri;
                ic._bitmapImage.UriSource = newUri;
                if (oldUri == null
                    || newUri == null
                    || oldUri.AbsoluteUri != newUri.AbsoluteUri)
                {
                    ic.DownloadProgress = 0;
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            DetachFromBitmapImage(_bitmapImage);
            _bitmapImage = (BitmapImage)this.GetTemplateChild("BitmapImage");
            AttachToBitmapImage(_bitmapImage);
        }

        private void UpdateVisualState(bool animate)
        {
            if (DownloadProgress < 100)
            {
                VisualStateManager.GoToState(this, "Normal", animate);
            }
            else
            {
                VisualStateManager.GoToState(this, "ImageLoaded", animate);
            }
        }

        private BitmapImage _bitmapImage = null;
    }
}
