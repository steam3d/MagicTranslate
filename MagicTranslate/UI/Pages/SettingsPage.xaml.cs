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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using TranslateLibrary;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
    }
}
