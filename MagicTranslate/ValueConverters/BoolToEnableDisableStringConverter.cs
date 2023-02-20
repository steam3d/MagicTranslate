using Microsoft.UI.Xaml.Data;
using System;

namespace MagicTranslate.ValueConverters
{
    public class BoolToEnableDisableStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
            if (!(bool)value)
            {
                return resourceLoader.GetString("Settings_AppSettings_ToggleButtonStartup_Enable/Content");
            }
            else
            {
                return resourceLoader.GetString("Settings_AppSettings_ToggleButtonStartup_Disable/Content");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
