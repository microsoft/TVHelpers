using System;
using Windows.UI.Xaml;

namespace MediaAppSample.UI.Converters
{
    public sealed class ValueToVisibilityConverter : ValueToBooleanConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolean = (bool)base.Convert(value, targetType, parameter, language);
            return boolean ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
