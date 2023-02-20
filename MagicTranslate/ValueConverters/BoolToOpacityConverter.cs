using Microsoft.UI.Xaml.Data;
using System;

namespace MagicTranslate.ValueConverters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? 1.0 : 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (double)value == 1.0;
        }
    }
}