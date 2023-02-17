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
        private int rootMagrin = 16;
        private int minHeight = 54;
        private int maxHeight = 496;
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
            _apw.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(596 * dpi), Convert.ToInt32(minHeight * dpi)));
            this.CenterToScreen();

            Root.SizeChanged += Root_SizeChanged;
            SearchBox1.TextChanged += SearchBox1_TextChanged;            
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
                _apw.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(596 * dpi), Convert.ToInt32((windowContentSize + (rootMagrin * 2)) * dpi)));
                Logger.Debug($"{Root.ActualHeight} {Root.Height}");
            }
        }

        private void SearchBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox1.Text))
            {
                Root.Height = double.NaN;
                Content.Navigate(typeof(EmptyPage));
                Content.Visibility= Visibility.Collapsed;
            }
            else if (SearchBox1.Text.Length == 1)
            {
                Content.Visibility = Visibility.Visible;
                Root.Height = double.NaN;
                Content.Navigate(typeof(TestPage));
            }
            else
            {
                Content.Visibility = Visibility.Visible;
                Root.Height = double.NaN;
                Content.Navigate(typeof(GoogleTranslatePage));
            }
        }        
    }
}
