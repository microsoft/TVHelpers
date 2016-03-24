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
using Windows.UI.Xaml.Media;

namespace MediaAppSample.UI.Converters
{
    public sealed class ValueToBrushConverter : ValueToBooleanConverter
    {
        public Brush TrueBrush { get; set; }
        public Brush FalseBrush { get; set; }
        
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolean = (bool)base.Convert(value, targetType, parameter, language);
            return boolean ? this.TrueBrush : this.FalseBrush;
        }

        public object ConvertBackCore(object value, Type targetType, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
