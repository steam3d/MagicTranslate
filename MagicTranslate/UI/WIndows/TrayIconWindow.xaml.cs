// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using H.NotifyIcon;
using MagicTranslate.Helper;
using MagicTranslate.UI.Commands;
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

        private void tb_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Logger.Debug("tapped");
        }
    }
}
