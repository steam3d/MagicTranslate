using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MagicTranslate.Helpers;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace MagicTranslate.Hotkeys
{
    /// <summary>
    /// /https://www.travelneil.com/wndproc-in-uwp.html
    /// </summary>
    internal class Hotkeys
    {
        private const int WM_HOTKEY = 0x0312;
        private const int WM_DESTROY = 0x0002;
        private List<int> IDs = new List<int>();
        private IntPtr hWnd;
        private IntPtr setWindowLongWndProc;
        private WNDPROC wndProc;

        /// <summary>
        /// Return id of pressed hotkey
        /// </summary>
        public event EventHandler<int> HotkeyPressed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window">Window that will receive hotkeys events. If window close hotkeys event stops rising</param>
        public Hotkeys(IntPtr window)
        {
            this.hWnd = window;
            wndProc = WNDPROC;
#warning replace
            setWindowLongWndProc = Win32Helper.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProc));            
        }

        public void RegisterHotKey(int id, Windows.Win32.UI.Input.KeyboardAndMouse.HOT_KEY_MODIFIERS fsModifiers, uint vk)
        {
            if (PInvoke.RegisterHotKey((HWND)hWnd, id, fsModifiers, vk))
            {
                IDs.Add(id);
                Trace.WriteLine("Hotkey registered");
            }
        }

        public void UnRegisterCombo(int id)
        {
            if (IDs.Contains(id))
            {
                PInvoke.UnregisterHotKey((HWND)hWnd, id);
                IDs.Remove(id);
            }
        }

        public void UnregisterAllCombos()
        {
            while (IDs.Count > 0)
            {
                UnRegisterCombo(IDs[0]);
            }
        }

        private LRESULT WNDPROC(HWND param0, uint paraml, WPARAM param2, LPARAM param3)
        {
            // Any custom WndProc handling code goes here...
            Trace.WriteLine($"{paraml}");

            switch (paraml)
            {
                case WM_HOTKEY: //raise the HotkeyPressed event                    
                    Trace.WriteLine($"{param2.Value}");
                    HotkeyPressed?.Invoke(this, Convert.ToInt32(param2));
                    break;

                case WM_DESTROY: //unregister all hot keys
                    UnregisterAllCombos();
                    Trace.WriteLine("Unregisterhotkey");
                    break;
            }

            return PInvoke.CallWindowProc((WNDPROC)Marshal.GetDelegateForFunctionPointer(setWindowLongWndProc, typeof(WNDPROC)), param0, paraml, param2, param3);
        }
    }
}
