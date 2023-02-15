using MagicTranslate.Extensions;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTranslate.ValueConverters
{
    internal class FirstCharToUpperStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var text = value.ToString();
            return text.ToFirstCharUpper() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var text = value.ToString();
            return text.ToFirstCharLower() ?? string.Empty;
        }
    }
}
