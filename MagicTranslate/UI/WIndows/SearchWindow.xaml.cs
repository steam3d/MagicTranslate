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
        private int minHeight = 32;
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

            SearchBox1.TextChanged += SearchBox1_TextChanged;
            Content.SizeChanged += NewContent_SizeChanged;
            SearchBox.SizeChanged += NewSearchBox_SizeChanged;

            
        }

        private void NewContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Logger.Debug($"Content {Content.ActualHeight} ContentRowHeight {ContentRowHeight.ActualHeight}");
            ResizeWindow();
        }

        private void NewSearchBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Logger.Debug($"SearchBox {SearchBox.ActualHeight} SearchBoxRowHeight {SearchBoxRowHeight.ActualHeight}");
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            var windowContentSize = SearchBoxRowHeight.ActualHeight + ContentRowHeight.ActualHeight + (rootMagrin * 2);            
            windowContentSize = windowContentSize > maxHeight ? maxHeight : windowContentSize;
            var newContentSize = windowContentSize - (rootMagrin * 2) - SearchBoxRowHeight.ActualHeight - 24;
            
            if (Content.Height != newContentSize)
                Content.Height = newContentSize;

            var dpi = this.GetDpi();
            if ((_apw.Size.Height / dpi) != windowContentSize)
                _apw.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(596 * dpi), Convert.ToInt32(windowContentSize * dpi)));

            Logger.Debug($"WindowSize {windowContentSize} ContentSize {newContentSize}");
        }

        private void SearchBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox1.Text))
            {
                Content.Height = double.NaN;
                Content.Navigate(typeof(EmptyPage));
            }
            else if (SearchBox1.Text.Length == 1)
            {
                Content.Height = double.NaN;
                Content.Navigate(typeof(TestPage));
            }
            else
            {
                Content.Height = double.NaN;
                Content.Navigate(typeof(GoogleTranslatePage));
            }
        }        
    }
}
