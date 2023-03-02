using MagicTranslate.Helpers;
using MagicTranslate.UI.Components;
using Microsoft.UI.Xaml;
using System;
using Windows.System;

namespace MagicTranslate.Settings.UiControl
{
    public static class HotkeyBoxControl
    {
        public static void HotkeyBox_Loaded(object sender, RoutedEventArgs e)
        {
            var hotkeyBox = (HotkeyBox)sender;
            var settingItem = FindMyParentHelper.FindMyParent<SettingsItem>.FindAncestor(hotkeyBox);
            if (settingItem != null)
            {
                object setting = GlobalSettings.LoadHeadphoneSetting((string)settingItem.Tag, (string)hotkeyBox.Tag);
                if (setting != null)
                {
                    Windows.Storage.ApplicationDataCompositeValue composite = (Windows.Storage.ApplicationDataCompositeValue)setting;
                    if (composite.Count > 0)
                    {
                        if (composite.TryGetValue("modifiers", out object modifiers))
                            hotkeyBox.Modifiers = (VirtualKeyModifiers)ModifiersHelper.ConvertModifiersIntToVirtualKeyModifiersInt((int)modifiers);

                        if (composite.TryGetValue("key", out object key))
                            hotkeyBox.Key = (VirtualKey)key;
                    }
                }
            }
        }

        public static void HotkeyBox_HotkeyChanged(object sender, EventArgs e)
        {
            var hotkeyBox = (HotkeyBox)sender;
            var settingItem = FindMyParentHelper.FindMyParent<SettingsItem>.FindAncestor(hotkeyBox);
            if (settingItem != null && settingItem.Tag != null)
            {
                Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
                if (hotkeyBox.Modifiers != VirtualKeyModifiers.None && hotkeyBox.Key != VirtualKey.None)
                {
                    //composite.Add("data", (string)hotkeyBox.Tag);
                    composite.Add("modifiers", ModifiersHelper.ConvertVirtualKeyModifiersIntToModifiersInt((int)hotkeyBox.Modifiers));
                    composite.Add("key", (int)hotkeyBox.Key);
                    GlobalSettings.SaveHeadphoneSetting((string)settingItem.Tag, (string)hotkeyBox.Tag, composite);
                }                
            }
        }
    }
}
