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
using Windows.UI.Xaml.Data;

namespace MediaAppSample.UI.Converters
{
    public abstract class StringCasingConverter : IValueConverter
    {
        protected bool IsLower { get; set; }
        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is string)
            {
                if (this.IsLower)
                    return value.ToString().ToLower();
                else
                    return value.ToString().ToUpper();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StringLowerCasingConverter : StringCasingConverter
    {
        public StringLowerCasingConverter()
        {
            this.IsLower = true;
        }
    }

    public sealed class StringUpperCasingConverter : StringCasingConverter
    {
        public StringUpperCasingConverter()
        {
            this.IsLower = false;
        }
    }
}
