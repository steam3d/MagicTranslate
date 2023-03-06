// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MagicTranslate.Args;
using MagicTranslate.Helpers;
using MagicTranslate.Settings;
using MagicTranslate.Settings.UiControl;
using MagicTranslate.UI.Theme;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using TranslateLibrary;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate.UI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        WindowBackdrops backdrops;
        public SettingsPage()
        {
            this.InitializeComponent();
            Version.Text = "Version " + App.GetAppVersion();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            backdrops = e.Parameter as WindowBackdrops;
        }

        private void SettingsItem_HyperlinkClick(object sender, EventArgs e)
        {
            Logger.Debug("Click");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThemeManagement.RootTheme = ElementTheme.Dark;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ThemeManagement.RootTheme = ElementTheme.Light;
        }

        private void Background_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedItem = (TextBlock)comboBox.SelectedItem;
            backdrops?.SetBackdrop(EnumHelper.GetEnum<BackdropType>((string)selectedItem.Tag));
            ComboBoxControl.ComboBox_SelectionChanged(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedItem = (TextBlock)comboBox.SelectedItem;
            backdrops?.SetBackdrop(EnumHelper.GetEnum<BackdropType>((string)selectedItem.Tag));
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var textblock = (TextBlock)Language.SelectedItem;
            var language = (string)textblock.Tag;

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride != language)
            {
                Logger.Info($"Language was change {Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride} => {language}");
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = language;
            }
        }

        private void Language_Loaded(object sender, RoutedEventArgs e)
        {
            bool isFound = false;

            //var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
            //// Add default language
            //Language.Items.Add(new TextBlock()
            //{
            //    Text = resourceLoader.GetString("Settings_AppSettings_Language_DefaultSystem/Text"),
            //    Tag = "",
            //});

            int i = 1;
            foreach (var lang in Windows.Globalization.ApplicationLanguages.ManifestLanguages)
            {
                CultureInfo cultureInfo = null;
                try
                {
                    cultureInfo = CultureInfo.GetCultureInfo(lang);
                }
                catch (Exception exception)
                {
                    Logger.Error($"{exception.HResult} {exception.Message} ({lang})");
                }

                Language.Items.Add(new TextBlock()
                {
                    Text = cultureInfo != null ? cultureInfo.DisplayName : lang,
                    Tag = lang,
                });

                if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == lang)
                {
                    Language.SelectedIndex = i;
                    isFound = true;
                }
                i++;
            }

            //If no found selected language set the default one
            if (!isFound)
                Language.SelectedIndex = 0;
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            GlobalSettings.RemoveAllSettings();
        }

        private void GoogleTranslateLanguages_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            var selectTag = (string)GlobalSettings.LoadApplicationSetting((string)comboBox.Tag);
            var tags2ISO = TranslateLibrary.Translate.LanguagesTag;

            Dictionary<string, string> language = new Dictionary<string, string>();
            foreach (var tag2 in tags2ISO)
            {
#warning Auto detect language disabled
                if (tag2 == "auto") continue;

                var cultureInfo = CultureInfo.GetCultureInfo(tag2);
                language.Add(tag2, cultureInfo.DisplayName);
                Logger.Debug($"{cultureInfo.DisplayName} | {cultureInfo.IetfLanguageTag}");            
            }

            int i = 1;
            bool isFound = false;
            foreach (KeyValuePair<string, string> item in language.OrderBy(key => key.Value))
            {
                comboBox.Items.Add(new TextBlock()
                {
                    Text = item.Value,
                    Tag = item.Key,
                });

                if (item.Key == selectTag)
                {
                    comboBox.SelectedIndex = i;
                    isFound = true;
                }
                i++;
            }

            if (!isFound)
            {
                var item = comboBox.Items[0] as TextBlock;
                if ((string)item.Tag == selectTag)
                    comboBox.SelectedIndex = 0;
            }            
        }

        private void Logger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var SelectedItem = (TextBlock)comboBox.SelectedItem;
            NlogConfiguration.NlogConfig.ChangeLogLevel((string)SelectedItem.Tag);
            ComboBoxControl.ComboBox_SelectionChanged(sender, e);
        }

        private async void ShowLogFolder_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.TemporaryFolder.Path);
            await Launcher.LaunchFolderAsync(folder);
        }
    }
}
