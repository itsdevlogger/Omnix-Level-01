using Omnix.CharaCon.HealthSystem;
using UnityEngine;

namespace Omnix.CharaCon
{
    [RequireComponent(typeof(Camera))]
    public class AgentCamera : MonoBehaviour
    {
        // @formatter:off
        public static Camera Current { get; private set; }
        private const float THRESHOLD = 0.01f;

        [Header("Cinemachine")] 
        [Tooltip(_.CINEMACHINE_CAMERA_TARGET)] public GameObject cinemachineCameraTarget;
        [Tooltip(_.TOP_CLAMP)]                 public float topClamp = 70.0f;
        [Tooltip(_.BOTTOM_CLAMP)]              public float bottomClamp = -30.0f;
        [Tooltip(_.CAMERA_ANGLE_OVERRIDE)]     public float cameraAngleOverride = 0.0f;
        [Tooltip(_.LOCK_CAMERA_POSITION)]      public bool lockCameraPosition = false;

        private bool IsCurrentDeviceMouse => true;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        // @formatter:on

        private void Awake()
        {
            if (Current != null)
            {
                Debug.LogError($"Multiple agent cameras in one scene. Using {Current.gameObject}", Current.gameObject);
                return;
            }

            Current = GetComponent<Camera>();
        }

        private void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (AgentInput.Look.sqrMagnitude >= THRESHOLD && !lockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += AgentInput.Look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += AgentInput.Look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}