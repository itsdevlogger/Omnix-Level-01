using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnix.CharaCon
{
    [DefaultExecutionOrder(-101)]
    public class AgentInput : MonoBehaviour
    {
        public static event Action<bool> OnSetInputActive; 
        
        public static AgentInput Instance { get; private set; }
        public static PlayerInputMap InputMap { get; private set; }
        
        public static Vector2 Move {get; private set;}
        public static Vector2 MouseDelta { get; private set; }
        public static Vector2 MousePosition { get; private set; }

        private InputAction _moveInput;
        private InputAction _lookInput;
        private InputAction _lookDirectionInput;
        private InputAction _sprintInput;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Multiple agents listening to inputs. Make sure only one agent has enabled AgentInput component."); 
                return;
            }

            Instance = this;
            InputMap = new PlayerInputMap();
            InputMap.Movement.Enable();
            

            _moveInput = InputMap.Movement.Move;
            _moveInput.started += UpdateMoveSpeed;
            _moveInput.performed += UpdateMoveSpeed;
            _moveInput.canceled += UpdateMoveSpeed;
            _moveInput.Enable();

            _lookInput = InputMap.Movement.Look;
            _lookInput.started += UpdateLook;
            _lookInput.performed += UpdateLook;
            _lookInput.canceled += UpdateLook;
            _lookInput.Enable();

            _lookDirectionInput = InputMap.Movement.LookDirection;
            _lookDirectionInput.Enable();
        }
        
        private void OnEnable()
        {
            InputMap.Enable();
            OnSetInputActive?.Invoke(true);
        }

        private void OnDisable()
        {
            InputMap.Disable();
            OnSetInputActive?.Invoke(false);
        }
        
        private void UpdateLook(InputAction.CallbackContext obj)
        {
            MouseDelta = _lookInput.ReadValue<Vector2>();
            MousePosition = Instance._lookDirectionInput.ReadValue<Vector2>();
        }
        
        private void UpdateMoveSpeed(InputAction.CallbackContext _)
        {
            Move = _moveInput.ReadValue<Vector2>();
        }
    }
}