using System;
using Windows.UI.Xaml.Data;

namespace MediaAppSample.UI.Converters
{
    public class ValueToDoubleConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, string language)
        {
            double returnValue = 0;

            if (value is double)
            {
                returnValue = (double)value;
            }
            else if (value != null)
            {
                double.TryParse(value.ToString(), out returnValue);
            }

            return returnValue;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
