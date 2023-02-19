using System.Globalization;

namespace MagicTranslate.Args
{
    internal class TranslateArgs
    {
        public CultureInfo TranslateFrom { get; }
        public CultureInfo TranslateTo { get; }
        public string TextToTranslate { get; }

        public TranslateArgs(CultureInfo translateFrom, string textToTranslate, CultureInfo translateTo)
        {
            TranslateFrom = translateFrom;
            TranslateTo = translateTo;
            TextToTranslate = textToTranslate;
        }
    }
}
