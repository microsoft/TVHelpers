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
using System.Collections;
using Windows.UI.Xaml.Data;

namespace MediaAppSample.UI.Converters
{
    public class ValueToBooleanConverter : IValueConverter
    {
        public bool InvertValue { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolean = true;

            if (value is bool)
            {
                boolean = (bool)value;
            }
            else if (value is string)
            {
                boolean = !string.IsNullOrWhiteSpace(value.ToString());
            }
            else if (value is DateTime)
            {
                var val = (DateTime)value;
                boolean = !(val == DateTime.MinValue || val == DateTime.MaxValue);
            }
            else if (value is int)
            {
                var val = (int)value;
                boolean = !(val == int.MinValue || val == int.MaxValue);
            }
            else if (value is double)
            {
                var val = (double)value;
                boolean = !(val == double.MinValue || val == double.MaxValue || double.IsNaN(val));
            }
            else if (value is long)
            {
                var val = (long)value;
                boolean = !(val == long.MinValue || val == long.MaxValue);
            }
            else if (value is float)
            {
                var val = (float)value;
                boolean = !(val == float.MinValue || val == float.MaxValue);
            }
            else if (value is IEnumerable)
            {
                var val = value as IEnumerable;
                boolean = val.GetEnumerator().MoveNext();
            }
            else if (value == null)
            {
                boolean = false;
            }

            if (InvertValue || parameter != null)
                boolean = !boolean;

            return boolean;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
                return this.InvertValue || parameter != null ? !((bool)value) : value;
            else
                throw new NotImplementedException();
        }
    }
}
