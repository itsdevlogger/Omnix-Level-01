using System;
using System.Collections;
using System.Collections.Generic;
using Omnix.CharaCon.Collections;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Omnix.CharaCon.HealthSystem
{
    public class Health : MonoBehaviour, IEnumerable<Shield>
    {
        /// <param name="damageApplied"> Damage dealt by IDamageDealer object </param>
        /// <param name="healthDeducted"> Health deducted after some damage is absorbed by shields. </param>
        public delegate void DamageDelegate(DamageInfo damageApplied, float healthDeducted);
        
        public event DamageDelegate OnDamaged;
        public event Action OnDeath;
        public event Action OnSpawnOrRespawned;

        [Header("Settings")]
        [SerializeField] private bool _isInvincible;
        [field: SerializeField] public float StartValue { get; protected set; } = 100f;
        [SerializeField] protected Rigidbody rigidBody;
        [SerializeField] protected List<Shield> shields = new List<Shield>();
        
        [Space, Header("Auto Respawn")]
        [Tooltip("0 or -ve means no respawning")]
        public float autoRespawnDelay = -1;
        public bool healShieldsOnRespawn = true;
        
        
        [Space, Header("Connected Objects")]
        public ConnectedObjects invincibleHandling;
        public ConnectedObjects respawnHandling;
        public ConnectedObjects deathHandling;

        private bool _hasInitialize = false;
        private float _value;

        public float Value
        {
            get => _hasInitialize ? _value : StartValue;
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

        
        // @formatter:off
        public virtual bool IsAlive                        => Value > 0;
        public bool IsDead                                 => !IsAlive;
        public int Count                                   => shields.Count;
        public Shield this[int index]                      => shields[index];
        public void Add(Shield shield)                     => shields.InitAndAdd(shield);
        public void AddAll(IEnumerable<Shield> collection) => shields.InitAndAdd(collection);
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

        protected virtual void Awake()
        {
            Value = StartValue;
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

        public virtual void TakeDamage(DamageInfo info)
        {
            if (_isInvincible || IsDead) return;
            
            float damageLeft = shields.Damage(info.amount);
            Value = Mathf.Max(0f, Value - damageLeft);
            if (rigidBody != null) rigidBody.AddForceAtPosition(info.force, info.position);
            
            OnDamaged?.Invoke(info, damageLeft);
            if (Value <= 0) Die();
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
            Value = StartValue;
            if (healShields) shields.Restore();
            respawnHandling.Set(true);
            OnSpawnOrRespawned?.Invoke();
        }
    }
}