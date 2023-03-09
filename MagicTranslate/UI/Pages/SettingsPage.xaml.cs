// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MagicTranslate.Args;
using MagicTranslate.Extensions;
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
using Windows.ApplicationModel;
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

        private async void Button_NavigateToURL(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null && button.Tag != null)
            {
                Uri uri = null;

                switch ((string)button.Tag)
                {
                    case "MagicPods":
                        uri = new Uri(@"https://www.microsoft.com/store/apps/9P6SKKFKSHKM");
                        break;
                    case "MagicSelect":
                        uri = new Uri(@"https://www.microsoft.com/store/apps/9N78GWBTW7L5");
                        break;
                    case "Weblate":
                        uri = new Uri(@"https://weblate.magicpods.app/engage/magicselect-windows/");
                        break;
                    case "YAD":
                        uri = new Uri(@"https://magicpods.app/yetanotherdino/");
                        break;
                }

                if (uri != null)
                    await Windows.System.Launcher.LaunchUriAsync(uri);                
            }
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

        private async void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var textblock = (TextBlock)Language.SelectedItem;
            var language = textblock.Tag == null ? "" : (string)textblock.Tag;

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride != language)
            {
                Logger.Info($"Language was change {Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride} => {language}");
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = language;

                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
                ContentDialog dialog = new ContentDialog();
                dialog.Title = resourceLoader.GetString("Settings_App_Setting_Common_ContentDialog_RestartRequired/Title");
                dialog.Content = resourceLoader.GetString("Settings_App_Setting_Common_ContentDialog_RestartRequired/Content");
                dialog.PrimaryButtonText = resourceLoader.GetString("Settings_App_Setting_Common_ContentDialog_RestartRequired/PrimaryButtonText");
                dialog.CloseButtonText = resourceLoader.GetString("Settings_App_Setting_Common_ContentDialog_RestartRequired/CloseButtonText");
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.XamlRoot = this.Content.XamlRoot;
                if (ContentDialogResult.Primary == await dialog.ShowAsync())
                    Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
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
                    Text = cultureInfo != null ? cultureInfo.NativeName.ToFirstCharUpper() : lang,
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
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "";
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
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
                language.Add(tag2, cultureInfo.DisplayName.ToFirstCharUpper());
                //Logger.Debug($"{cultureInfo.DisplayName} | {cultureInfo.IetfLanguageTag}");            
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

        private async void Startup_Loaded(object sender, RoutedEventArgs e)
        {
            StartupTask startupTask = await StartupTask.GetAsync("MagicTranslateHotkeyId");
            Logger.Info("Startup state {0}", startupTask.State);
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    Startup.IsOn = false;
                    break;
                case StartupTaskState.DisabledByUser:
                    Startup.IsOn = false;
                    break;
                case StartupTaskState.DisabledByPolicy:
                    Startup.IsOn = false;
                    break;
                case StartupTaskState.Enabled:
                    Startup.IsOn = true;
                    break;
            }

            Startup.Toggled += Startup_Toggled;
        }

        private async void Startup_Toggled(object sender, RoutedEventArgs e)
        {
            Startup.Toggled -= Startup_Toggled;
            StartupTask startupTask = await StartupTask.GetAsync("MagicTranslateHotkeyId");

            if (Startup.IsOn)
            {
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                        Logger.Info("Try to enable startup");
                        await startupTask.RequestEnableAsync();
                        break;

                    case StartupTaskState.DisabledByUser:
                        Logger.Info("Startup disabled by user show dialog");
                        var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
                        ContentDialog dialog = new ContentDialog();
                        dialog.Title = resourceLoader.GetString("Settings_App_Setting_Startup_ContentDialog_DisabledByUser/Title");
                        dialog.Content = string.Format(resourceLoader.GetString("Settings_App_Setting_Startup_ContentDialog_DisabledByUser/Content"), Application.Current.Resources["AppName"]);
                        dialog.PrimaryButtonText = resourceLoader.GetString("Settings_App_Setting_Startup_ContentDialog_DisabledByUser/PrimaryButtonText");
                        dialog.CloseButtonText = resourceLoader.GetString("Settings_App_Setting_Startup_ContentDialog_DisabledByUser/CloseButtonText");
                        dialog.DefaultButton = ContentDialogButton.Primary;
                        dialog.XamlRoot = this.Content.XamlRoot;
                        if (ContentDialogResult.Primary == await dialog.ShowAsync())
                            await Launcher.LaunchUriAsync(new Uri("ms-settings:startupapps"));
                        break;
                    case StartupTaskState.DisabledByPolicy:
                        Logger.Info("Startup disabled by pilicy show dialog");
                        var resourceLoader1 = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
                        ContentDialog dialog1 = new ContentDialog();
                        dialog1.Title = resourceLoader1.GetString("Settings_App_Setting_Startup_ContentDialog_DisabledByPolicy/Title");
                        dialog1.Content = resourceLoader1.GetString("Settings_App_Setting_Startup_ContentDialog_DisabledByPolicy/Content");
                        dialog1.CloseButtonText = resourceLoader1.GetString("Settings_App_Setting_Startup_ContentDialog_DisabledByPolicy/CloseButtonText");
                        dialog1.XamlRoot = this.Content.XamlRoot;
                        await dialog1.ShowAsync();
                        break;
                }
            }
            else
            {
                Logger.Info("Disable startup");
                startupTask.Disable();
            }
            Startup_Loaded(sender, e);            
        }
    }
}
