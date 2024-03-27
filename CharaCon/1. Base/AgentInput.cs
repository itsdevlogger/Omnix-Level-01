using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnix.CharaCon
{
    [DefaultExecutionOrder(-101)]
    public class AgentInput : MonoBehaviour
    {
        public static AgentInput Instance { get; private set; }
        
        public Vector2 Move {get; private set;}
        public Vector2 Look { get; private set; }
        public PlayerInputMap InputMap { get; private set; }

        private InputAction _moveInput;
        private InputAction _lookInput;
        private InputAction _sprintInput;


        private void OnEnable() => InputMap.Enable();
        private void OnDisable() => InputMap.Disable();

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
        }
        
        private void UpdateMoveSpeed(InputAction.CallbackContext _)
        {
            Move = _moveInput.ReadValue<Vector2>();
        }

        private void UpdateLook(InputAction.CallbackContext obj)
        {
            Look = _lookInput.ReadValue<Vector2>();
        }
    }
}