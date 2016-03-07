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
