using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace MagicTranslate
{
    class Program
    {
        public static event EventHandler<AppActivationArguments> OnActivated;
        [STAThread]
        static async Task<int> Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            bool isRedirect = await DecideRedirection();
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
            return 0;
        }

        private static async Task<bool> DecideRedirection()
        {
            bool isRedirect = false;
            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
            ExtendedActivationKind kind = args.Kind;
            AppInstance keyInstance = AppInstance.FindOrRegisterForKey("randomKey");

            if (keyInstance.IsCurrent)
            {
                keyInstance.Activated += _OnActivated;
            }
            else
            {
                isRedirect = true;
                await keyInstance.RedirectActivationToAsync(args);
            }
            return isRedirect;
        }

        private static void _OnActivated(object sender, AppActivationArguments args)
        {
            ExtendedActivationKind kind = args.Kind;
            OnActivated?.Invoke(sender, args);
        }
    }
}
