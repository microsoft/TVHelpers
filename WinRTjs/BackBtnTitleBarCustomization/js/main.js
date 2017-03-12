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
(function mainModule() {
    "use strict";

    var systemNavManager = Windows.UI.Core.SystemNavigationManager.getForCurrentView();
    function startApp(){
        var defaultNavbtn = document.querySelector(".navToDetails");
        if (defaultNavbtn !== null) {
            defaultNavbtn.addEventListener("click", defaultToDetails, false);            
        }

        // backRequested event
        if (systemNavManager !== null) {
            systemNavManager.addEventListener("backrequested", backRequested, false);
            UpdateBackButtonVisibility();
        }

        var applicationView = Windows.UI.ViewManagement.ApplicationView;

        if (applicationView !== null) {
            var customColors = {
                backgroundColor: { a: 255, r: 24, g: 60, b: 216 },
                foregroundColor: { a: 255, r: 132, g: 211, b: 162 },
                buttonBackgroundColor: { a: 255, r: 24, g: 60, b: 216 },
                buttonForegroundColor: { a: 255, r: 132, g: 211, b: 162 },
                buttonHoverBackgroundColor: { a: 255, r: 19, g: 21, b: 40 },
                buttonHoverForegroundColor: { a: 255, r: 255, g: 255, b: 255 },
                buttonPressedBackgroundColor: { a: 255, r: 132, g: 211, b: 162 },
                buttonPressedForegroundColor: { a: 255, r: 24, g: 60, b: 216 },
                inactiveBackgroundColor: { a: 255, r: 135, g: 141, b: 199 },
                inactiveForegroundColor: { a: 255, r: 132, g: 211, b: 162 },
                buttonInactiveBackgroundColor: { a: 255, r: 135, g: 141, b: 199 },
                buttonInactiveForegroundColor: { a: 255, r: 132, g: 211, b: 162 },
            };

            var titleBar = applicationView.getForCurrentView().titleBar;
            if (titleBar != null) {
                titleBar.buttonBackgroundColor = customColors.buttonBackgroundColor;
                titleBar.buttonForegroundColor = customColors.buttonForegroundColor;
                titleBar.buttonHoverForegroundColor = customColors.buttonHoverForegroundColor;
                titleBar.backgroundColor = customColors.backgroundColor;
                titleBar.foregroundColor = customColors.foregroundColor;
            }
        }
    }

    function backRequested() {
        if (window.location.href.indexOf("/default.html") === -1) {  // not on home page, go back
            // nav back
            window.history.back();
        }
    }

    if (window.addEventListener) {
        window.addEventListener('load', startApp, false);
    }
    
    function defaultToDetails(sender) {

        window.location.href = '/details.html';
    }

    // Hide  show back button in title bar depending on where you are in the navigation back stack
    function UpdateBackButtonVisibility()
    {
        if (systemNavManager !== null)
        {
            if (window.location.href.indexOf("/default.html") === -1)  {
                systemNavManager.appViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.visible;
            } else {
                systemNavManager.appViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.collapsed;
            };
        }
    }

})();