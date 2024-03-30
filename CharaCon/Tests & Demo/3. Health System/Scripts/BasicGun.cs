using Omnix.CharaCon.HealthSystem;
using UnityEngine;

namespace Omnix.CharaCon.HealthTest
{
    public class BasicGun : MonoBehaviour
    {
        [SerializeField] private LayerMask _damageRecieverLayer;
        [SerializeField] private float _damagePerShot;
        [SerializeField] private float _forcePerShot;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = AgentCamera.Current.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _damageRecieverLayer, QueryTriggerInteraction.Ignore))
                {
                    // Do not use hit.transform as it returns transform of object with rigidbody
                    // Creates problem when parent has rigidbody, and the collider doesn't
                    if (hit.collider.TryGetComponent(out IDamageable receiver))
                    {
                        receiver.TakeDamage(_damagePerShot, this, hit.point, ray.direction * _forcePerShot);
                    }
                }
            }
        }
    }
}