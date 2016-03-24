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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaAppSample.UI.Controls
{
	/// <summary>
	/// Value changed event delegate
	/// </summary>
	/// <typeparam name="T">Value type</typeparam>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The <see cref="MediaAppSample.UI.Controls.ValueChangedEventArgs{T}"/> instance containing the event data.</param>
    public delegate void ValueChangedEventHandler<T>(object sender, ValueChangedEventArgs<T> e);
}
