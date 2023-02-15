using MagicTranslate.Extensions;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using TranslateLibrary;

namespace MagicTranslate.ValueConverters
{
    internal class ListSentencesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var listSentences = value as List<Sentences>;
            if (listSentences != null)
            {
                string mergedString = string.Empty;
                foreach (var item in listSentences)
                {

                    mergedString += item.trans;
                }
                return mergedString.ToFirstCharUpper() ?? string.Empty;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new List<string>();
        }
    }
}
