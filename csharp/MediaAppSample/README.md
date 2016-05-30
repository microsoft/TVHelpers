ONLY THE SOURCE AND BINARY CODE IS LICENSED UNDER THE MIT LICENSE.  ANY INCLUDED IMAGES ARE PROVIDED FOR INFORMATIONAL PURPOSES ONLY, NO FURTHER USE OR DISTRIBUTION OF THESE IMAGES IS ALLOWED.

#The MIT License (MIT)

#Copyright (c) 2016 Microsoft. All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
#
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
#
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


#Media App Sample
The Media App Sample is a Windows 10 UWP App cross-device sample that demonstrates responsive user experience techniques supporting Desktop, Laptop, Tablet, Phone, and Xbox One.  The sample includes many diferent features in addition to the responsive User Experience such as background tasks, authentication, app rating, and storage management, all integrated into a single Universal Windows App sample.

While the domain of the sample is media, this applicaiton can be used as a basis for your application by simply removing the Views, ViewModels, and sample data to start a new application without losing the rest of the application sample functionality.

#Known Issues
1. The sample is still in development for the Xbox One platform.  The preview SDK released at //build does not provide full UWP support.  Plan is to fully support Xbox One once a new SDK with full Xbox One support is available.
2. There are three known native exceptions that can occur if debugging natively and all C++, win32, and CLR exceptions are enabled:
    a. Obtaining user credentials for the first time upon launch, when searching the credential locker a System.Runtime.InteropServices.COMException (0x40080201) excetion is thrown.  (This is expected for the API when credentials are not found)
    b. When loading te Voice Command File, a EETypeLoadException exception is thrown.
    c. Upon app launch, an exception in KernelBase.dll (0x40080201) is thrown.