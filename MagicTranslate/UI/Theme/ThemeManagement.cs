using System;
using Windows.Storage;
using Microsoft.UI.Xaml;
using MagicTranslate.Helper;
using System.Reflection;
using Windows.UI.ViewManagement;
using Microsoft.UI;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Dispatching;
using System.Threading;

namespace MagicTranslate.UI.Theme
{
    /// <summary>
    /// Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeManagement
    {
        private static UISettings uiSettings;
        private const string SelectedAppThemeKey = "SelectedAppTheme";

#if !UNPACKAGED
        private static Window CurrentApplicationWindow;
#endif
        /// <summary>
        /// Gets the current actual theme of the app based on the requested theme of the
        /// root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme
        {
            get
            {
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        if (rootElement.RequestedTheme != ElementTheme.Default)
                        {
                            return rootElement.RequestedTheme;
                        }
                    }
                }
                return GetEnum<ElementTheme>(App.Current.RequestedTheme.ToString());
            }
        }

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        return rootElement.RequestedTheme;
                    }
                }

                return ElementTheme.Default;
            }
            set
            {
                foreach (Window window in WindowHelper.ActiveWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = value;
                    }
                }
                UpdateTitleBarColor();

#if !UNPACKAGED
                ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey] = value.ToString();
#endif
            }
        }

        public static void Initialize()
        {
#if !UNPACKAGED
            // Save reference as this might be null when the user is in another app
            CurrentApplicationWindow = App.StartupWindow;
            string savedTheme = ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey]?.ToString();

            if (savedTheme != null)
            {
                RootTheme = GetEnum<ElementTheme>(savedTheme);
            }
#endif
            uiSettings = new UISettings();
            uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
        }

        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
#if !UNPACKAGED
            if (CurrentApplicationWindow != null)
            {
                CurrentApplicationWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateTitleBarColor();
                });
            }                        
#endif
        }

        private static void UpdateTitleBarColor()
        {
            var res = Microsoft.UI.Xaml.Application.Current.Resources;

            res["WindowCaptionBackground"] = new SolidColorBrush(Colors.Transparent);
            res["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);
            res["WindowCaptionButtonBackground"] = new SolidColorBrush(Colors.Transparent);

            if (IsDarkTheme())
            {
                res["WindowCaptionForeground"] = new SolidColorBrush(Colors.White);
                res["WindowCaptionForegroundDisabled"] = new SolidColorBrush(Colors.White);
                res["WindowCaptionButtonBackgroundPointerOver"] = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
                res["WindowCaptionButtonBackgroundPressed"] = new SolidColorBrush(Color.FromArgb(14, 255, 255, 255));
            }
            else
            {
                res["WindowCaptionForeground"] = new SolidColorBrush(Colors.Black);
                res["WindowCaptionForegroundDisabled"] = new SolidColorBrush(Colors.Black);
                res["WindowCaptionButtonBackgroundPointerOver"] = new SolidColorBrush(Color.FromArgb(12, 0, 0, 0));
                res["WindowCaptionButtonBackgroundPressed"] = new SolidColorBrush(Color.FromArgb(6, 0, 0, 0));
            }
        }

        public static bool IsDarkTheme()
        {
            if (RootTheme == ElementTheme.Default)
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            }
            return RootTheme == ElementTheme.Dark;
        }

        private static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }
    }
}
