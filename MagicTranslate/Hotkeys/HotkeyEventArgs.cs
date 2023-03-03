using System;

namespace MagicTranslate.Hotkeys
{
    public class HotkeyEventArgs : EventArgs
    {
        public string SettingName { get; }
        public string Data { get; }

        public string ReadableHotkey { get; }

        public HotkeyEventArgs(string settingName, string data, string readableHotkey)
        {
            SettingName = settingName;
            Data = data;
            ReadableHotkey = readableHotkey;
        }
    }
}
