using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omnix.CharaCon.Abilities
{
    public enum InputHandling
    {
        Ignore,
        Toggle,
        Enable,
        Disable,
    }

    /// <remarks> set <see cref="BaseAbility.enabled"/> = true if the ability is being used, otherwise set <see cref="BaseAbility.enabled"/> = false </remarks>
    public abstract class BaseAbility : MonoBehaviour
    {
        #region Static Events
        public static event Action<BaseAbility> OnAbilityEnabled;
        public static event Action<BaseAbility> OnAbilityDisabled;
        #endregion

        // @formatter:off
        #region Fields
        [Header("References")]
        [SerializeField] private Agent _agent;
        [SerializeField] private Animator _agentAnimator;
        
        [Space, Header("Settings")]
        [SerializeField, Tooltip(_.ABILITY_INDEX)]                    private int _abilityIndex;
        [SerializeField, Tooltip(_.ABILITY_INPUT_START_HANDLING)]     public InputHandling inputStartedHandling;
        [SerializeField, Tooltip(_.ABILITY_INPUT_PERFORMED_HANDLING)] public InputHandling inputPerformedHandling;
        [SerializeField, Tooltip(_.ABILITY_INPUT_CANCEL_HANDLING)]    public InputHandling inputCanceledHandling;
        
        public int AbilityIndex               => _abilityIndex;
        public Agent Agent                    => _agent;
        public Camera MainCamera              => AgentCamera.Current;
        public Animator Animator              => _agentAnimator;
        public PlayerInputMap InputMap        => AgentInput.Instance.InputMap;
        public CharacterController Controller => _agent.Controller;
        #endregion

        #region Abstract
        /// <returns> Input action that will Enable/Disable this ability </returns>
        [NotNull] protected abstract InputAction InputAction { get; }
        #endregion
        // @formatter:on

        #region Virtual Methods
        /// <summary> Callback for <see cref="InputAction"/>.started event </summary>
        protected virtual void OnInputStarted(InputAction.CallbackContext obj) => HandleKeyEvent(inputStartedHandling);

        /// <summary> Callback for <see cref="InputAction"/>.performed event </summary>
        protected virtual void OnInputPerformed(InputAction.CallbackContext obj) => HandleKeyEvent(inputPerformedHandling);

        /// <summary> Callback for <see cref="InputAction"/>.canceled event </summary>
        protected virtual void OnInputCanceled(InputAction.CallbackContext obj) => HandleKeyEvent(inputCanceledHandling);

        /// <summary> Check if the ability can be enabled (including check for <see cref="MonoBehaviour.enabled"/>) </summary>
        /// <returns> true if the ability can be enabled, false otherwise </returns>
        protected virtual bool CanEnable() => !enabled;

        /// <summary> Check if the ability can be disabled (including check for <see cref="MonoBehaviour.enabled"/></summary>
        /// <returns> true if the ability can be disabled, false otherwise </returns>
        protected virtual bool CanDisable() => enabled;
        #endregion

        #region Virtual Methods (Unity Functions)
        protected virtual void Awake()
        {
            InputAction action = InputAction;
            action.started += OnInputStarted;
            action.performed += OnInputPerformed;
            action.canceled += OnInputCanceled;
            InputAction.Enable();
            Agent.RegisterAbility(this);
        }

        protected virtual void OnEnable()
        {
            OnAbilityEnabled?.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            OnAbilityDisabled?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            InputAction action = InputAction;
            action.started -= OnInputStarted;
            action.performed -= OnInputPerformed;
            action.canceled -= OnInputCanceled;
            Agent.UnregisterAbility(this);
        }

        protected virtual void Reset()
        {
            _agent = GetComponentInParent<Agent>();
            if (_agent == null)
            {
                #if UNITY_EDITOR
                EditorUtility.DisplayDialog("Agent Not Found", "Ability must be added as a child of Agent.", "Okay");
                #else
                Debug.LogError("Ability must be added as a child of Agent.");
                #endif

                DestroyImmediate(this);
                return;
            }

            _agentAnimator = _agent.GetComponent<Animator>();
            enabled = false;

            foreach (var attribute in GetType().GetCustomAttributes(typeof(Attribute), true))
            {
                switch (attribute)
                {
                    case DefaultAbilityIndex abilityIndex:
                        _abilityIndex = abilityIndex.value;
                        break;
                    case DefaultInputHandling kdh:
                        inputStartedHandling = kdh.start;
                        inputPerformedHandling = kdh.performed;
                        inputCanceledHandling = kdh.canceled;
                        break;
                }
            }
        }
        #endregion

        #region Functionalities
        protected void HandleKeyEvent(InputHandling mode)
        {
            switch (mode)
            {
                case InputHandling.Disable:
                case InputHandling.Toggle when enabled:
                    if (CanDisable()) enabled = false;
                    break;
                case InputHandling.Enable:
                case InputHandling.Toggle: // when !enabled
                    if (CanEnable()) enabled = true;
                    break;
                case InputHandling.Ignore: break;
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
        #endregion
    }
}