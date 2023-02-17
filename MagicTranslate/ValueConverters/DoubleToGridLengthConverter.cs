using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace MagicTranslate.ValueConverters
{
    internal class DoubleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {                        
            return new GridLength((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var gridLength = (GridLength)value;            
            return gridLength.Value;
        }
    }
}
