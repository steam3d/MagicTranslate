using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using Windows.Win32;
using Windows.Win32.Foundation;

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
                AppInstance keyInstance = AppInstance.FindOrRegisterForKey("MagicTranslateInstance");

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

        private static Microsoft.Win32.SafeHandles.SafeFileHandle redirectEventHandle;

        public static void RedirectActivationTo(
            AppActivationArguments args, AppInstance keyInstance)
        {
            redirectEventHandle = PInvoke.CreateEvent(null, true, false, (string)null);

            Task.Run(() =>
            {
                keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
                PInvoke.SetEvent(redirectEventHandle);
            });

            uint CWMO_DEFAULT = 0;
            uint INFINITE = 0xFFFFFFFF;
            var handle = new HANDLE(redirectEventHandle.DangerousGetHandle());
            _ = PInvoke.CoWaitForMultipleObjects(CWMO_DEFAULT, INFINITE, new HANDLE[] { handle }, out uint handleIndex1);
        }

        private static void _OnActivated(object sender, AppActivationArguments args)
        {
            ExtendedActivationKind kind = args.Kind;
            OnActivated?.Invoke(sender, args);
        }
    }
}
