using MagicTranslate.Hotkeys;
using Windows.System;

namespace MagicTranslate.Helpers
{
    public static class HotkeyHelper
    {
        /// <summary>
        /// Ger readable hotkey string for Win32 hotkey
        /// </summary>
        /// <param name="modifiersInt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetReadableStringFromHotkey(int modifiersInt, int key, string separator = "")
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
                hotkeyString += $"Win{separator}+{separator}";               
            }

            if (modifiersInt >= (int)Modifiers.Shift)
            {
                modifiersInt -= (int)Modifiers.Shift;
                hotkeyString += $"Shift{separator}+{separator}";
            }

            if (modifiersInt >= (int)Modifiers.Control)
            {
                modifiersInt -= (int)Modifiers.Control;
                hotkeyString += $"Ctrl{separator}+{separator}";
            }

            if (modifiersInt >= (int)Modifiers.Alt)
            {
                // I have no idea why alt = menu
                modifiersInt -= (int)Modifiers.Alt;
                hotkeyString += $"Alt{separator}+{separator}";
            }

            //Make hotkey string the same in UWP.Modifiers have swapped Control and Alt
            if (hotkeyString.Contains("Alt") && hotkeyString.Contains("Ctrl"))
            {
                hotkeyString = hotkeyString.Replace("Alt", "tmp");
                hotkeyString = hotkeyString.Replace("Ctrl", "Alt");
                hotkeyString = hotkeyString.Replace("tmp", "Ctrl");
            }

            hotkeyString += ((VirtualKey)key).ToString();

            return hotkeyString;
        }

        public static string GetReadableStringFromHotkey(VirtualKeyModifiers modifiers, VirtualKey key, string separator = "")
        {
            string hotkeyString = string.Empty;
            var modifiersInt = (int)modifiers;

            if (modifiersInt >= (int)VirtualKeyModifiers.Windows)
            {
                modifiersInt -= (int)VirtualKeyModifiers.Windows;
                hotkeyString += $"Win{separator}+{separator}";
            }

            if (modifiersInt >= (int)VirtualKeyModifiers.Shift)
            {
                modifiersInt -= (int)VirtualKeyModifiers.Shift;
                hotkeyString += $"Shift{separator}+{separator}";
            }

            if (modifiersInt >= (int)VirtualKeyModifiers.Menu)
            {
                modifiersInt -= (int)VirtualKeyModifiers.Menu;
                hotkeyString += $"Alt{separator}+{separator}";
            }

            if (modifiersInt >= (int)VirtualKeyModifiers.Control)
            {
                // I have no idea why alt = menu
                modifiersInt -= (int)Modifiers.Control;
                hotkeyString += $"Ctrl{separator}+{separator}";
            }

            hotkeyString += key.ToString();

            return hotkeyString;
        }
    }
}
