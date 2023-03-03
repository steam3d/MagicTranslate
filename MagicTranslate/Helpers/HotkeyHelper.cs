using MagicTranslate.Hotkeys;
using Windows.System;

namespace MagicTranslate.Helpers
{
    public static class HotkeyHelper
    {
        public static string GetReadableStringFromHotkey(int modifiersInt, int key)
        {
            string hotkeyString = string.Empty;

            if (modifiersInt >= (int)Modifiers.NoRepeast)
            {
                //VirtualKeyModifiers does not have NoRepeast
                modifiersInt -= (int)Modifiers.NoRepeast;
            }

            if (modifiersInt >= (int)Modifiers.Windows)
            {
                modifiersInt -= (int)Modifiers.Windows;
                hotkeyString += "Win + ";               
            }

            if (modifiersInt >= (int)Modifiers.Shift)
            {
                modifiersInt -= (int)Modifiers.Shift;
                hotkeyString += "Shift + ";
            }

            if (modifiersInt >= (int)Modifiers.Control)
            {
                modifiersInt -= (int)Modifiers.Control;
                hotkeyString += "Ctrl + ";
            }

            if (modifiersInt >= (int)Modifiers.Alt)
            {
                // I have no idea why alt = menu
                modifiersInt -= (int)Modifiers.Alt;
                hotkeyString += "Alt + ";
            }

            //Make hotkey string the same in UWP. Modifiers have swapped Control and Alt 
            if (hotkeyString.Contains("Alt") && hotkeyString.Contains("Ctrl"))
            {
                hotkeyString = hotkeyString.Replace("Alt", "tmp");
                hotkeyString = hotkeyString.Replace("Ctrl", "Alt");
                hotkeyString = hotkeyString.Replace("tmp", "Ctrl");
            }
            
            hotkeyString += ((VirtualKey)key).ToString();

            return hotkeyString;
        }
    }
}
