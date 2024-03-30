using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    [RequireComponent(typeof(Collider))]
    public class Damageable : MonoBehaviour, IDamageable
    {
        [field: SerializeField] public float DamageMultiplier { get; private set; } = 1f;
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public Collider Collider { get; private set; }

        protected virtual void Reset()
        {
            DamageMultiplier = 1f;
            Health = GetComponentInParent<Health>(true);
            Collider = GetComponent<Collider>();
            if (Health == null)
            {
                Debug.LogError($"DamageReceiver [{gameObject.name}] has no Health component in parent.");
            }
        }

        public void TakeDamage(float amount, object dealer) => TakeDamage(amount, dealer, Vector3.zero, Vector3.zero);
        public void TakeDamage(float amount, object dealer, Vector3 position) => TakeDamage(amount, dealer, position, Vector3.zero);
        public void TakeDamage(float amount, object dealer, Vector3 position, Vector3 force)
        {
            if (Health == null)
            {
                Debug.LogError("Cannot apply damage, as there is no Health component in parent", gameObject);
            }
            else
            {
                Health.TakeDamage(new DamageInfo(amount, dealer, Collider, this, position, force));
            }
        }
    }
}