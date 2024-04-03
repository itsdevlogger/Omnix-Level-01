using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    public interface IDamageable
    {
        public float DamageMultiplier { get; }
        void TakeDamage(float amount, object dealer);
        void TakeDamage(float amount, object dealer, Vector3 position);
        void TakeDamage(float amount, object dealer, Vector3 position, Vector3 force);
    }
}