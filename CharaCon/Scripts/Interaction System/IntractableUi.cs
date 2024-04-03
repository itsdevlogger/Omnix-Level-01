using TMPro;
using UnityEngine;
namespace Omnix.CharaCon.InteractionSystem
{
    /*public class InteractionUiElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _interactionTitleText;
        [SerializeField] private TextMeshProUGUI _interactionKeyText;

        private readonly HashSet<IntractableBase> _managedIntractables = new HashSet<IntractableBase>();
        private KeyCode _checkCode;

        private void Awake()
        {
            _interactionTitleText.text = "";
            _interactionKeyText.text = "";
        }
        
        /// <summary> Update title for this UI Element, assuming that this element manages multiple interactable </summary>
        private void UpdateAllInteractableTitles()
        {
            if (_interactionTitleText == null) return;
            
            StringBuilder builder = new StringBuilder();
            foreach (IntractableBase interactable in _managedIntractables)
            {
                string text = interactable.interactionText;
                if (!string.IsNullOrEmpty(text)) builder.AppendLine(text);
            }
            _interactionTitleText.text = builder.ToString();
        }
        
        /// <summary>
        /// Setup this UI element assuming the given interactable is the only interactable managed by this key
        /// Used to display listview of Multiple-Interactions
        /// </summary>
        public void SetupForOne(IntractableBase intractable, int index)
        {
            if (_interactionTitleText != null)
            {
                string text = intractable.interactionText;
                if (string.IsNullOrEmpty(text)) _interactionTitleText.text = "Interact";
                else _interactionTitleText.text = text;
            }

            if (_interactionKeyText != null)
            {
                _interactionKeyText.text = $"Press {index}: ";
                _checkCode = (KeyCode)(48 + index);
            }
        }

        /// <summary> Add interactable to managed list. Assumes that all the intractable in managed list have same activation key. </summary>
        public void AddInteractable(IntractableBase intractable)
        {
            _managedIntractables.Add(intractable);
            UpdateAllInteractableTitles();

            if (_interactionKeyText != null)
            {
                _interactionKeyText.text = intractable.InteractionKey.ToString();
                _checkCode = KeyCode.None;
            }
        }

        /// <summary> Remove interactable from managed list. Assumes that all the intractable in managed list have same activation key. </summary>
        public void RemoveInteractable(IntractableBase intractable)
        {
            if (intractable == null) return;
            
            _managedIntractables.Remove(intractable);
            if (_managedIntractables.Count == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                UpdateAllInteractableTitles();
            }
        }

        /// <summary> Check if the required key is pressed. </summary>
        public bool CheckConfirmation()
        {
            return _checkCode != KeyCode.None && Input.GetKeyUp(_checkCode);
        }
    }*/
}