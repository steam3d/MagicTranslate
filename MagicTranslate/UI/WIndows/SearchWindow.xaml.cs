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
        public SearchWindow()
        {
            this.InitializeComponent();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            
            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
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

            var windowRect = this.GetRect();
            var maxScreenHeight = (displayArea.WorkArea.Height / dpi) - (windowRect.top / dpi) - rootMagrin.Bottom - rootMagrin.Top - screenBorder;
            maxHeight = maxHeight > maxScreenHeight ? Convert.ToInt32(maxScreenHeight) : maxHeight;
            this.ApplyTheme();
            Root.SizeChanged += Root_SizeChanged;
            SearchBox.TextChanged += SearchBox_TextChanged;
            textChangedDebouncingTimer.Elapsed += TextChangedDebouncingTimer_Elapsed;

            inputLanguage.CurrentInputChanged += InputLanguage_CurrentInputChanged;
            InputLanguage_CurrentInputChanged(this, inputLanguage.CurrentInput);
            this.Closed += SearchWindow_Closed;
            this.Activated += SearchWindow_Activated;
            
            backdrops = new WindowBackdrops(this);

            //Root.PreviewKeyDown += Root_PreviewKeyDown;            
            //Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread
        }

        private void SearchWindow_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                this.Close();

            if (args.WindowActivationState == WindowActivationState.PointerActivated || args.WindowActivationState == WindowActivationState.CodeActivated)
            {
                this.Focus();            
                SearchBox.Focus(FocusState.Keyboard);
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
                if (string.IsNullOrEmpty(SearchBox.Text))
                {
                    Root.Height = double.NaN;
                    ContentRowDefinition.Height = new GridLength(1, GridUnitType.Auto);                    
                    Content.Navigate(typeof(EmptyPage));
                    Content.Visibility = Visibility.Collapsed;
                }
                //else if (SearchBox.Text.Length == 1)
                //{
                //    Content.Visibility = Visibility.Visible;
                //    Root.Height = double.NaN;
                //    ContentRowDefinition.Height = new GridLength(1, GridUnitType.Auto);
                //    Content.Navigate(typeof(TestPage), null, new DrillInNavigationTransitionInfo());
                //}
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
