using System;
using UnityEngine;

namespace Omnix.Inputs
{
    [Serializable]
    public class UnitInput : ISerializationCallbackReceiver
    {
        [SerializeField] KeysModifier modifier;
        [SerializeField] KeyCode key;
        [SerializeField] CheckType keyCheckType;

        public KeysModifier Modifier => modifier;
        public KeyCode Key => key;
        public CheckType KeyCheckType => keyCheckType;
        
        private Func<bool> _isPressedModifier;
        private Func<KeyCode, bool> _isPressedKey;

        public void Init()
        {
            switch (modifier)
            {
                case (KeysModifier.None):
                {
                    _isPressedModifier = ModifierChecker.PressedNone;
                    break;
                }
                case (KeysModifier.Control):
                {
                    _isPressedModifier = ModifierChecker.PressedControl;
                    break;
                }
                case (KeysModifier.Alt):
                {
                    _isPressedModifier = ModifierChecker.PressedAlt;
                    break;
                }
                case (KeysModifier.Shift):
                {
                    _isPressedModifier = ModifierChecker.PressedShift;
                    break;
                }
                case (KeysModifier.ControlShift):
                {
                    _isPressedModifier = ModifierChecker.PressedControlShift;
                    break;
                }
                case (KeysModifier.AltShift):
                {
                    _isPressedModifier = ModifierChecker.PressedAltShift;
                    break;
                }
                case (KeysModifier.ControlAlt):
                {
                    _isPressedModifier = ModifierChecker.PressedControlAlt;
                    break;
                }
                case (KeysModifier.ControlAltShift):
                {
                    _isPressedModifier = ModifierChecker.PressedControlAltShift;
                    break;
                }
                default:
                {
                    _isPressedModifier = ModifierChecker.PressedNone;
                    break;
                }
            }

            if (keyCheckType == CheckType.Down) _isPressedKey = Input.GetKeyDown;
            if (keyCheckType == CheckType.Hold) _isPressedKey = Input.GetKey;
            if (keyCheckType == CheckType.Release) _isPressedKey = Input.GetKeyUp;
        }

        public bool IsPressed()
        {
            #if UNITY_EDITOR
            {
                Init();
            }
            #endif
            return _isPressedModifier() && _isPressedKey(key);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Init();
        }
        
        public override string ToString()
        {
            return $"{modifier}+{key} ({keyCheckType})";
        }
    }
}


