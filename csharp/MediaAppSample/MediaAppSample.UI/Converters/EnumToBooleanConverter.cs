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

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MediaAppSample.UI.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public string EnumTypeName { get; set; }

        private Type EnumType { get; set; }
        
        public EnumToBooleanConverter()
        {
        }

        private void Init()
        {
            if (this.EnumType == null)
            {
                this.EnumType = Type.GetType(this.EnumTypeName);
                if (this.EnumType == null)
                    throw new ArgumentException(string.Format("EnumTypeName of '{0}' is not a valid type.", this.EnumTypeName));
            }
        }

        /// <summary>
        /// Compares the bound value with an enum param. Returns true when they match.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string)
            {
                this.Init();
                string valueEnum = Enum.GetName(this.EnumType, value);
                object parameterEnum = Enum.Parse(this.EnumType, parameter.ToString());
                if (parameterEnum == null)
                    throw new ArgumentException(string.Format("Paramter '{0}' is not an enumeration value in {0}.", parameter, this.EnumTypeName));
                return parameterEnum.ToString() == valueEnum;
            }
            else
            {
                if (value == null && parameter == null)
                    return true;
                else if (value == null && parameter != null)
                    return false;
                else if (value != null && parameter == null)
                    return false;
                else
                    return ((int)value).Equals((int)parameter);
            }
        }

        /// <summary>
        /// Converts the boolean back into an enum.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string)
            {
                if (value.Equals(true))
                {
                    this.Init();
                    object parameterEnum = Enum.Parse(this.EnumType, parameter.ToString());
                    if (parameterEnum == null)
                        throw new ArgumentException(string.Format("Paramter '{0}' is not an enumeration value in {0}.", parameter, this.EnumTypeName));
                    return parameterEnum;
                }
                else
                    return DependencyProperty.UnsetValue;
            }
            else
            {
                return value.Equals(true) ? parameter : DependencyProperty.UnsetValue;
            }
        }
    }
}
