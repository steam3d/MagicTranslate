using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace MagicTranslate.Helpers
{

    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }


    [Flags]
    public enum ExtendedWindowStyles
    {
        // ...
        WS_EX_TOOLWINDOW = 0x00000080,
        // ...
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    /// </summary>
    public enum HWndInsertAfter
    {
        /// <summary>
        /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        HWND_BOTTOM = 1,
        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window. 
        /// </summary>
        HWND_NOTOPMOST = -2,
        /// <summary>
        /// Places the window at the top of the Z order. 
        /// </summary>
        HWND_TOP = 0,
        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated. 
        /// </summary>
        HWND_TOPMOST = -1,
    }

    internal class Win32Helper
    {
        public static long GetWindowLong(IntPtr hWnd, WINDOW_LONG_PTR_INDEX nIndex)
        {
#if x86
            return PInvoke.GetWindowLong((Windows.Win32.Foundation.HWND)hWnd, nIndex);
#endif

#if x64 || ARM64
            return PInvoke.GetWindowLongPtr((Windows.Win32.Foundation.HWND)hWnd, nIndex);
#endif
        }

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, WINDOW_LONG_PTR_INDEX nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            PInvoke.SetLastError(0);

#if x86            
            int tempResult = PInvoke.SetWindowLong((Windows.Win32.Foundation.HWND)hWnd, nIndex, IntPtrToInt32(dwNewLong));
            error = Marshal.GetLastWin32Error();
            result = new IntPtr(tempResult);
#endif

#if x64 || ARM64
            
            result = PInvoke.SetWindowLongPtr((Windows.Win32.Foundation.HWND)hWnd, nIndex, dwNewLong);
            error = Marshal.GetLastWin32Error();           
#endif
            if (result == IntPtr.Zero && error != 0)
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        /// <summary>
        /// Get VirtualScreen(all computer screens) rect
        /// </summary>
        /// <returns></returns>
        internal static Windows.Win32.Foundation.RECT GetVirtualDisplayRect()
        {
            return new Windows.Win32.Foundation.RECT(left: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_XVIRTUALSCREEN),
                            top: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_YVIRTUALSCREEN),
                            right: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXVIRTUALSCREEN),
                            bottom: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYVIRTUALSCREEN));

        }
    }
}
