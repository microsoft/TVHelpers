using System;

namespace MediaAppSample.UI.Converters
{
    public sealed class ValueToOpacityConverter : ValueToBooleanConverter
    {
        public double TrueOpacity { get; set; }
        public double FalseOpacity { get; set; }

        public ValueToOpacityConverter()
        {
            this.TrueOpacity = 1;
            this.FalseOpacity = .3;
        }

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolean = (bool)base.Convert(value, targetType, parameter, language);
            return boolean ? this.TrueOpacity : this.FalseOpacity;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
