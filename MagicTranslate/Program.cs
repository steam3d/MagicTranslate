﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace MagicTranslate
{
    /// <summary>
    /// https://github.com/jingwei-a-zhang/WinAppSDK-DrumPad/tree/main/DrumPad
    /// </summary>
    class Program
    {
        public static event EventHandler<AppActivationArguments> OnActivated;
        [STAThread]
        static void Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            bool isRedirect = DecideRedirection();
            if (!isRedirect)
            {
                Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var context = new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }
        }

        private static bool DecideRedirection()
        {
            bool isRedirect = false;

            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
            ExtendedActivationKind kind = args.Kind;

            try
            {
                AppInstance keyInstance = AppInstance.FindOrRegisterForKey("randomKey");

                if (keyInstance.IsCurrent)
                {
                    keyInstance.Activated += _OnActivated;
                }
                else
                {
                    isRedirect = true;
                    RedirectActivationTo(args, keyInstance);
                }
            }

            catch (Exception ex)
            {

            }

            return isRedirect;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateEvent(
IntPtr lpEventAttributes, bool bManualReset,
bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(IntPtr hEvent);

        [DllImport("ole32.dll")]
        private static extern uint CoWaitForMultipleObjects(
            uint dwFlags, uint dwMilliseconds, ulong nHandles,
            IntPtr[] pHandles, out uint dwIndex);

        private static IntPtr redirectEventHandle = IntPtr.Zero;

        public static void RedirectActivationTo(
            AppActivationArguments args, AppInstance keyInstance)
        {
            redirectEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
            Task.Run(() =>
            {
                keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
                SetEvent(redirectEventHandle);
            });
            uint CWMO_DEFAULT = 0;
            uint INFINITE = 0xFFFFFFFF;
            _ = CoWaitForMultipleObjects(
               CWMO_DEFAULT, INFINITE, 1,
               new IntPtr[] { redirectEventHandle }, out uint handleIndex);
        }

        private static void _OnActivated(object sender, AppActivationArguments args)
        {
            ExtendedActivationKind kind = args.Kind;
            OnActivated?.Invoke(sender, args);
        }
    }
}
