// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MagicTranslate.Helper;
using MagicTranslate.Hotkeys;
using MagicTranslate.Settings;
using MagicTranslate.UI.Theme;
using MagicTranslate.UI.WIndows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static HotkeysGlobal Hotkeys = null;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            NlogConfiguration.NlogConfig.Configure(
                System.IO.Path.Combine(ApplicationData.Current.TemporaryFolder.Path, "MagicTranslateLog.txt"),
                NlogConfiguration.NlogConfig.LogLevelFromString((string)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "LogLevel")),
                NLog.LogLevel.Fatal);
            Program.OnActivated += Program_OnActivated;
            GlobalSettings.OnSettingChange += GlobalSettings_OnSettingChange;
        }

        
        private void GlobalSettings_OnSettingChange(object sender, Args.SettingArgs e)
        {
            Logger.Debug($"{e.ContainerName} -> {e.SettingName} ->  {e.OldValue} changed to {e.NewValue} ");
        }

        private void Program_OnActivated(object sender, Microsoft.Windows.AppLifecycle.AppActivationArguments e)
        {
            CreatSearchWindowOrActive();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //m_window = new PlayGroundWindow();
            //StartupWindow = WindowHelper.CreateWindow(typeof(SearchWindow));
            //StartupWindow = WindowHelper.CreateWindow(typeof(DefaultWindow));
            StartupWindow = WindowHelper.CreateWindow(typeof(TrayIconWindow));
            ThemeManagement.Initialize();

            if (Hotkeys == null)
            {
                Hotkeys = new HotkeysGlobal(StartupWindow);
                Hotkeys.HotkeyPressed += Hotkeys_HotkeyPressed;
            }

            StartupWindow?.Activate();

            CreatSearchWindowOrActive();
        }

        private void Hotkeys_HotkeyPressed(object sender, HotkeyEventArgs e)
        {
            Logger.Debug(e.ReadableHotkey);
            CreatSearchWindowOrActive();
        }

        private void CreatSearchWindowOrActive()
        {
            if (StartupWindow == null)
            {
                Logger.Error("Main window is null");
                return;
            }

            StartupWindow.DispatcherQueue.TryEnqueue(() =>
            {
                if (SearchWindow == null)
                    SearchWindow = WindowHelper.CreateWindow(typeof(SearchWindow));
                
                if(SearchWindow != null)
                {
                    SearchWindow?.Activate();
                    SearchWindow.Closed += (e, ee) => SearchWindow = null;
                }
            });
        }

        public static Window StartupWindow;
        public static Window SearchWindow;
    }
}
