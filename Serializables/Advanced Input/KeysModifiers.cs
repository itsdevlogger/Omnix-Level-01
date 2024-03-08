using UnityEngine;

namespace Omnix.Inputs
{
    public enum KeysModifier
    {
        None,
        Control,
        Alt,
        Shift,
        ControlShift,
        AltShift,
        ControlAlt,
        ControlAltShift
    }

    public static class ModifierChecker
    {
        private static bool Control => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        private static bool Alt => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        private static bool Shift => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        public static bool PressedNone() => true;
        public static bool PressedControl() => Control && !Shift && !Alt;
        public static bool PressedAlt() =>     Alt     && !Shift && !Control;
        public static bool PressedShift() =>   Shift   && !Alt   && !Control;
        
        public static bool PressedControlShift() => Control && Shift && !Alt;
        public static bool PressedAltShift() =>     Alt     && Shift && !Control;
        public static bool PressedControlAlt() =>   Control && Alt   && !Shift;
        
        public static bool PressedControlAltShift() => Control && Alt && Shift;

    }
}

