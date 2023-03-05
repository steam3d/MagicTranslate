using MagicTranslate.Helpers;
using MagicTranslate.Settings;
using MagicTranslate.UI.Theme;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using WinRT;

namespace MagicTranslate.UI
{
    public enum BackdropType
    {
        Mica,
        MicaAlt,
        DesktopAcrylic,
        DefaultColor,
    }

    internal class WindowBackdrops
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Window window;

        WindowsSystemDispatcherQueueHelper m_wsdqHelper;
        BackdropType m_currentBackdrop;
        Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController;
        Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController m_acrylicController;
        Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;

        public WindowBackdrops(Window window)
        {
            this.window = window;
            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            string savedBackground = (string)GlobalSettings.LoadHeadphoneSetting("ApplicationSettings", "Background");
            if (!string.IsNullOrEmpty(savedBackground))
                SetBackdrop(EnumHelper.GetEnum<BackdropType>(savedBackground));

            //GlobalSettings.OnSettingChange += GlobalSettings_OnSettingChange;
        }

        private void GlobalSettings_OnSettingChange(object sender, Args.SettingArgs e)
        {
            if (e.ContainerName == "ApplicationSettings" && e.SettingName == "Background")
            {
                SetBackdrop(EnumHelper.GetEnum<BackdropType>((string)e.NewValue));
            }
        }

        public void SetBackdrop(BackdropType type)
        {
            // Reset to default color. If the requested type is supported, we'll update to that.
            // Note: This sample completely removes any previous controller to reset to the default
            //       state. This is done so this sample can show what is expected to be the most
            //       common pattern of an app simply choosing one controller type which it sets at
            //       startup. If an app wants to toggle between Mica and Acrylic it could simply
            //       call RemoveSystemBackdropTarget() on the old controller and then setup the new
            //       controller, reusing any existing m_configurationSource and Activated/Closed
            //       event handlers.
            m_currentBackdrop = BackdropType.DefaultColor;
            if (m_micaController != null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }
            if (m_acrylicController != null)
            {
                m_acrylicController.Dispose();
                m_acrylicController = null;
            }
            window.Activated -= Window_Activated;
            window.Closed -= Window_Closed;

            //Error appears when GlobalSettings_OnSettingChange raised once. After it close the settings page and open it again. Try to change background and you will get this error.
#warning The operation identifier is not valid. (0x800710DD)
            ((FrameworkElement)window.Content).ActualThemeChanged -= Window_ThemeChanged;
            m_configurationSource = null;

            if (type == BackdropType.Mica)
            {
                if (TrySetMicaBackdrop(false))
                {
                    Logger.Debug("Mica");
                    m_currentBackdrop = type;
                }
                else
                {
                    // Mica isn't supported. Try Acrylic.
                    type = BackdropType.DesktopAcrylic;
                    Logger.Error("  Mica isn't supported. Trying Acrylic.");
                }
            }
            if (type == BackdropType.MicaAlt)
            {
                if (TrySetMicaBackdrop(true))
                {
                    Logger.Debug("MicaAlt");
                    m_currentBackdrop = type;
                }
                else
                {
                    // MicaAlt isn't supported. Try Acrylic.
                    type = BackdropType.DesktopAcrylic;
                    Logger.Error("  MicaAlt isn't supported. Trying Acrylic.");
                }
            }
            if (type == BackdropType.DesktopAcrylic)
            {
                if (TrySetAcrylicBackdrop())
                {
                    Logger.Debug("Acrylic");
                    m_currentBackdrop = type;
                }
                else
                {
                    // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
                    Logger.Error("  Acrylic isn't supported. Switching to default color.");
                }
            }
        }

        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                // Hooking up the policy object
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                window.Activated += Window_Activated;
                window.Closed += Window_Closed;
                ((FrameworkElement)window.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                if (useMicaAlt)
                {
                    m_micaController.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt;
                }
                else
                {
                    m_micaController.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
                }

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_micaController.AddSystemBackdropTarget(window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }

        bool TrySetAcrylicBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
            {
                // Hooking up the policy object
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                window.Activated += Window_Activated;
                window.Closed += Window_Closed;
                ((FrameworkElement)window.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_acrylicController = new Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController();
                UpdateAcrylicTint();
                
                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_acrylicController.AddSystemBackdropTarget(window.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Acrylic is not supported on this system
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (m_configurationSource != null)
                m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
            if (m_micaController != null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }
            if (m_acrylicController != null)
            {
                m_acrylicController.Dispose();
                m_acrylicController = null;
            }
            window.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
                UpdateAcrylicTint();
            }
        }

        private void UpdateAcrylicTint()
        {
            if (m_acrylicController == null)
                return;

            var isDarkTheme = ThemeManagement.IsDarkTheme();
            float opacity = isDarkTheme ? 0.75f : 0.65f;


            if (((FrameworkElement)window.Content).Resources.TryGetValue("ApplicationPageBackgroundThemeBrush", out object value))
            {
                var brush = (SolidColorBrush)value;
                m_acrylicController.TintColor = brush.Color;
            }
            m_acrylicController.TintOpacity = opacity;
            m_acrylicController.LuminosityOpacity = 1;
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)window.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }
    }
}
