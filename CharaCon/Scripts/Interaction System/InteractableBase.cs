using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Omnix.CharaCon.InteractionSystem
{
    [Serializable]
    public class InteractionDescription
    {
        [SerializeField] private string _keycode;
        [SerializeField] private string _description;

        public InteractionDescription()
        {
            _keycode = "E";
        }

        public void Get(out string keycode, out string description)
        {
            keycode = _keycode;
            description = _description;
        }
    }

    [RequireComponent(typeof(Collider))]
    public abstract class InteractableBase : MonoBehaviour
    {
        #region Fields
        public InteractionDescription description;
        
        /// <summary> Player that is currently interacting with this object. </summary>
        [NonSerialized, CanBeNull] protected IndicatorBase indicator;
        public bool IsInteracting { get; private set; }
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            indicator = GetComponent<IndicatorBase>();
        }
        #endregion

        #region Abstract
        /// <summary> Callback when the player Interact with this object </summary>
        public abstract void OnInteract(AgentInteraction agentInteraction);

        /// <summary> Callback when the player Un-Interacts with this object </summary>
        public abstract void OnUnInteract(AgentInteraction agentInteraction);
        #endregion

        #region Virtuals
        /// <summary> Is interaction valid. </summary>
        /// <returns> true if player can interact with this object, false otherwise </returns>
        public virtual bool IsValid(AgentInteraction agentInteraction)
        {
            return true;
        }

        /// <summary> Toggle highlight of this object. Indicating the player that they can interact with this object. </summary>
        public virtual void ToggleHighlightItem(AgentInteraction agentInteraction, bool isOn)
        {
            InteractionUiHandler.ToggleInteractionUi(this, isOn);
            if (indicator == null) return;
            if (isOn) indicator.Highlight(this, agentInteraction);
            else indicator.Unhighlight(this, agentInteraction);
        }

        /// <summary> Should the player interact with this object in this frame. </summary>
        /// <returns> true if the interaction should begin, false otherwise. </returns>
        public virtual bool ShouldInteractInThisFrame(AgentInteraction agentInteraction)
        {
            return AgentInput.InputMap.BasicAbilities.Interact.IsInProgress();
        }
        #endregion
        
        #region Internal
        internal void _InternalStartInteraction(AgentInteraction agentInteraction)
        {
            IsInteracting = true;
            OnInteract(agentInteraction);
        }

        internal void _InternalEndInteraction(AgentInteraction agentInteraction)
        {
            IsInteracting = false;
            OnUnInteract(agentInteraction);
        }
        #endregion
    }
}