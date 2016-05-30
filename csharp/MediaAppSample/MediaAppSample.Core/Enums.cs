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

namespace MediaAppSample.Core
{
    #region Enumerations

    /// <summary>
    /// Enumeration used to indicate if core code is executing in a new instance of the application, 
    /// if it were resumed, or if executing in the background.
    /// </summary>
    public enum InitializationModes
    {
        /// <summary>
        /// New instance of the application launched.
        /// </summary>
        New,

        /// <summary>
        /// App restored from a suspended state.
        /// </summary>
        Restore,

        /// <summary>
        /// App background task launched.
        /// </summary>
        Background
    }

    /// <summary>
    /// Device families supported by Windows.
    /// </summary>
    public enum DeviceFamily
    {
        Unknown,
        Xbox,
        Desktop,
        Mobile,
        IoT,
    }

    /// <summary>
    /// Enumeration representing each way maps can be displayed.
    /// </summary>
    public enum MapExternalOptions
    {
        /// <summary>
        /// Standard directions
        /// </summary>
        Normal,

        /// <summary>
        /// Directions for driving
        /// </summary>
        DrivingDirections,

        /// <summary>
        /// Directions for walking
        /// </summary>
        WalkingDirections
    }

    #endregion
}
