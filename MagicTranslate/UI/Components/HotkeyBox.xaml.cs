// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate.UI.Components
{
    public sealed partial class HotkeyBox : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Used to fill the TextBox the name of pressed keys like alt, shift and etc.
        /// </summary>
        private List<VirtualKey> previwModifyKeysList = new List<VirtualKey>();

        public VirtualKey _key = VirtualKey.None;

        /// <summary>
        /// Hotkey key.
        /// </summary>
        public VirtualKey Key
        {
            get { return _key; }
            set { _key = value; UpdateHotkeyTextBoxText(); }
        }

        public VirtualKeyModifiers _modifiers = VirtualKeyModifiers.None;

        /// <summary>
        /// Hotkey modifiers keys.(Ctrl, alt, win and shift)
        /// </summary>
        public VirtualKeyModifiers Modifiers
        {
            get { return _modifiers; }
            set { _modifiers = value; UpdateHotkeyTextBoxText(); }
        }

        public event EventHandler HotkeyChanged;

        public HotkeyBox()
        {
            this.InitializeComponent();
        }

        public List<VirtualKey> ParseVirtualKeyModifiers(VirtualKeyModifiers modifiers)
        {
            var modifyKeysInt = (int)modifiers;
            List<VirtualKey> parsedModifyKeysList = new List<VirtualKey>();

            if (modifyKeysInt >= (int)VirtualKeyModifiers.Windows)
            {
                modifyKeysInt -= (int)VirtualKeyModifiers.Windows;
                parsedModifyKeysList.Add(VirtualKey.LeftWindows);
            }

            if (modifyKeysInt >= (int)VirtualKeyModifiers.Shift)
            {
                modifyKeysInt -= (int)VirtualKeyModifiers.Shift;
                parsedModifyKeysList.Add(VirtualKey.Shift);
            }

            if (modifyKeysInt >= (int)VirtualKeyModifiers.Menu)
            {
                modifyKeysInt -= (int)VirtualKeyModifiers.Menu;
                parsedModifyKeysList.Add(VirtualKey.Menu);
            }

            if (modifyKeysInt >= (int)VirtualKeyModifiers.Control)
            {
                modifyKeysInt -= (int)VirtualKeyModifiers.Control;
                parsedModifyKeysList.Add(VirtualKey.Control);
            }

            return parsedModifyKeysList;
        }

        private void UpdateHotkeyTextBoxText()
        {
            if (Key != VirtualKey.None && Modifiers != VirtualKeyModifiers.None)
            {
                HotkeyTextBox.Text = GetHotkeyReadableString(ParseVirtualKeyModifiers(Modifiers)) + GetkeyReadableString(Key);
                RemoveHotkeyButton.Visibility = Visibility.Visible;
            }
            else
            {
                HotkeyTextBox.Text = string.Empty;
                RemoveHotkeyButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveHotkey(VirtualKeyModifiers modifiers, VirtualKey key)
        {
            Modifiers = modifiers;
            Key = key;
            RemoveFocusFromTextBox(HotkeyTextBox);
            HotkeyChanged?.Invoke(this, EventArgs.Empty);
        }

        private string GetHotkeyReadableString(List<VirtualKey> modifyKeys)
        {
            string hotkey = string.Empty;

            foreach (var key in modifyKeys)
            {
                string keyName = key.ToString();

                if (key == VirtualKey.Control)
                    keyName = "Ctrl";

                if (key == VirtualKey.Menu)
                    keyName = "Alt";

                if (key == VirtualKey.LeftWindows)
                    keyName = "Win";

                hotkey += keyName + " + ";
            }

            return hotkey;
        }

        private string GetkeyReadableString(VirtualKey key)
        {
            string keyString = key.ToString();

            if (keyString.Contains("NumberPad"))
                keyString = keyString.Replace("NumberPad", "NumPad ");

            if (keyString.Contains("Number"))
                keyString = keyString.Replace("Number", "");

            if (keyString == "Up")
                keyString = keyString.Replace("Up", "↑");


            if (keyString == "Down")
                keyString = keyString.Replace("Down", "↓");

            if (keyString == "Left")
                keyString = keyString.Replace("Left", "←");


            if (keyString == "Right")
                keyString = keyString.Replace("Right", "→");

            return keyString;
        }

        private bool IsModifyKey(VirtualKey key)
        {
            return key == VirtualKey.Control | key == VirtualKey.Menu | key == VirtualKey.Shift | key == VirtualKey.LeftWindows | key == VirtualKey.RightWindows;
        }

        private void RemoveFocusFromTextBox(TextBox textBox)
        {
            textBox.IsTabStop = false;
            textBox.IsEnabled = false;
            textBox.IsEnabled = true;
            textBox.IsTabStop = true;
        }

        private void TextBlock_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (e.Key == VirtualKey.Escape)
            {
                RemoveFocusFromTextBox(textBox);
                return;
            }

            if (IsModifyKey(e.Key))
            {
                var keydown = e.Key == VirtualKey.RightWindows ? VirtualKey.LeftWindows : e.Key;

                if (!previwModifyKeysList.Contains(keydown))
                {
                    Logger.Debug($"KeyDown {e.Key}");
                    previwModifyKeysList.Add(keydown);
                    textBox.Text = GetHotkeyReadableString(previwModifyKeysList);
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

        private void TextBlock_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (IsModifyKey(e.Key))
            {
                var keydown = e.Key == VirtualKey.RightWindows ? VirtualKey.LeftWindows : e.Key;

                if (previwModifyKeysList.Contains(keydown))
                {
                    Logger.Debug($"KeyUp {e.Key}");
                    previwModifyKeysList.Remove(keydown);
                    var textBox = sender as TextBox;
                    textBox.Text = GetHotkeyReadableString(previwModifyKeysList);
                    textBox.SelectionStart = textBox.Text.Length;
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HotkeyTextBox.Text = string.Empty;
            previwModifyKeysList.Clear();
            RemoveHotkeyButton.Visibility = Visibility.Collapsed;
        }


        private void TextBox_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args.Modifiers == VirtualKeyModifiers.None) return;

            Logger.Debug($"Hotkey {args.Modifiers} + {args.Key}");
            if (Enum.GetName(typeof(VirtualKey), args.Key) != null)
            {
                SaveHotkey(args.Modifiers, args.Key);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateHotkeyTextBoxText();
        }

        private void HotkeyBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        private void RemoveHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            SaveHotkey(VirtualKeyModifiers.None, VirtualKey.None);
        }
    }
}
