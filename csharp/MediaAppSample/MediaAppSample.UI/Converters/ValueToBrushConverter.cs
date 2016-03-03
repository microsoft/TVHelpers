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
