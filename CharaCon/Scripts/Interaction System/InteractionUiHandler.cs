using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omnix.CharaCon.InteractionSystem
{
    public class InteractionUiHandler : MonoBehaviour
    {
        private static InteractionUiHandler instance;

        [SerializeField] private GameObject _activeWithUi;
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private TextMeshProUGUI _descriptionText;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                instance._activeWithUi.SetActive(false);
            }
            else Destroy(gameObject);
        }
        
        private void Setup(InteractableBase interactable)
        {
            interactable.description.Get(out string keycode, out string description);
            _keyText.SetText(keycode);
            _descriptionText.SetText(description);
        }

        public static void HideAllUi()
        {
            instance._activeWithUi.SetActive(false);
        }

        public static void ToggleInteractionUi(InteractableBase interactable, bool isOn)
        {
            instance._activeWithUi.SetActive(isOn);
            if (isOn) instance.Setup(interactable);

        }

        public static void ShowMultipleInteraction(IEnumerable<InteractableBase> interactions, Action<InteractableBase> onConfirm, Action onCancel)
        {
            foreach (InteractableBase interaction in interactions)
            {
                onConfirm?.Invoke(interaction);
                break;
            }    
        }
    }
    
    /*public class InteractionUiHandler : MonoBehaviour
    {
        [CanBeNull] private static InteractionUiHandler instance;

        #region Fields
        [Header("Prefabs")] [SerializeField] private InteractionUiElement _keyPrefab;
        [SerializeField] private InteractionUiElement _elementPrefab;

        [Header("UI Parents")] [SerializeField]
        private Transform _keysParent;

        [SerializeField] private Transform _elementsParent;

        private Action _mimCancelCallback; // mim: Multiple Interactions Menu
        private Action<IntractableBase> _mimConfirmCall; // mim: Multiple Interactions Menu
        private Dictionary<IntractableBase, InteractionUiElement> _activeElements;
        private bool _showingMultipleInteractions;
        #endregion

        #region Unity Callback
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            _activeElements = new Dictionary<IntractableBase, InteractionUiElement>();
            _showingMultipleInteractions = false;
        }

        private void Update()
        {
            if (!_showingMultipleInteractions) return;
            if (Input.GetMouseButtonDown(0))
            {
                HideAllUi();
                _mimCancelCallback?.Invoke();
                return;
            }

            bool hideAll = false;
            foreach (var element in _activeElements)
            {
                if (element.Value.CheckConfirmation())
                {
                    _mimConfirmCall?.Invoke(element.Key);
                    hideAll = true;
                    break;
                }
            }

            if (hideAll)
            {
                HideAllUi();
            }
        }
        #endregion

        #region Functionalities
        /// <summary> Finds interaction (with active UI button) that has given keycode as activation key </summary>
        private InteractionUiElement GetInteractionOfSameKey(KeyCode keyCode)
        {
            foreach (var pair in _activeElements)
            {
                if (pair.Key.InteractionKey == keyCode)
                {
                    return pair.Value;
                }
            }

            return null;
        }

        /// <summary> Show UI for activation key for the give interactable </summary>
        private static void ShowInteractionKey(IntractableBase intractable)
        {
            if (instance == null) return;
            if (instance._activeElements.ContainsKey(intractable)) return;

            InteractionUiElement element = instance.GetInteractionOfSameKey(intractable.InteractionKey);
            if (element == null) element = Instantiate(instance._keyPrefab, instance._keysParent);

            element.AddInteractable(intractable);
            instance._activeElements.Add(intractable, element);
        }

        /// <summary> Hide UI for activation key for the give interactable </summary>
        private static void HideInteractionKey(IntractableBase intractable)
        {
            if (instance == null) return;
            if (!instance._activeElements.ContainsKey(intractable)) return;
            instance._activeElements[intractable].RemoveInteractable(intractable);
            instance._activeElements.Remove(intractable);
        }

        /// <summary> Hide all active UI </summary>
        public static void HideAllUi()
        {
            if (instance == null) return;
            foreach (InteractionUiElement element in instance._activeElements.Values)
            {
                Destroy(element.gameObject);
            }

            instance._activeElements.Clear();
            instance._showingMultipleInteractions = false;
        }

        /// <summary> Show list view for all the interactableElements with a number key associated with each of them </summary>
        public static void ShowMultipleInteraction(IEnumerable<IntractableBase> interactableElements, Action<IntractableBase> confirmCallback, Action cancelCallback)
        {
            if (instance == null) return;
            HideAllUi();
            int index = 1;

            foreach (IntractableBase interactable in interactableElements)
            {
                if (index > 9) break;

                InteractionUiElement element = Instantiate(instance._elementPrefab, instance._elementsParent);
                element.SetupForOne(interactable, index);
                instance._activeElements.Add(interactable, element);
                index++;
            }

            instance._mimConfirmCall = confirmCallback;
            instance._mimCancelCallback = cancelCallback;
            instance._showingMultipleInteractions = true;
        }

        /// <summary> Toggle the key-indication ui for a specific interactable  </summary>
        public static void ToggleInteractionUi(IntractableBase io, bool value)
        {
            if (value) ShowInteractionKey(io);
            else HideInteractionKey(io);
        }
        #endregion
    }*/
}