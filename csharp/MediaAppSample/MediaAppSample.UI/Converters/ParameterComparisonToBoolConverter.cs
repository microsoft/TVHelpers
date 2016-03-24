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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MediaAppSample.UI.Converters
{
    public class ParameterComparisonToBoolConverter : IValueConverter
    {
        public ParameterComparisonToBoolConverter()
        {
        }
        
        /// <summary>
        /// Compares the bound value with an enum param. Returns true when they match.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null && parameter == null)
                return true;
            else if ((value == null && parameter != null) || (value != null && parameter == null))
                return false;
            else
                return System.Collections.Generic.EqualityComparer<object>.Default.Equals(value, parameter);
        }

        /// <summary>
        /// Converts the boolean back into an enum.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.Equals(true) ? parameter : DependencyProperty.UnsetValue;
        }
    }
}
