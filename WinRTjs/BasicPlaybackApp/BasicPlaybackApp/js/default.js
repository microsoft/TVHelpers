
/*
#      The MIT License (MIT)
#
#      Copyright (c) 2016 Microsoft. All rights reserved.
#
#      Permission is hereby granted, free of charge, to any person obtaining a copy
#      of this software and associated documentation files (the "Software"), to deal
#      in the Software without restriction, including without limitation the rights
#      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#      copies of the Software, and to permit persons to whom the Software is
#      furnished to do so, subject to the following conditions:
#
#      The above copyright notice and this permission notice shall be included in
#      all copies or substantial portions of the Software.
#
#      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#      THE SOFTWARE.
*/
(function () {

    Windows.UI.WebUI.WebUIApplication.addEventListener('activated', function (args) {
        // Use the full screen width and height (i.e. turn off automatic Title Safe Area
        // This means your app design needs to take into account TSA.  
        if (Windows.Foundation.Metadata.ApiInformation.isTypePresent("Windows.UI.ViewManagement.ApplicationViewBoundsMode") && (Windows.System.Profile.AnalyticsInfo.versionInfo.deviceFamily === "Windows.Xbox")) {
            var appView = Windows.UI.ViewManagement.ApplicationView.getForCurrentView();
            appView.setDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.useCoreWindow); // Use all of the screen.
            //Try to disable app scaling (150% for a JS app, 200% for a XAML app)
            Windows.UI.ViewManagement.ApplicationViewScaling.trySetDisableLayoutScaling(true);
        }
        
        // Create the MediaPlayer
        var mediaPlayer = new TVJS.MediaPlayer(document.getElementById("mediaPlayer"));
        mediaPlayer.element.focus();

        // Add a Marker
        mediaPlayer.addMarker(3, TVJS.MarkerType.chapter, "scene 1", "customMarkerClass");
        mediaPlayer.addEventListener("markerreached", function (ev) {
            var markerData = ev.detail.data;
        });

        // Additional setup code here...
    });

})();