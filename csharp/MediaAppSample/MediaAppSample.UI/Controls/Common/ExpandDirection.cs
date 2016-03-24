//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;

namespace MediaAppSample.UI.Controls
{
	/// <summary>
	/// Expand Direction used by the <see cref="Primitives.LinearClipper" />
	/// </summary>
	/// <seealso cref="Primitives.LinearClipper.ExpandDirection"/>
    public enum ExpandDirection
    {
		/// <summary>
		/// Down
		/// </summary>
        Down = 0,
		/// <summary>
		/// Up
		/// </summary>
        Up = 1,
		/// <summary>
		/// The left
		/// </summary>
        Left = 2,
		/// <summary>
		/// The right
		/// </summary>
        Right = 3,
    }
}
