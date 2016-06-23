ONLY THE SOURCE AND BINARY CODE IS LICENSED UNDER THE MIT LICENSE.  ANY INCLUDED IMAGES ARE PROVIDED FOR INFORMATIONAL PURPOSES ONLY, NO FURTHER USE OR DISTRIBUTION OF THESE IMAGES IS ALLOWED.

#The MIT License (MIT)

#Copyright (c) 2016 Microsoft. All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
#
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
#
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


#Media App Sample v0.9
The Media App Sample is a Windows 10 UWP App cross-device sample that demonstrates responsive user experience techniques supporting Desktop, Laptop, Tablet, Phone, and Xbox One.  The sample includes many diferent features in addition to the responsive User Experience such as background tasks, authentication, app rating, and storage management, all integrated into a single Universal Windows App sample.

While the domain of the sample is media, this applicaiton can be used as a basis for your application by simply removing the Views, ViewModels, and sample data to start a new application without losing the rest of the application sample functionality.

#Planned Updates
1. Refine Xbox One support for the latest SDK.
2. Update playback to take advantage of the new Media client improvments available in Windows 10 Anniversary edition.
3. Address known issues.

#Known Issues
1. The sample is updated to use Windows 10 Anniversary Edition Insider Preview (10.0; Build 14366).  You can download the latest preview SDK from https://insider.windows.com/ after you sign-in, under the "for developers" section.
2. When you attempt playback, you will be prompted to login with MSA or an account.  Choose "Sign in" and enter anything for username and password.  Currently it kicks you back to the home page---a bug.  However if you navigate back to playback it will play the video.
3. There are three known native exceptions that can occur if debugging natively and all C++, win32, and CLR exceptions are enabled:
    a. Obtaining user credentials for the first time upon launch, when searching the credential locker a System.Runtime.InteropServices.COMException (0x40080201) excetion is thrown.  (This is expected for the API when credentials are not found)
    b. When loading te Voice Command File, a EETypeLoadException exception is thrown.
    c. Upon app launch, an exception in KernelBase.dll (0x40080201) is thrown.
