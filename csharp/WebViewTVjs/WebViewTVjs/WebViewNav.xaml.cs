using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WebViewTVjs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebViewNav : Page
    {
        public WebViewNav()
        {
            this.InitializeComponent();

            LoadHTMLContent();
        }

        private void LoadHTMLContent()
        {
            // WebViewControl.Navigate(new Uri("ms-appx-web:///html/BasicDirectionalNavigation.html"));
            WebViewControl.Navigate(new Uri("http://www.microsoft.com"));

            WebViewControl.Focus(FocusState.Programmatic);
        }

        private void WebViewControl_OnFrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // Do something if needed on navigation within the web content
            StatusBlock.Text = "Loading....";
            ProgressIndicator.IsActive = true;
        }

        private void WebViewControl_OnFrameNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // Do something if needed when navigation is completed
            StatusBlock.Text = "Content Loaded!";
            ProgressIndicator.IsActive = false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (WebViewControl.CanGoBack)  // Check if the back stack is not zero
            {
                WebViewControl.GoBack();  // Navigate one back in the back stack
            }
            else
            {
                //If back stack cannot go back for web content, then navigate away from the XAML page hosting the WebView
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame?.GoBack();
            }
        }
    }
}
