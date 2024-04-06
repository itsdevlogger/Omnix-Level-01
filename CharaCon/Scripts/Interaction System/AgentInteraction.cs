using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Omnix.CharaCon.InteractionSystem
{
    public class AgentInteraction : MonoBehaviour
    {
        #region Fields
        [SerializeField] private int _maxSimultaneousTargets = 3;

        [SerializeField, Tooltip("Max distance from which player can start interaction")]
        private float _maxInteractionDistance;

        [SerializeField, Tooltip("Auto-end interaction when player to interactable distance is more than range.")]
        private float _interactionRange;

        [FormerlySerializedAs("_intractableLayer")] [SerializeField]
        private LayerMask _interactableLayer;


        /// <summary> Is the player currently interacting with any object </summary>
        public bool IsInteracting { get; private set; } = false;

        /// <summary> Objects that are being targeted for interaction. </summary>
        private readonly HashSet<InteractableBase> _targetedObjects = new HashSet<InteractableBase>();

        /// <summary>
        /// if <see cref="IsInteracting"/> is true, then this is the objects that player is interacting with. This set will contain exactly one element.
        /// if <see cref="IsInteracting"/> is false, then the player has confirmed interaction with all of these object and we are currently waiting for the player to choose exactly one of these 
        /// </summary>
        private readonly HashSet<InteractableBase> _interactingWith = new HashSet<InteractableBase>();

        /// <summary> All the objects to which ray cast has hit in this frame </summary>
        private RaycastHit[] _rayCastHits;

        private bool _canInteract = true; 
        public bool CanInteract
        {
            get => _canInteract;
            set
            {
                _canInteract = value;
                if (value == false && IsInteracting == false)
                {
                    OnAllTargetsLost();
                    InteractionUiHandler.HideAllUi();
                }
            }
        }
        #endregion

        #region Unity Callbacks
        public void OnEnable()
        {
            _rayCastHits = new RaycastHit[_maxSimultaneousTargets];
            if (Agent.Current != null && Agent.Current.Health != null) Agent.Current.Health.OnDeath += OnDeath;
            Agent.OnSwitched += OnAgentSwitched;
            AgentInput.OnSetInputActive += OnSetInputActive;
        }

        public void OnDisable()
        {
            if (Agent.Current != null && Agent.Current.Health != null) Agent.Current.Health.OnDeath -= OnDeath;
            Agent.OnSwitched -= OnAgentSwitched;
            AgentInput.OnSetInputActive += OnSetInputActive;
        }

        private void Update()
        {
            if (IsInteracting || _interactingWith.Count > 0)
            {
                CheckInteractionOver();
            }
            else if (_canInteract)
            {
                UpdateTargets();
                UpdateInteraction();
            }
        }
        #endregion

        #region Functionalities
        private void OnDeath()
        {
            if (IsInteracting)
            {
                EndInteraction();
            }
        }
        
        private void OnSetInputActive(bool value)
        {
            CanInteract = value;
        }

        private void OnAgentSwitched(Agent oldAgent, Agent newAgent)
        {
            if (oldAgent.Health != null) oldAgent.Health.OnDeath -= OnDeath;
            if (newAgent.Health != null) newAgent.Health.OnDeath += OnDeath;
        }

        private void CheckInteractionOver()
        {
            InteractableBase item = _interactingWith.First();
            float distance = Vector3.Distance(item.transform.position, transform.position);
            if (distance > _interactionRange)
            {
                if (IsInteracting)
                {
                    EndInteraction();
                }
                else
                {
                    OnAllTargetsLost();
                    InteractionUiHandler.HideAllUi();
                }
            }
        }

        /// <summary>
        /// Updates the objects that are being targeted
        /// </summary>
        private void UpdateTargets()
        {
            Ray ray = AgentCamera.Current.ScreenPointToRay(AgentInput.MousePosition);
            int hitCount = Physics.RaycastNonAlloc(ray, _rayCastHits, _maxInteractionDistance, _interactableLayer);

            if (hitCount == 0)
            {
                // If ray cast hits nothing, then hide the ui
                if (_targetedObjects.Count != 0 || _interactingWith.Count != 0)
                {
                    InteractionUiHandler.HideAllUi();
                    OnAllTargetsLost();
                }
            }
            else
            {
                if (hitCount > 1) QuickSortHitsDistance(0, hitCount);
                UpdateTargetsInner(hitCount);
            }
        }

        /// <summary>
        /// Updates the target <see cref="_targetedObjects"/> list based on <see cref="_rayCastHits"/> array
        /// </summary>
        private void UpdateTargetsInner(int hitCount)
        {
            var cachedComponents = new HashSet<InteractableBase>();
            for (int i = 0; i < hitCount; i++)
            {
                Collider hitCollider = _rayCastHits[i].collider;
                if (hitCollider == null) continue;

                if (hitCollider.TryGetComponent(out InteractableBase component))
                {
                    cachedComponents.Add(component);
                }
                else if (hitCollider.isTrigger == false)
                {
                    // As the _rayCastHits array is sorted by distance from camera,
                    // The first non-trigger collider we find, will be blocking the player's view.
                    break;
                }
            }


            // If a component was targeted in last frame, and is not found in RayCast of this frame
            // Then that target is LOST
            _targetedObjects.RemoveWhere(io =>
            {
                if (cachedComponents.Contains(io)) return false;
                
                io.ToggleHighlightItem(this, false);
                return true;
            });


            // If a component was not targeted in last frame, and is found in RayCast of this frame
            // Then that target is FOUND
            foreach (InteractableBase interactable in cachedComponents)
            {
                if (_targetedObjects.Contains(interactable) || !interactable.IsValid(this)) continue;
                
                _targetedObjects.Add(interactable);
                interactable.ToggleHighlightItem(this, true);
            }
        }

        /// <summary>
        /// Check if any target interaction is finalized by the player
        /// </summary>
        private void UpdateInteraction()
        {
            if (_targetedObjects.Count == 0)
            {
                return;
            }

            // No need to clear _interactingWith before logic,
            // Because if it has any element, then this method won't be called
            // Check Update method
            foreach (InteractableBase io in _targetedObjects)
            {
                if (io.ShouldInteractInThisFrame(this))
                {
                    _interactingWith.Add(io);
                }
            }

            int count = _interactingWith.Count;
            if (count == 1)
            {
                InteractableBase element = _interactingWith.First();
                BeginInteraction(element);
            }
            else if (count > 1)
            {
                InteractionUiHandler.ShowMultipleInteraction(_interactingWith, BeginInteraction, OnAllTargetsLost);
            }
        }
        
        /// <summary>
        /// Called when the raycast hits no object and we are not interacting with anything
        /// </summary>
        private void OnAllTargetsLost()
        {
            IsInteracting = false;
            foreach (InteractableBase io in _interactingWith.Concat(_targetedObjects))
            {
                io.ToggleHighlightItem(this, false);
            }

            _targetedObjects.Clear();
            _interactingWith.Clear();
        }

        /// <summary>
        /// Called when player has chosen exactly one object to interact with.
        /// </summary>
        public void BeginInteraction(InteractableBase interactable)
        {
            // Check if it can be interacted with
            if (/*!_targetedObjects.Contains(interactable) ||*/ IsInteracting) return;

            // Unhighlight all that we are not interacting with
            _targetedObjects.Remove(interactable);
            _interactingWith.Remove(interactable);
            OnAllTargetsLost();

            // Update fields
            // Must set these field before calling OnInteract. Because some object interact instantaneously
            // And call OnInteractedFinished in the same frame, which messes up the flow.
            _interactingWith.Add(interactable);
            IsInteracting = true;

            // Hide all the UI
            InteractionUiHandler.HideAllUi();

            // Invoke Callbacks
            interactable.ToggleHighlightItem(this, false);
            interactable._InternalStartInteraction(this);
        }

        /// <summary> Force end the player's interaction </summary>
        public void EndInteraction()
        {
            InteractionUiHandler.HideAllUi();
            if (_interactingWith.Count == 0) return;

            InteractableBase interactable = _interactingWith.ElementAt(0);
            IsInteracting = false;
            _interactingWith.Clear();
            _targetedObjects.Clear();

            interactable._InternalEndInteraction(this);
        }

        /// <summary> Force end the player's interaction after delay (in sec) </summary>
        public void EndInteractionDelayed(float delay)
        {
            Invoke(nameof(EndInteraction), delay);
        }

        private void QuickSortHitsDistance(int low, int high)
        {
            if (low >= high) return;

            float distance = _rayCastHits[high].distance;
            int i1 = low;
            for (int i2 = low; i2 < high; ++i2)
            {
                if (_rayCastHits[i2].distance < distance)
                {
                    (_rayCastHits[i1], _rayCastHits[i2]) = (_rayCastHits[i2], _rayCastHits[i1]);
                    ++i1;
                }
            }

            (_rayCastHits[i1], _rayCastHits[high]) = (_rayCastHits[high], _rayCastHits[i1]);
            QuickSortHitsDistance(low, i1 - 1);
            QuickSortHitsDistance(i1 + 1, high);
        }
        #endregion
    }
}