using MagicTranslate.Hotkeys;
using Windows.System;

namespace MagicTranslate.Helpers
{
    public class ModifiersHelper
    {
        /// <summary>
        /// Convert <see cref="Modifiers"/> int to <see cref="VirtualKeyModifiers"/> int
        /// </summary>
        /// <param name="modifiersInt"></param>
        /// <returns></returns>
        public static int ConvertModifiersIntToVirtualKeyModifiersInt(int modifiersInt)
        {
            int virtualKeyModifiersInt = 0;

            if (modifiersInt >= (int)Modifiers.NoRepeast)
            {
                //VirtualKeyModifiers does not have NoRepeast
                modifiersInt -= (int)Modifiers.NoRepeast;                
            }

            if (modifiersInt >= (int)Modifiers.Windows)
            {
                modifiersInt -= (int)Modifiers.Windows;
                virtualKeyModifiersInt |= (int)VirtualKeyModifiers.Windows;
            }

            if (modifiersInt >= (int)Modifiers.Shift)
            {
                modifiersInt -= (int)Modifiers.Shift;
                virtualKeyModifiersInt |= (int)VirtualKeyModifiers.Shift;
            }

            if (modifiersInt >= (int)Modifiers.Control)
            {
                modifiersInt -= (int)Modifiers.Control;
                virtualKeyModifiersInt |= (int)VirtualKeyModifiers.Control;
            }

            if (modifiersInt >= (int)Modifiers.Alt)
            {
                // I have no idea why alt = menu
                modifiersInt -= (int)Modifiers.Alt;
                virtualKeyModifiersInt |= (int)VirtualKeyModifiers.Menu;
            }
            return virtualKeyModifiersInt;
        }

        /// <summary>
        /// Convert <see cref="VirtualKeyModifiers"/> int to <see cref="Modifiers"/> int
        /// </summary>
        /// <param name="virtualKeyModifiersInt"></param>
        /// <returns></returns>
        public static int ConvertVirtualKeyModifiersIntToModifiersInt(int virtualKeyModifiersInt)
        {
            int modifiersInt = 0;

            if (virtualKeyModifiersInt >= (int)VirtualKeyModifiers.Windows)
            {
                virtualKeyModifiersInt -= (int)VirtualKeyModifiers.Windows;
                modifiersInt |= (int)Modifiers.Windows;
            }

            if (virtualKeyModifiersInt >= (int)VirtualKeyModifiers.Shift)
            {
                virtualKeyModifiersInt -= (int)VirtualKeyModifiers.Shift;
                modifiersInt |= (int)Modifiers.Shift;
            }

            if (virtualKeyModifiersInt >= (int)VirtualKeyModifiers.Menu)
            {
                virtualKeyModifiersInt -= (int)VirtualKeyModifiers.Menu;
                modifiersInt |= (int)Modifiers.Alt;
            }

            if (virtualKeyModifiersInt >= (int)VirtualKeyModifiers.Control)
            {
                virtualKeyModifiersInt -= (int)VirtualKeyModifiers.Control;
                modifiersInt |= (int)Modifiers.Control;
            }

            return modifiersInt;
        }

    }
}
