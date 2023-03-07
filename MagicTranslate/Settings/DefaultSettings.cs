using Windows.Foundation.Collections;

namespace MagicTranslate.Settings
{
    class DefaultSettings
    {
        /// <summary>
        /// Current application setting version. Specified by developer. Help to migrate setting from old format to the new one. Increase by one.
        /// </summary>
        public const uint Version = 0;

        public static ValueSet Settings = new ValueSet()
        {
            ["Theme"] = "Default",
            ["Background"] = "Mica",
            ["HotkeyOpenSearchBar"] = new Windows.Storage.ApplicationDataCompositeValue() { ["modifiers"] = 2, ["key"] = 32 },
            ["GoogleTranslateFrom"] = "LanguageNotSelected",
            ["GoogleTranslateTo"] = "SystemLanguage",
            ["LogLevel"] = "Info",
            ["SkipTutorial"] = false,
        };
    }
}
