
ONLY THE SOURCE AND BINARY CODE IS LICENSED UNDER THE MIT LICENSE.  ANY INCLUDED IMAGES ARE PROVIDED FOR INFORMATIONAL PURPOSES ONLY, NO FURTHER USE OR DISTRIBUTION OF THESE IMAGES IS ALLOWED.

#Copyright (c) 2016 Microsoft. All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
#
#
#
#Sample Notes
This project does not include any third party script libraries.  
You can test out loading a third-party script such as ReactJS by following these steps.

1. Download ReactJS from http://facebook.github.io/react/.
2. Copy the ReactJS files to the \scriptlibrary folder.
3. Copy base.css from the ReactJS basic example to \scriptlibrary\base.css.
3. Copy the ReactJS basic sample html file to the project route and rename to index.html.
4. Modify index.html to include base.css and reactjs from the \scriptlibrary folder.
5. Default.html will load index.html into an iframe:  <iframe id="ui" src="ms-appx-web:///index.html" aria-label="UI"></iframe>.
6. Set a breakpoint in default.js and run the project.  
7. Once you hit a breakpoint, go to the Debug | Windows menu in Visual Studio 2015 and select JavaScript Console.
8. Type "window.Windows." in the JavaScript Console to see that you have access to the Windows Runtime API as a result of the <uap:Rule Match="ms-appx-web:///" Type="include" WindowsRuntimeAccess="all" /> line in the package.appxmanifest.
   Note: Recommend only granting access to the APIs required for your app when submitting to the store as a best practice.
9. You may need to open the package.appxmanifest file using the XML editor to inspect the configuration.

Please note, any third-party script library can be used to build your web app on Windows 10.  This smaple just happens to demonstrate key concepts using ReactJS.