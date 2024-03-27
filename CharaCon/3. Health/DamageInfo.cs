using System;
using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    [Serializable]
    public readonly struct DamageInfo
    {
        /// <summery> Damage dealer. </summery>
        public readonly IDamageDealer dealer;

        /// <summery> Location (in this object) where damage should be dealt. </summery>
        public readonly Vector3 position;

        /// <summery> Direction of impact </summery>
        public readonly Vector3 direction;

        /// <summery> Force of impact </summery>
        public readonly Vector3 force;

        /// <summery> Collider (Child of this object) which received the damage </summery>
        public readonly Collider hitCollider;

        /// <summery> HitBox which received the damage </summery>
        public readonly DamageReceiver receiver;

        /// <summery> How much damage to add according to dealer. This does not account for DamageMultiplier of the hitBox. </summery>
        public readonly float rawAmount;

        /// <summary> How much damage to actually deal. This is damage after multiplying the rawAmount with the DamageMultiplier of the hitBox. </summary>
        public readonly float amount;

        public DamageInfo(float rawAmount, IDamageDealer dealer, Collider hitCollider, DamageReceiver receiver, Vector3 position, Vector3 force)
        {
            this.rawAmount = rawAmount;
            this.dealer = dealer;
            this.position = position;
            this.force = force;
            this.direction = force.normalized;
            this.hitCollider = hitCollider;
            this.receiver = receiver;
            this.amount = this.receiver == null ? rawAmount : rawAmount * this.receiver.DamageMultiplier;
        }
    }
}