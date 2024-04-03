using UnityEngine;

namespace Omnix.CharaCon.InteractionSystem
{
    /// <summary> Base class for all the class which handle highlighting and un-highlighting and Interactable Object. </summary>
    public abstract class IndicatorBase : MonoBehaviour
    {
        /// <summary> Highlight this object, indicating player can interact with it </summary>
        public abstract void Highlight(InteractableBase interactable, AgentInteraction player);

        /// <summary> Unhighlight this object, indicating player can't interact with it </summary>
        public abstract void Unhighlight(InteractableBase interactable, AgentInteraction player);
    }
}
