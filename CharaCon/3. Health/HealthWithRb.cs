using MenuManagement.Base;
using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    [GroupProperties("References", nameof(_rigidBody))]
    public class HealthWithRb : Health
    {
        [SerializeField] private Rigidbody _rigidBody;

        protected override void Reset()
        {
            base.Reset();
            _rigidBody = GetComponent<Rigidbody>();
        }

        public override void TakeDamage(DamageInfo info)
        {
            base.TakeDamage(info);
            if (_rigidBody != null) _rigidBody.AddForceAtPosition(info.force, info.position);
        }
    }
}