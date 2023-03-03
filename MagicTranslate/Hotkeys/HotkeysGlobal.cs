using MagicTranslate.Helpers;
using MagicTranslate.Settings;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace MagicTranslate.Hotkeys
{
    internal class HotkeysGlobal
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private List<HotkeyData> hotkeyDataList = new List<HotkeyData>();

        /// <summary>
        /// Main hotkey logic
        /// </summary>
        private HotkeysWin32Logic hotkeyWin32;

        /// <summary>
        /// Unique id for hotkeys
        /// </summary>
        private int idCounter = 0;


        /// <summary>
        /// Event rise when any registered hotkey was pressed
        /// </summary>
        public event EventHandler<HotkeyEventArgs> HotkeyPressed;

        public HotkeysGlobal(Window window)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            hotkeyWin32 = new HotkeysWin32Logic(hWnd);

            hotkeyWin32.HotkeyPressed += HotkeyWindow_HotkeyPressed;
            RegisterAllCombos();
            GlobalSettings.OnSettingChange += GlobalSettings_OnSettingChange;
        }
        private void GlobalSettings_OnSettingChange(object sender, Args.SettingArgs e)
        {
            if (e.SettingName.Contains("Hotkey") && e.NewValue != null && e.NewValue is Windows.Storage.ApplicationDataCompositeValue compositeValue)
            {                
                if (compositeValue.Count != 0)
                {
                    int modifiers = (int)compositeValue["modifiers"];
                    int key = (int)compositeValue["key"];
                    if (!UpdateCombo(e.ContainerName, e.SettingName, modifiers, key))
                        RegisterCombo(e.ContainerName, e.SettingName, modifiers, key);
                }
                else
                {
                    UnregisterHotKey(e.ContainerName, e.SettingName);            
                }                
            }
        }

        private void HotkeyWindow_HotkeyPressed(object sender, int e)
        {
            foreach (var hd in hotkeyDataList)
            {
                if (hd.ID == e)
                {
                    HotkeyPressed?.Invoke(this, new HotkeyEventArgs(hd.ContainerName, hd.SettingName, HotkeyHelper.GetReadableStringFromHotkey(hd.Modifiers, hd.Key)));
                    return;
                }
            }
        }


        /// <summary>
        /// Register hotkey. Use dispatcher if call this in non UI thread
        /// </summary>
        /// <param name="settingName">Will Include in callback <see cref="HotkeyEventArgs"/></param>
        /// <param name="data">Will Include in callback <see cref="HotkeyEventArgs"/></param>
        /// <param name="modifiers"></param>
        /// <param name="key"></param>
        private void RegisterCombo(string settingName, string data, int modifiers, int key)
        {
            idCounter += 1;
            HotkeyData hotkeyData = new HotkeyData(settingName, data, modifiers, key, idCounter);
            hotkeyWin32.RegisterCombo(hotkeyData.ID, (HOT_KEY_MODIFIERS)hotkeyData.Modifiers, Convert.ToUInt32(hotkeyData.Key));
            hotkeyDataList.Add(hotkeyData);
            Logger.Info("Registered hotkey {0} {1} {2}", hotkeyData.ContainerName, hotkeyData.SettingName, HotkeyHelper.GetReadableStringFromHotkey(hotkeyData.Modifiers, hotkeyData.Key));
        }

        private bool UpdateCombo(string containerName, string settingName, int modifiers, int key)
        {
            foreach (var hd in hotkeyDataList)
            {
                if (hd.ContainerName == containerName && hd.SettingName == settingName)
                {
                    if (hd.Modifiers != modifiers || hd.Key != key)
                    {
                        // update with new data
                        hd.Modifiers = modifiers;
                        hd.Key = key;

                        // update combo
                        hotkeyWin32.UnRegisterCombo(hd.ID);
                        hotkeyWin32.RegisterCombo(hd.ID, (HOT_KEY_MODIFIERS)hd.Modifiers, Convert.ToUInt32(hd.Key));
                        Logger.Info("Updated hotkey {0} {1} {2}", hd.ContainerName, hd.SettingName, HotkeyHelper.GetReadableStringFromHotkey(modifiers, key));
                        return true;
                    }
                    Logger.Info("Hotkey already binded {0} {1} {2}", hd.ContainerName, hd.SettingName, HotkeyHelper.GetReadableStringFromHotkey(modifiers, key));
                    return true;
                }
            }
            return false;
        }

        public void UnregisterHotKey(string containerName, string settingName)
        {
            foreach (var hd in hotkeyDataList)
            {
                if (hd.ContainerName == containerName && hd.SettingName == settingName)
                {
                    hotkeyWin32.UnRegisterCombo(hd.ID);
                    hotkeyDataList.Remove(hd);
                    return;
                }
            }
        }

        private void RegisterAllCombos()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var containersList = localSettings.Containers.Keys.ToList();
            foreach (var containerName in containersList)
            {
                foreach (var setting in localSettings.Containers[containerName].Values)
                {
                    if (setting.Key.Contains("Hotkey") && setting.Value is Windows.Storage.ApplicationDataCompositeValue compositeValue)
                    {
                        if (compositeValue.Count != 0)
                        {
                            int modifiers = (int)compositeValue["modifiers"];
                            int key = (int)compositeValue["key"];                            
                            RegisterCombo(containerName, setting.Key, modifiers, key);
                        }
                    }                    
                }
            }
        }
    }
}
