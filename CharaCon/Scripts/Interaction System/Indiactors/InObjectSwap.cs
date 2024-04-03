using UnityEngine;

namespace Omnix.CharaCon.InteractionSystem
{
    /// <summary> Highlight an interactable by activating and deactivating visual gameObjects </summary>
    public class InObjectSwap : IndicatorBase
    {
        [SerializeField] private GameObject _normalObject;
        [SerializeField] private GameObject _highlightedObject;

        private void Start()
        {
            Unhighlight(null, null);
        }

        public override void Highlight(InteractableBase interactable, AgentInteraction player)
        {
            _normalObject.SetActive(false);
            _highlightedObject.SetActive(true);
        }

        public override void Unhighlight(InteractableBase interactable, AgentInteraction player)
        {
            _normalObject.SetActive(true);
            _highlightedObject.SetActive(false);
        }
    }
}