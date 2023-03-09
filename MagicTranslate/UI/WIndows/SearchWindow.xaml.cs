// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MagicTranslate.UI.Pages;
using Microsoft.UI.Windowing;
using Microsoft.UI;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using MagicTranslate.Extensions;
using System.Runtime.Intrinsics.Arm;
using System.Timers;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Media.Animation;
using MagicTranslate.Input;
using MagicTranslate.Args;
using MagicTranslate.Settings;
using System.Globalization;
using Windows.Win32;
using MagicTranslate.Helper;
using System.Threading.Tasks;
using MagicTranslate.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate.UI.WIndows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchWindow : Window
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        AppWindow _apw;
        private Thickness rootMagrin = new Thickness(16);

        //Indepent pixels
        private int minHeight = 54;
        private int maxHeight = 496;        
        private int MaxWidth = 596;
        private int screenBorder = 16;

        Timer textChangedDebouncingTimer = new Timer(512);
        private InputLanguage inputLanguage = new InputLanguage();
        private WindowBackdrops backdrops;

        char[] charsToTrim = { ' ', '\n', '\t','\r', '\0' };

        private bool SkipTutorial = true;
        private bool SkipTutorialStarted = false;

        public SearchWindow()
        {
            this.InitializeComponent();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            
            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            this.Title = (string)Application.Current.Resources["AppName"];
            _apw.SetIcon((string)Application.Current.Resources["AppIcoIconPath"]);
            var _presenter = _apw.Presenter as OverlappedPresenter;
            _presenter.IsResizable = false;
            _presenter.IsAlwaysOnTop = true;            
            _presenter.SetBorderAndTitleBar(false, false);
            
            var dpi = this.GetDpi();
            var displayArea = DisplayArea.GetFromWindowId(myWndId, DisplayAreaFallback.Primary);
            
            var maxScreenWidth = (displayArea.WorkArea.Width / dpi) - screenBorder - rootMagrin.Left - rootMagrin.Right;
            MaxWidth = MaxWidth > maxScreenWidth ? Convert.ToInt32(maxScreenWidth) : MaxWidth;

            _apw.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(MaxWidth * dpi), Convert.ToInt32(minHeight * dpi)));
            this.CenterToScreen();
            this.HideTaskBarAndAltTabIcon();

            var windowRect = this.GetRect();
            var maxScreenHeight = (displayArea.WorkArea.Height / dpi) - (windowRect.top / dpi) - rootMagrin.Bottom - rootMagrin.Top - screenBorder;
            maxHeight = maxHeight > maxScreenHeight ? Convert.ToInt32(maxScreenHeight) : maxHeight;
            this.ApplyTheme();
            Root.SizeChanged += Root_SizeChanged;
            SearchBox.TextChanged += SearchBox_TextChanged;
            textChangedDebouncingTimer.Elapsed += TextChangedDebouncingTimer_Elapsed;

            inputLanguage.CurrentInputChanged += InputLanguage_CurrentInputChanged;
            InputLanguage_CurrentInputChanged(this, inputLanguage.CurrentInput);
            SkipTutorial = (bool)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "SkipTutorial");
            this.Closed += SearchWindow_Closed;
            this.Activated += SearchWindow_Activated;
            
            backdrops = new WindowBackdrops(this);
            //Root.PreviewKeyDown += Root_PreviewKeyDown;            
            //Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread
        }

        #region Teaching
        private async void TeachingTipTour()
        {
            ShortcutOpenSettings.IsEnabled = false; //App will crash when use shortcut during tutorial
            SkipTutorialStarted = true; //To prevent multiple tutorial when user try to type something in SearchBox
            string hotkeyReadableString = string.Empty;
            var compositeValue = (Windows.Storage.ApplicationDataCompositeValue)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "HotkeyOpenSearchBar");
            if (compositeValue != null && compositeValue.Count > 0)            
                hotkeyReadableString = HotkeyHelper.GetReadableStringFromHotkey((int)compositeValue["modifiers"], (int)compositeValue["key"]);
            
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
            var teachingTip = new TeachingTip();
            teachingTip.Tag = 0;
            teachingTip.PreferredPlacement = TeachingTipPlacementMode.Center;
            teachingTip.Title = resourceLoader.GetString("Search_TeachingTip_Welcome/Title");
            teachingTip.Subtitle = string.Format(resourceLoader.GetString("Search_TeachingTip_Welcome/Subtitle"), hotkeyReadableString);
            teachingTip.CloseButtonContent = resourceLoader.GetString("Search_TeachingTip_Buttons_Next");
            teachingTip.CloseButtonClick += TeachingTip_CloseButtonClick;
            Root.Children.Add(teachingTip);

            //https://github.com/microsoft/microsoft-ui-xaml/issues/7937
            await Task.Delay(100);
            teachingTip.IsOpen = true;
        }

        private async void TeachingTipTour1()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
            var teachingTip = new TeachingTip();
            teachingTip.Tag = 1;
            teachingTip.Target = SearchBox;
            teachingTip.PreferredPlacement = TeachingTipPlacementMode.BottomLeft;
            teachingTip.Subtitle = resourceLoader.GetString("Search_TeachingTip_SwitchLanguage/Subtitle");
            teachingTip.CloseButtonContent = resourceLoader.GetString("Search_TeachingTip_Buttons_Next");
            teachingTip.CloseButtonClick += TeachingTip_CloseButtonClick;            
            Root.Children.Add(teachingTip);
            
            //https://github.com/microsoft/microsoft-ui-xaml/issues/7937
            await Task.Delay(100);
            teachingTip.IsOpen = true;            
        }

        private async void TeachingTipTour2()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
            var teachingTip = new TeachingTip();
            teachingTip.Tag = 2;
            teachingTip.Target = Content;
            teachingTip.PreferredPlacement = TeachingTipPlacementMode.Center;
            teachingTip.Subtitle = resourceLoader.GetString("Search_TeachingTip_TranslationResult/Subtitle");
            teachingTip.CloseButtonContent = resourceLoader.GetString("Search_TeachingTip_Buttons_Next");
            teachingTip.CloseButtonClick += TeachingTip_CloseButtonClick;
            Root.Children.Add(teachingTip);

            await Task.Delay(100);
            teachingTip.IsOpen = true;
        }

        private async void TeachingTipTour3()
        {
            string hotkeyReadableString = HotkeyHelper.GetReadableStringFromHotkey(ShortcutOpenSettings.Modifiers, ShortcutOpenSettings.Key);
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
            string settingsString = resourceLoader.GetString("Taskbar_MenuFlyout_Settings/Text");

            var teachingTip = new TeachingTip();
            teachingTip.Tag = 3;
            teachingTip.PreferredPlacement = TeachingTipPlacementMode.Bottom;
            teachingTip.Subtitle = string.Format(resourceLoader.GetString("Search_TeachingTip_OpenSettings/Subtitle"), settingsString, hotkeyReadableString);
            teachingTip.CloseButtonContent = resourceLoader.GetString("Search_TeachingTip_Buttons_Finish");
            teachingTip.ActionButtonContent = resourceLoader.GetString("Search_TeachingTip_Buttons_Settings");
            teachingTip.CloseButtonClick += TeachingTip_CloseButtonClick;
            teachingTip.ActionButtonClick += TeachingTip_ActionButtonClick;
            Root.Children.Add(teachingTip);

            await Task.Delay(100);
            teachingTip.IsOpen = true;
        }

        private void TeachingTip_ActionButtonClick(TeachingTip sender, object args)
        {
            SkipTutorial = true;
            ShortcutOpenSettings.IsEnabled = true;
            GlobalSettings.SaveHeadphoneSetting("ApplicationSettings", "SkipTutorial", true);
            App.OpenSettingWindow();
        }

        private void TeachingTip_CloseButtonClick(TeachingTip sender, object args)
        {            
            Root.Children.Remove(sender);                   
            switch ((int)sender.Tag)
            {
                case 0:
                    TeachingTipTour1();
                    break;
                case 1:                    
                    TeachingTipTour2();
                    break;
                case 2:
                    TeachingTipTour3();
                    break;
                case 3:
                    SkipTutorial = true;
                    GlobalSettings.SaveHeadphoneSetting("ApplicationSettings", "SkipTutorial", true);
                    ShortcutOpenSettings.IsEnabled = true;
                    TextChangedDebouncingTimer_Elapsed(null, null);
                    break;
            }
        }
        #endregion

        private void SearchWindow_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                this.Close();

            if (args.WindowActivationState == WindowActivationState.PointerActivated || args.WindowActivationState == WindowActivationState.CodeActivated)
            {
                this.Focus();            
                SearchBox.Focus(FocusState.Keyboard);
                if (SkipTutorial == false)
                    TextChangedDebouncingTimer_Elapsed(null, null);
            }

        }

        private void InputLanguage_CurrentInputChanged(object sender, System.Globalization.CultureInfo e)
        {
            var translateFromTag = (string)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "GoogleTranslateFrom");
            var translateToTag = (string)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "GoogleTranslateTo");
            
            if (!string.IsNullOrEmpty(translateToTag) && !string.IsNullOrEmpty(translateFromTag))
            {
                CultureInfo translateFromCalture = e;

                var translateToCalture = translateToTag == "SystemLanguage" ? CultureInfo.InstalledUICulture : new CultureInfo(translateToTag);

                if (translateToCalture.Equals(e))
                {
                    if (translateFromTag != "LanguageNotSelected")
                    {
                        translateFromCalture = translateToCalture;
                        translateToCalture = new CultureInfo(translateFromTag);
                    }
                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
                    var placeholder = resourceLoader.GetString("Search_SearchBox_Placeholder");
                    SearchBox.PlaceholderText = string.Format(placeholder,translateFromCalture.EnglishName,translateToCalture.EnglishName);
                    
                    //Rise timer if user type something and changed language
                    if (!string.IsNullOrEmpty(SearchBox.Text))
                    {
                        SearchBox_TextChanged(null, null);
                    }
                });
            }
        }

        private void SearchWindow_Closed(object sender, WindowEventArgs args)
        {
            inputLanguage.Dispose();
        }

        /// <summary>
        /// When SearchBox text contains 2 symbols new string the size after remove the will be incorrect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height != e.NewSize.Height)
            {
                var windowContentSize = e.NewSize.Height > maxHeight ? maxHeight : e.NewSize.Height;
                var dpi = this.GetDpi();
                Root.Height = windowContentSize;
                ContentRowDefinition.Height = new GridLength(1,GridUnitType.Star);
                _apw.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(MaxWidth * dpi), Convert.ToInt32((windowContentSize + rootMagrin.Top + rootMagrin.Bottom) * dpi)));
                Logger.Debug($"{Root.ActualHeight} {Root.Height}");
            }
        }

        private void TextChangedDebouncingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                //SearchBox.Text = SearchBox.Text.Trim(charsToTrim);
                //SearchBox.SelectionStart = SearchBox.Text.Length;
                //SearchBox.SelectionLength = 0;
                if (SkipTutorial == false)
                {
                    if (SkipTutorialStarted)
                        return;

                    Content.Visibility = Visibility.Visible;
                    Root.Height = double.NaN;
                    ContentRowDefinition.Height = new GridLength(1, GridUnitType.Auto);
                    Content.Navigate(typeof(GoogleTranslatePage), null, new DrillInNavigationTransitionInfo());
                    TeachingTipTour();
                }
                else if (string.IsNullOrEmpty(SearchBox.Text))
                {
                    Root.Height = double.NaN;
                    ContentRowDefinition.Height = new GridLength(1, GridUnitType.Auto);                    
                    Content.Navigate(typeof(EmptyPage));
                    Content.Visibility = Visibility.Collapsed;
                }
                else
                {
                    var translateFromTag = (string)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "GoogleTranslateFrom");
                    var translateToTag = (string)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "GoogleTranslateTo");

                    if (!string.IsNullOrEmpty(translateToTag) && !string.IsNullOrEmpty(translateFromTag))
                    {
                        CultureInfo translateFromCalture = inputLanguage.CurrentInput;
                        
                        var translateToCalture = translateToTag == "SystemLanguage" ? CultureInfo.InstalledUICulture : new CultureInfo(translateToTag);

                        if (translateToCalture.Equals(inputLanguage.CurrentInput))
                        {
                            if (translateFromTag != "LanguageNotSelected")
                            {
                                translateFromCalture = translateToCalture;
                                translateToCalture = new CultureInfo(translateFromTag);
                            }
                        }

                        var args = new TranslateArgs(translateFromCalture, SearchBox.Text, translateToCalture);
                        Content.Visibility = Visibility.Visible;
                        Root.Height = double.NaN;
                        ContentRowDefinition.Height = new GridLength(1, GridUnitType.Auto);
                        Content.Navigate(typeof(GoogleTranslatePage), args, new DrillInNavigationTransitionInfo());
                    }
                }
            });           
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!textChangedDebouncingTimer.Enabled)
            {
                textChangedDebouncingTimer.AutoReset = false;
                textChangedDebouncingTimer.Start();
            }
            else
            {
                textChangedDebouncingTimer.Stop();
                textChangedDebouncingTimer.Start();
            }
        }

        private void ShortcutOpenSettings_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            App.OpenSettingWindow();
        }

        private void ShortcutClose_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            this.Close();
        }
    }
}
