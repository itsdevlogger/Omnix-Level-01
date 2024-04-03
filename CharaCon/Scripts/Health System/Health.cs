using System;
using System.Collections;
using System.Collections.Generic;
using MenuManagement.Base;
using Omnix.CharaCon.Utils;
using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    [GroupProperties("Settings", nameof(_isInvincible), nameof(startValue), nameof(damageMultiplier), nameof(shields))]
    [GroupProperties("Auto Respawn", nameof(autoRespawnDelay), nameof(healShieldsOnRespawn))]
    [GroupProperties("References", nameof(damageCollider))]
    [GroupProperties("Connected Objects", nameof(invincibleHandling), nameof(respawnHandling), nameof(deathHandling))]
    public class Health : MonoBehaviour, IDamageable, IEnumerable<Shield>
    {
        #region Events
        /// <param name="damageApplied"> Damage dealt by IDamageDealer object </param>
        /// <param name="healthDeducted"> Health deducted after some damage is absorbed by shields. </param>
        public delegate void DamageDelegate(DamageInfo damageApplied, float healthDeducted);

        public event DamageDelegate OnDamaged;
        public event Action OnDeath;
        public event Action OnSpawnOrRespawned;
        #endregion

        #region Fields
        // @formatter:off
        // Settings 
        [       SerializeField] private bool _isInvincible;
        [field: SerializeField] protected float startValue = 100f;
        [field: SerializeField] protected float damageMultiplier = 1f;
        [       SerializeField] protected List<Shield> shields = new List<Shield>();

        // Auto Respawn 
        [Tooltip("0 or -ve means no respawning")] 
        [SerializeField] public float autoRespawnDelay = -1;
        [SerializeField] public bool healShieldsOnRespawn = true;

        // References 
        [field: SerializeField, Tooltip("[Con Be Null] Collider that will be receiving damage for this health component")] 
        protected Collider damageCollider;

        // Connected Objects 
        [SerializeField] public ConnectedObjects invincibleHandling;
        [SerializeField] public ConnectedObjects respawnHandling;
        [SerializeField] public ConnectedObjects deathHandling;

        // Non Serialized
        private bool _hasInitialize = false;
        private float _value;
        public bool IsDead => !IsAlive;
        public float StartValue => startValue;
        public float DamageMultiplier => damageMultiplier;
        public Collider Collider => damageCollider;
        // @formatter:on

        public float Value
        {
            get => _hasInitialize ? _value : startValue;
            protected set => _value = value;
        }

        public bool IsInvincible
        {
            get => _isInvincible;
            set
            {
                _isInvincible = value;
                invincibleHandling.Set(value);
            }
        }
        #endregion

        #region Access Shields
        // @formatter:off
        public void Add(Shield shield)                     => shields.InitAndAdd(shield);           // Reason why we can make shields public
        public void AddAll(IEnumerable<Shield> collection) => shields.InitAndAdd(collection);       // Reason why we can make shields public
        public int Count                                   => shields.Count;
        public Shield this[int index]                      => shields[index];
        public void Remove(Shield shield)                  => shields.Remove(shield);
        public void Remove(int index)                      => shields.RemoveAt(index);
        public void RemoveAll(Predicate<Shield> predicate) => shields.RemoveAll(predicate);
        public void RemoveAll()                            => shields.Clear();
        public bool Exists(Predicate<Shield> predicate)    => shields.Exists(predicate);
        public bool Contains(Shield shield)                => shields.Contains(shield);
        public Shield Find(Predicate<Shield> predicate)    => shields.Find(predicate);
        public IEnumerator<Shield> GetEnumerator()         => shields.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()            => shields.GetEnumerator();
        // @formatter:on
        #endregion

        #region Virtuals
        public virtual bool IsAlive
        {
            get { return Value > 0; }
        }

        protected virtual void Awake()
        {
            Value = startValue;
            invincibleHandling.Set(_isInvincible);
            shields.Init();
            if (Value <= 0) Die();
            else deathHandling.Set(false);
            _hasInitialize = true;
        }

        protected virtual void Start()
        {
            OnSpawnOrRespawned?.Invoke();
        }

        protected virtual void Reset()
        {
            damageCollider = GetComponent<Collider>();
        }

        /// <summary> Kill this object this instance regardless of its health or shields it has </summary>
        public virtual void Die()
        {
            Value = 0f;
            deathHandling.Set(true);
            OnDeath?.Invoke();

            if (autoRespawnDelay > 0) Invoke(nameof(Respawn), autoRespawnDelay);
        }

        public virtual void Respawn()
        {
            Respawn(healShieldsOnRespawn);
        }

        public virtual void Respawn(bool healShields)
        {
            Value = startValue;
            if (healShields) shields.Restore();
            respawnHandling.Set(true);
            OnSpawnOrRespawned?.Invoke();
        }

        public virtual void TakeDamage(float amount, object dealer)
        {
            TakeDamage(new DamageInfo(amount, dealer, damageCollider, this, Vector3.zero, Vector3.zero));
        }

        public virtual void TakeDamage(float amount, object dealer, Vector3 position)
        {
            TakeDamage(new DamageInfo(amount, dealer, damageCollider, this, position, Vector3.zero));
        }

        public virtual void TakeDamage(float amount, object dealer, Vector3 position, Vector3 force)
        {
            TakeDamage(new DamageInfo(amount, dealer, damageCollider, this, position, force));
        }

        public virtual void TakeDamage(DamageInfo info)
        {
            if (_isInvincible || IsDead) return;

            float damageLeft = shields.Damage(info.amount);
            Value = Mathf.Max(0f, Value - damageLeft);

            OnDamaged?.Invoke(info, damageLeft);
            if (Value <= 0) Die();
        }
        #endregion
    }
}