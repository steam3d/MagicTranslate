using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using MagicTranslate.Helpers;

namespace MagicTranslate.Extensions
{
    internal static class WindowExtensions
    {
        /// <summary>
        /// Remove title bar of specific window
        /// </summary>
        /// <param name="window"></param>
        public static void HideTitleBar(this Window window)
        {
            var hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWndMain);
            var _apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
            var _presenter = _apw.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            _presenter.IsResizable = false;
            _presenter.SetBorderAndTitleBar(false, false);
        }

        /// <summary>
        /// Place window to the center of current screen
        /// </summary>
        /// <param name="window"></param>
        public static void CenterToScreen(this Window window)
        {
            var hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWndMain);
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            if (appWindow is not null)
            {
                Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                if (displayArea is not null)
                {
                    var CenteredPosition = appWindow.Position;
                    CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                    CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                    appWindow.Move(CenteredPosition);
                }
            }
        }

        /// <summary>
        /// Apply <see cref="HideTitleBar(Window)"/> before
        /// Update for Windows 11 from michalleptuch comment : https://github.com/microsoft/microsoft-ui-xaml/issues/1247#issuecomment-1374474960
        /// otherwise there are borders + shadow from his test
        /// Returns logically 0x80070057 (E_INVALIDARG) on Windows 10
        /// </summary>
        /// <param name="window"></param>
        public static void RemoveWindowStyle(this Window window)
        {
            var hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
            int nValue = (int)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;

            unsafe
            {
                PInvoke.DwmSetWindowAttribute(new HWND(hWndMain),
                    Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, &nValue, (uint)Marshal.SizeOf(typeof(int)));
            }
        }

        /// <summary>
        /// Hide window icon from task and alt+tab
        /// https://stackoverflow.com/questions/74261765/remove-the-window-from-the-taskbar-in-winui-3
        /// </summary>
        /// <param name="window"></param>
        public static void HideTaskBarAndAltTabIcon(this Window window)
        {
            var hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
            int exStyle = (int)Win32Helper.GetWindowLong(hWndMain, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            Win32Helper.SetWindowLong(hWndMain, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private static IntPtr BoolToHWndInsertAfter(bool isTopmost)
        {
            return isTopmost ? new IntPtr((int)HWndInsertAfter.HWND_TOPMOST) : new IntPtr((int)HWndInsertAfter.HWND_NOTOPMOST);
        }

        /// <summary>
        /// Make windows on top of others windows, except Task Manager
        /// </summary>
        /// <param name="window"></param>
        /// <param name="isTopmost">True - on, False - off</param>
        public static void Topmost(this Window window, bool isTopmost = true)
        {            
            var hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
            //RECT rectWnd;
            //PInvoke.GetWindowRect(new HWND(hWndMain), out rectWnd);
            PInvoke.SetWindowPos(new HWND(hWndMain), new HWND(BoolToHWndInsertAfter(isTopmost)), 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
        }

        /// <summary>
        /// Resize window to all screen.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="isTopmost">True enable topmost, false disable topmost</param>
        public static void ResizeOnAllScreens(this Window window, bool isTopmost = false)
        {
            var hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var virtualDisplayRect = Win32Helper.GetVirtualDisplayRect();            
            PInvoke.SetWindowPos(new HWND(hWndMain), new HWND(BoolToHWndInsertAfter(isTopmost)), virtualDisplayRect.left, virtualDisplayRect.top, virtualDisplayRect.right, virtualDisplayRect.bottom, SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED);
        }

        private const double DefaultPixelsPerInch = 96D; // Default pixels per Inch
        public static double GetDpi(this Window window)
        {
            var hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var dpi = PInvoke.GetDpiForWindow(new HWND(hWndMain));

            if (dpi == 0)
                return 1D;

            return dpi / DefaultPixelsPerInch;
        }
    }
}
