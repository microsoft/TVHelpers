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
using Windows.UI.Xaml;

namespace MediaAppSample.UI.Controls
{
	/// <summary>
	/// Event args for a value changing event
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
		/// <summary>
		/// Gets the old value.
		/// </summary>
        public T OldValue { get; private set; }
		/// <summary>
		/// Gets the new value.
		/// </summary>
        public T NewValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
