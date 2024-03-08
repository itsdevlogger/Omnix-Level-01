using Omnix.Inputs;
using UnityEngine;

namespace Omnix.Inputs
{
    [RequireComponent(typeof(IKeyEventListener))]
    public class AdvancedInputEventCaller : MonoBehaviour
    {
        [SerializeField] private AdvancedInput keycode;
    
        void Update()
        {
            if (keycode.GetPressedCombo(out UnitInput combo))
            {
                GetComponent<IKeyEventListener>().OnKeyDown(combo);
            }
        }
    }

    public interface IKeyEventListener
    {
        void OnKeyDown(UnitInput combo);
    }
}

