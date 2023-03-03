using System;

namespace MagicTranslate.Hotkeys
{
    /// <summary>
    /// Contains data necessary to bind hotkey
    /// </summary>
    public class HotkeyData
    {
        /// <summary>
        /// Used as action name to detect which action to do
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Data to pass to action
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        /// Id binded to RegisterCombo
        /// </summary>
        public Int32 ID { get; set; }

        /// <summary>
        /// Modifiers binded to RegisterCombo
        /// </summary>
        public int Modifiers { get; set; }

        /// <summary>
        /// Keys <see cref="VirtualKey"/> binded to RegisterCombo
        /// </summary>
        public int Key { get; set; }

        public HotkeyData(string containerName, string settingName, int modifiers, int key, int id)
        {
            ContainerName = containerName;
            SettingName = settingName;
            Modifiers = modifiers;
            Key = key;
            ID = id;
        }

    }
}
