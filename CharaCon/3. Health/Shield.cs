using System;
using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    [Serializable]
    public class Shield
    {
        public string name;
        
        [Tooltip("Health of the shield")]
        public float value;

        [Tooltip("Portion of dealt damage that the shield can absorb"), Range(0f, 1f)]
        public float absorptionRate;

        [Tooltip("Should this shield be restored on respawn")]
        public bool restoreOnRespawn;
        
        protected float startValue;


        /// <summary> Construct a shield with given values </summary>
        /// <param name="name">  </param>
        /// <param name="value"> Health of the shield </param>
        /// <param name="absorptionRate"> Portion of dealt damage that the shield can absorb (0 to 1)</param>
        /// <param name="restoreOnRespawn"> Should this shield be restored on respawn </param>
        public Shield(string name, float value, float absorptionRate, bool restoreOnRespawn)
        {
            this.name = name;
            this.startValue = value;
            this.value = value;
            this.absorptionRate = Mathf.Clamp01(absorptionRate);
            this.restoreOnRespawn = restoreOnRespawn;
        }

        /// <summary> Construct a copy of an existing shield </summary>
        /// <param name="original"> source </param>
        /// <param name="copyHealth"> if true, then new shield will be as depleted as the original </param>
        public Shield(Shield original, bool copyHealth = false)
        {
            this.name = original.name;
            this.startValue = original.startValue;
            this.absorptionRate = original.absorptionRate;
            this.restoreOnRespawn = original.restoreOnRespawn;
            this.value = copyHealth ? original.value : startValue;
        }

        /// <summary>
        /// Construct a shield with name: "New Shield", startValue: 10, value: 10, absorptionRate: 1, restoreOnRespawn: true
        /// </summary>
        public Shield()
        {
            this.name = "New Shield";
            this.startValue = 10f;
            this.value = 10f;
            this.absorptionRate = 1f;
            this.restoreOnRespawn = true;
        }

        /// <summary> Init this object </summary>
        public virtual void Init()
        {
            startValue = value;
        }

        /// <summary> Damage the shield </summary>
        /// <param name="damage"> Amount Damage </param>
        /// <returns> Amount of damage left un-dealt (This damage will be applied to next <see cref="Shield"/> or <see cref="DamageReceiver"/>) </returns>
        public virtual float Damage(float damage)
        {
            // If shield is already dead, then return
            if (value <= 0) return damage;

            // Calculate absorbed damage
            float absorbedDamage = damage * absorptionRate;
    
            // If shield value is greater than absorbed damage, deduct absorbed damage directly
            if (value > absorbedDamage)
            {
                value -= absorbedDamage;
            }
            
            // If absorbed damage exceeds shield value, set shield value to 0
            else
            {
                absorbedDamage = value;
                value = 0;
            }
    
            // Return unabsorbed damage
            return damage - absorbedDamage;
        }

        /// <summary>
        /// Restores health to starting value
        /// </summary>
        public virtual void Restore()
        {
            if (restoreOnRespawn)
            {
                value = startValue;
            }
        }
    }
}
