// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using H.NotifyIcon;
using MagicTranslate.Helper;
using MagicTranslate.Helpers;
using MagicTranslate.Hotkeys;
using MagicTranslate.Settings;
using MagicTranslate.UI.Commands;
using Microsoft.UI.Windowing;
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
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate.UI.WIndows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrayIconWindow : Window
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ICommand DoubleClickCommand { get; set; }
        public ICommand OpenSettings { get; set; }
        public ICommand CloseProgram { get; set; }

        public TrayIconWindow()
        {
            this.InitializeComponent();

            // Make blink almost invisible 
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var apw = AppWindow.GetFromWindowId(myWndId);
            var presenter = apw.Presenter as OverlappedPresenter;
            presenter.SetBorderAndTitleBar(false, false);
            apw.Resize(new Windows.Graphics.SizeInt32(1, 1));

            var _DoubleClickCommand = new EventCommand();
            _DoubleClickCommand.CommandExecuted += _DoubleClickCommand_CommandExecuted;
            DoubleClickCommand = _DoubleClickCommand;

            var _openSettings = new EventCommand();
            _openSettings.CommandExecuted += _openSettings_CommandExecuted;
            OpenSettings = _openSettings;

            var _closeProgram = new EventCommand();
            _closeProgram.CommandExecuted += _closeProgram_CommandExecuted;
            CloseProgram = _closeProgram;

            this.Activated += TrayIcon_Activated;
            this.Closed += TrayIconWindow_Closed;
            GlobalSettings.OnSettingChange += GlobalSettings_OnSettingChange;
            UpdateToolTipText();
        }

        private void TrayIconWindow_Closed(object sender, WindowEventArgs args)
        {
            GlobalSettings.OnSettingChange -= GlobalSettings_OnSettingChange;
        }

        private void _openSettings_CommandExecuted(object sender, EventArgs e)
        {
            App.OpenSettingWindow();
        }

        private void _closeProgram_CommandExecuted(object sender, EventArgs e)
        {
            var windows =  new List<Window>(WindowHelper.ActiveWindows);
            foreach (var w in windows)
            {
                w.Close();
            }
            Application.Current.Exit();
        }

        private void _DoubleClickCommand_CommandExecuted(object sender, EventArgs e)
        {
            App.OpenSearchWindow();
        }

        private void TrayIcon_Activated(object sender, WindowActivatedEventArgs args)
        {
            this.Hide(true);
        }

        private void UpdateToolTipText()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();

            string toolTipText;
            var compositeValue = (Windows.Storage.ApplicationDataCompositeValue)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "HotkeyOpenSearchBar");
            if (compositeValue != null && compositeValue.Count > 0)
            {
                var hotkeyReadableString = HotkeyHelper.GetReadableStringFromHotkey((int)compositeValue["modifiers"], (int)compositeValue["key"]);
                toolTipText = string.Format(resourceLoader.GetString("Taskbar_TaskbarIcon_ToolTipText_hotkey"), hotkeyReadableString);

            }
            else
            {
                toolTipText = resourceLoader.GetString("Taskbar_TaskbarIcon_ToolTipText");
            }

            tb.ToolTipText = toolTipText;
        }

        private void GlobalSettings_OnSettingChange(object sender, Args.SettingArgs e)
        {
            if (e.ContainerName == "ApplicationSettings" && e.SettingName == "HotkeyOpenSearchBar")
                this.DispatcherQueue.TryEnqueue(() => { UpdateToolTipText(); });
        }
    }
}
