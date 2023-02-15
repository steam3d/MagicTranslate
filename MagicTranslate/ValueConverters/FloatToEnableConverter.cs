using Microsoft.UI.Xaml.Data;
using System;

namespace MagicTranslate.ValueConverters
{
    internal class FloatToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {            
            if (value != null && parameter != null)
            {
                var score = (float)value;                
                var position = int.Parse((string)parameter);

                float bestScore = 0;
                switch (position)
                {
                    case 3:
                        bestScore = 0.06f;
                        break;
                    case 2:
                        bestScore = 0.003f;
                        break;
                    case 1:
                        bestScore = 0f;
                        break;
                }

                var round = (float)Math.Round(score, 3);

                if (round >= bestScore)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0;
        }
    }
}
