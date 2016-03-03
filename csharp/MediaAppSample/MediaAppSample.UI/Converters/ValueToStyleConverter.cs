using System;
using Windows.UI.Xaml;

namespace MediaAppSample.UI.Converters
{
    public sealed class ValueToStyleConverter : ValueToBooleanConverter
    {
        public Style TrueStyle { get; set; }
        public Style FalseStyle { get; set; }

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolean = (bool)base.Convert(value, targetType, parameter, language);
            return boolean ? this.TrueStyle : this.FalseStyle;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
