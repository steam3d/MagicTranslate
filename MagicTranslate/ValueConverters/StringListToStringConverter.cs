using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace MagicTranslate.ValueConverters
{
    internal class StringListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var listStrings = value as List<string>;
            if (listStrings != null)
            {
                string mergedString = string.Empty;
                foreach (string str in listStrings)
                {
                    mergedString += str + ", ";
                }
                return mergedString.Substring(0, mergedString.Length-2);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new List<string>();
        }
    }
}
