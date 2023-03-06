using MagicTranslate.UI.Theme;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace MagicTranslate.ValueConverters
{
    internal class FloatToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isDarkTheme = ThemeManagement.IsDarkTheme();
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
                {
                    var uiSettings = new UISettings();
                    UIColorType uIColorType = isDarkTheme ? UIColorType.AccentLight2 : UIColorType.AccentDark2;              
                    return new SolidColorBrush(uiSettings.GetColorValue(uIColorType));
                    //return (SolidColorBrush)Application.Current.Resources["AltSystemAccentColor"];
                }
            }
#warning Hardcoded colors
            var color = isDarkTheme ? Color.FromArgb(255, 64, 64, 64) : Color.FromArgb(255, 207, 207, 207);
            return new SolidColorBrush(color);
            //return (SolidColorBrush)Application.Current.Resources["ContextMenuBorderBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0;
        }
    }
}
