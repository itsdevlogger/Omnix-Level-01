using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Omnix.CharaCon.Abilities;
using Omnix.CharaCon.HealthSystem;
using UnityEngine;

namespace Omnix.CharaCon
{
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(CharacterController))]
    public class Agent : MonoBehaviour
    {
        public delegate void AgentSwitchDelegate([CanBeNull] Agent oldAgent, [CanBeNull] Agent newAgent);

        /// <summary> Called when the player switches Character </summary>
        public static event AgentSwitchDelegate OnSwitched;

        [Obsolete("Use Current (Without underscores) instead")]
        private static Agent __current__;
        public static Agent Current
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            get => __current__;
            private set
            {
                Agent old = __current__;
                __current__ = value;
                OnSwitched?.Invoke(old, value);
            }
            #pragma warning restore CS0618 // Type or member is obsolete
        }
        
        
        // @formatter:off
        [Header("Settings")]
        [Tooltip(_.MOVE_SPEED)]                               public float moveSpeed = 2.0f;
        [Tooltip(_.MASS)]                                     public float mass = -15.0f;
        [Tooltip(_.GRAVITY)]                                  public float gravity = -15.0f;
        [Tooltip(_.SPEED_CHANGE_RATE)]                        public float speedChangeRate = 10.0f;
        [Tooltip(_.ROTATION_SMOOTH_TIME)] [Range(0f, 0.3f)]   public float rotationSmoothTime = 0.12f;

        [Space, Header("Settings")]
        [Tooltip(_.GROUND_LAYERS)]                            public LayerMask groundLayers;
        [Tooltip(_.GROUNDED_OFFSET)]                          public float groundOffset = -0.14f;
        [Tooltip(_.GROUNDED_RADIUS)]                          public float groundCheckRadius = 0.28f;
        
        [Space, Header("References")]
        [Tooltip(_.HEALTH), SerializeField, CanBeNull]        private Health _health;

        public bool IsMoving      { get; private set; } = false;
        public bool IsGrounded    { get; private set; } = true;
        public float CurrentSpeed { get; private set; } = 0f;
        // @formatter:on

        private CharacterController _controller;
        private Vector3 _currentVelocity = Vector3.zero;
        private Dictionary<Type, BaseAbility> _allAbilities = new Dictionary<Type, BaseAbility>();

        // intermediates (Used for calculations) 
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _terminalVelocity = 53.0f;

        public CharacterController Controller => _controller;
        public PlayerInputMap InputMap => AgentInput.Instance.InputMap;
        [CanBeNull] public Health Health => _health;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            Current = this;
        }

        private void Update()
        {
            Gravity();
            GroundedCheck();
            Move();
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (IsGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Vector3 position = transform.position;
            position.y -= groundOffset;
            Gizmos.DrawSphere(position, groundCheckRadius);
        }

        private void Reset()
        {
            _health = GetComponent<Health>();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 position = transform.position;
            position.y -= groundOffset;
            IsGrounded = Physics.CheckSphere(position, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Move()
        {
            float targetSpeed;
            if (AgentInput.Instance.Move == Vector2.zero)
            {
                IsMoving = false;
                targetSpeed = 0.0f;
            }
            else
            {
                IsMoving = true;
                targetSpeed = moveSpeed;
            }

            // a reference to the players current horizontal velocity
            Vector3 controllerVelocity = _controller.velocity;
            float currentHorizontalSpeed = new Vector3(controllerVelocity.x, 0.0f, controllerVelocity.z).magnitude;
            float speedOffset = 0.1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                CurrentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
                CurrentSpeed = Mathf.Round(CurrentSpeed * 1000f) / 1000f;
            }
            else
            {
                CurrentSpeed = targetSpeed;
            }

            // if there is a move input rotate player when the player is moving
            if (AgentInput.Instance.Move != Vector2.zero)
            {
                Vector3 inputDirection = new Vector3(AgentInput.Instance.Move.x, 0.0f, AgentInput.Instance.Move.y).normalized;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + AgentCamera.Current.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            targetDirection.Normalize();
            targetDirection *= CurrentSpeed;
            _controller.Move((targetDirection + _currentVelocity) * Time.deltaTime);
        }

        private void Gravity()
        {
            if (IsGrounded)
            {
                if (_currentVelocity.y < 0.0f)
                {
                    _currentVelocity.y = -2f;
                }
            }
            else if (_currentVelocity.y < _terminalVelocity)
            {
                _currentVelocity.y += gravity * Time.deltaTime;
            }
        }


        /// <summary> Add force </summary>
        public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            switch (forceMode)
            {
                case ForceMode.Force:
                    _currentVelocity += force * (Time.deltaTime / mass);
                    break;
                case ForceMode.Acceleration:
                    _currentVelocity += force * Time.deltaTime;
                    break;
                case ForceMode.Impulse:
                    _currentVelocity += force / mass;
                    break;
                case ForceMode.VelocityChange:
                    _currentVelocity += force;
                    break;
            }
        }

        /// <summary> Register an ability with this agent </summary>
        public void RegisterAbility(BaseAbility ability)
        {
            Type type = ability.GetType();
            if (_allAbilities.TryGetValue(type, out BaseAbility existingAbility) && existingAbility != null)
            {
                Debug.LogError($"Multiple ability of same type: {type.Name}");
            }
            else
            {
                _allAbilities.Add(type, ability);
            }
        }

        /// <summary> Unregister an ability with this agent </summary>
        public void UnregisterAbility(BaseAbility ability)
        {
            _allAbilities.Remove(ability.GetType());
        }

        /// <summary> Tries to get ability of specified type. </summary>
        /// <returns> type if ability is not null. </returns>
        public bool TryGetAbility<T>(out T ability) where T : BaseAbility
        {
            if (_allAbilities.TryGetValue(typeof(T), out BaseAbility existing))
            {
                ability = existing as T;
                return ability != null;
            }

            ability = null;
            return false;
        }

        /// <summary> Tries to get ability of specified type. </summary>
        /// <returns> type if ability is not null. </returns>
        public bool TryGetAbility(Type type, out BaseAbility ability)
        {
            return _allAbilities.TryGetValue(type, out ability) && ability != null;
        }

        /// <summary> Get ability of specified type. </summary>
        /// <returns> Ability, can be null </returns>
        public T GetAbility<T>() where T : BaseAbility
        {
            if (_allAbilities.TryGetValue(typeof(T), out BaseAbility existing))
            {
                return existing as T;
            }

            return null;
        }

        /// <summary> Get ability of specified type. </summary>
        /// <returns> Ability, can be null </returns>
        public BaseAbility GetAbility(Type type)
        {
            if (_allAbilities.TryGetValue(type, out BaseAbility existing))
            {
                return existing;
            }

            return null;
        }
    }
}