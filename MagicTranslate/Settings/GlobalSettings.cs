using Windows.Storage;
using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using MagicTranslate.Args;

namespace MagicTranslate.Settings
{
    /// <summary>
    /// Get or set settings to MagicPodsUI and MagicPodsService
    /// </summary>
    internal static class GlobalSettings
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static event EventHandler<SettingArgs> OnSettingChange;

        #region Capability MegrateSettings.cs  
        [Obsolete("Save is deprecated, please use SaveApplicationSetting instead.")]
        public static void Save(string key, object value)
        {
            ApplicationDataContainer Sett = ApplicationData.Current.LocalSettings;
            if (!object.Equals(LoadApplicationSetting(key), value))
            {
                Sett.Values[key] = value;
                Logger.Trace(String.Format("Save {0} = {1}", key, value));
            }
        }

        [Obsolete("Load is deprecated, please use LoadApplicationSetting instead.")]
        public static object Load(string key)
        {
            ApplicationDataContainer Sett = ApplicationData.Current.LocalSettings;
            return Sett.Values[key];
        }
        #endregion

        [Obsolete("Save is deprecated, please use PrintSettings instead.")]
        public static void PrintSettingsOld()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            foreach (var data in localSettings.Values)
            {
                Logger.Info("Settings " + data.Key + ": " + data.Value);
            }
        }

        public static void PrintSettings()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var containersList = localSettings.Containers.Keys.ToList();
            foreach (var conainerName in containersList)
            {
                Logger.Info($"Settings {conainerName}:");
                foreach (var value in localSettings.Containers[conainerName].Values)
                {
                    Logger.Info($"Settings    {value.Key}={value.Value}");
                    if (value.Value.GetType() == typeof(ApplicationDataCompositeValue))
                    {
                        foreach(var compositeValue in (ApplicationDataCompositeValue)value.Value)
                        {
                            Logger.Info($"Settings        {compositeValue.Key}={compositeValue.Value}");
                        }
                    }
                }
            }
        }

        public static object LoadApplicationSetting(string key)
        {
            return LoadSettingFromContainerStorage("ApplicationSettings",key);
        }

        public static void SaveApplicationSetting(string key, object value)
        {
            SaveSettingToContainerStorage("ApplicationSettings", key, value);
        }

        public static object LoadHeadphoneSetting(string btDeviceId, string key)
        {
            return LoadSettingFromContainerStorage(btDeviceId, key);
        }

        public static void SaveHeadphoneSetting(string btDeviceId, string key, object value)
        {
            SaveSettingToContainerStorage(btDeviceId, key, value);
        }

        public static List<ValueSet> GetHotkeysValueSetList()
        {
            List<ValueSet> hotkeyValueSetList = new List<ValueSet>();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var containersList = localSettings.Containers.Keys.ToList();

            foreach (var conainerName in containersList)
            {
                foreach (var value in localSettings.Containers[conainerName].Values)
                {
                    if (value.Key.Contains("hotkey") && value.Value.GetType() == typeof(ApplicationDataCompositeValue))
                    {
                        var valueSet = new ValueSet();
                        valueSet.Add(value.Key, (ApplicationDataCompositeValue)value.Value);
                        hotkeyValueSetList.Add(valueSet);
                    }
                }
            }
            return hotkeyValueSetList;
        }



        private static object LoadSettingFromContainerStorage(string containerName, string key)
        {
            string tag = "Loaded";

            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(key))
            {
                Logger.Error($"{tag} Can't load settings. One or more params are null.");
                return null;
            }
            
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Containers.ContainsKey(containerName))
            {
                if (localSettings.Containers[containerName].Values.ContainsKey(key))
                {
                    object value = localSettings.Containers[containerName].Values[key];
                    Logger.Trace($"{tag} '{containerName}' -> '{key}' -> '{value}' ");
                    return value;
                }
            }

            // Container empty or does not exist, try to get value from DefaultSettings
            if (DefaultSettings.Settings.ContainsKey(key))
            {
                object value = DefaultSettings.Settings[key];
                Logger.Trace($"{tag} '{containerName}' -> 'DefaultSettings' -> '{key}' -> '{value}'");
                return DefaultSettings.Settings[key];
            }


            Logger.Error($"{tag} Key '{key}' does not found");
            return null;
        }


        private static void SaveSettingToContainerStorage(string containerName, string key, object value)
        {
            string tag = "Saved";

            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(key))
            {
                Logger.Error($"{tag} Can't save settings. One or more params are null.");
                return;
            }

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer container = localSettings.CreateContainer(containerName, ApplicationDataCreateDisposition.Always);
            if (localSettings.Containers.ContainsKey(containerName))
            {
                object savedSettings = LoadSettingFromContainerStorage(containerName, key);
                if (savedSettings != null)
                {                
                    if (!object.Equals(savedSettings, value))
                    {
                        localSettings.Containers[containerName].Values[key] = value;
                        var args = new SettingArgs(containerName, key, value, savedSettings);
                        OnSettingChange?.Invoke(null, args);
                        Logger.Trace($"{tag} '{containerName}' -> '{key}' -> '{value}'");
                    }
                    else
                    {
                        Logger.Trace($"Skip saving '{containerName}' X '{key}' X '{value}'");
                    }
                }
                else
                {
                    Logger.Error($"{tag} Key '{key}' does not found");
                }
            }
            else
            {
                Logger.Error($"{tag} Container '{containerName}' does not exist");
            }
        }

        public static void RemoveAllSettings()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var containersList = localSettings.Containers.Keys.ToList();
            foreach (var conainerName in containersList)
            {
                localSettings.DeleteContainer(conainerName);
                Logger.Info($"Removed {conainerName} container");
            }
        }
    }
}
