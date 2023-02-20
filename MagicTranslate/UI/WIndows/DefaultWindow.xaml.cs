// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MagicTranslate.Extensions;
using MagicTranslate.UI.Pages;
using Microsoft.UI;
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
using System.Runtime.Intrinsics.Arm;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate.UI.WIndows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DefaultWindow : Window
    {
        WindowBackdrops backdrops;
        public DefaultWindow()
        {
            this.InitializeComponent();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var _apw = AppWindow.GetFromWindowId(myWndId);
            _apw.Resize(new Windows.Graphics.SizeInt32(1024, 512));
            this.CenterToScreen();
            this.Topmost(true);

            backdrops = new WindowBackdrops(this);
            backdrops.SetBackdrop(BackdropType.Mica);

            Content.Navigate(typeof(SettingsPage));
            //this.Activated += DefaultWindow_Activated;
        }

        private void DefaultWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            Content.Navigate(typeof(SettingsPage));
        }
    }
}
