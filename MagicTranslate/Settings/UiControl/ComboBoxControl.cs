using MagicTranslate.Helpers;
using MagicTranslate.UI.Components;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MagicTranslate.Settings.UiControl
{
    public static class ComboBoxControl
    {
        public static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedItem = (TextBlock)comboBox.SelectedItem;
            var settingsItem = FindMyParentHelper.FindMyParent<SettingsItem>.FindAncestor(comboBox);
            if (settingsItem != null && settingsItem.Tag != null)
            {
                GlobalSettings.SaveHeadphoneSetting((string)settingsItem.Tag, (string)comboBox.Tag, selectedItem.Tag);
            }
        }

        //Note! Supported only TextBox items
        public static void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var settingsItem = FindMyParentHelper.FindMyParent<SettingsItem>.FindAncestor(comboBox);
            if (settingsItem != null && settingsItem.Tag != null)
            {
                object setting = GlobalSettings.LoadHeadphoneSetting((string)settingsItem.Tag, (string)comboBox.Tag);
                if (setting != null)
                {
                    var settingString = (string)setting;
                    foreach (var item in comboBox.Items)
                    {
                        var Tbox = (TextBlock)item;
                        if ((string)Tbox.Tag == settingString)
                        {
                            comboBox.SelectedItem = Tbox;
                            return;
                        }
                    }                
                }
            }
        }
    }
}
