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

using MediaAppSample.Core;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MediaAppSample.UI.Converters
{
    public sealed class ColorToBrushConverter : IValueConverter
    {
        public SolidColorBrush DefaultBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                return new SolidColorBrush(GeneralFunctions.HexToColor(value.ToString()));
            else if (parameter != null && parameter is string)
                return new SolidColorBrush(GeneralFunctions.HexToColor(parameter.ToString()));
            else if (parameter != null && parameter is Color)
                return new SolidColorBrush((Color)parameter);
            else
                return this.DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
