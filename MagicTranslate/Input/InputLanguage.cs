using NLog;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Win32;

namespace MagicTranslate.Input
{
    internal class InputLanguage
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Task getInputLanguageLoop;
        CancellationTokenSource cancelToken = new CancellationTokenSource();
        private short cultureId = 0;

        public CultureInfo CurrentInput { get => new CultureInfo(cultureId); }

        public event EventHandler<CultureInfo> CurrentInputChanged;

        public InputLanguage()
        {            
            getInputLanguageLoop = Task.Run(() => GetInputLanguageLoop(), cancelToken.Token);
        }

        public void Dispose()
        {
            cancelToken.Cancel();
            getInputLanguageLoop.Wait();
            cancelToken.Dispose();
        }

        private void GetInputLanguageLoop()
        {
            while (cancelToken.IsCancellationRequested == false)
            {
                uint threadId = 0;
                unsafe
                {
                    threadId = PInvoke.GetWindowThreadProcessId(PInvoke.GetForegroundWindow());
                }

                var l = PInvoke.GetKeyboardLayout(threadId);
                var id = (short)Convert.ToInt64(l);

                if (cultureId != id)
                {
                    cultureId = id;
                    var culture = new CultureInfo((short)Convert.ToInt64(l));
                    CurrentInputChanged?.Invoke(this, culture);
                    Logger.Debug(culture.TwoLetterISOLanguageName);

                }
                Thread.Sleep(100);
            }
            Logger.Debug("Stopped");
        }
    }
}
