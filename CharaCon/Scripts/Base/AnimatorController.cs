using System;
using System.Collections.Generic;
using Omnix.CharaCon.Abilities;
using UnityEngine;

namespace Omnix.CharaCon
{
    [RequireComponent(typeof(AnimatorController))]
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField] private Agent _agent;

        [Header("Parameters")] [SerializeField]
        private string _moveSpeedName;

        [SerializeField] private string _isGroundedName;
        [SerializeField] private string _isMovingName;
        [SerializeField] private string _abilityIndexName;

        private List<BaseAbility> _abilityHistory = new List<BaseAbility>();
        private ApInt _abilityIndex;
        private ApFloat _moveSpeed;
        private ApBool _isGrounded;
        private ApBool _isMoving;

        private void Start()
        {
            var animator = GetComponent<Animator>();
            _abilityIndex = new ApInt(_abilityIndexName, animator);
            _moveSpeed = new ApFloat(_moveSpeedName, animator);
            _isGrounded = new ApBool(_isGroundedName, animator);
            _isMoving = new ApBool(_isMovingName, animator);
        }

        private void OnEnable()
        {
            BaseAbility.OnAbilityEnabled += OnAbilityEnabled;
            BaseAbility.OnAbilityDisabled += OnAbilityDisabled;
        }

        private void OnDisable()
        {
            BaseAbility.OnAbilityEnabled -= OnAbilityEnabled;
            BaseAbility.OnAbilityDisabled -= OnAbilityDisabled;
        }

        private void LateUpdate()
        {
            if (_agent.IsGrounded != _isGrounded) _isGrounded.Set(_agent.IsGrounded);
            if (_agent.IsMoving != _isMoving) _isMoving.Set(_agent.IsMoving);
            if (Math.Abs(_agent.CurrentSpeed - _moveSpeed) > 0.001f) _moveSpeed.Set(_agent.CurrentSpeed);
        }

        private void Reset()
        {
            _agent = GetComponent<Agent>();
        }

        private void OnAbilityEnabled(BaseAbility ability)
        {
            _abilityIndex.Set(ability.AbilityIndex);
            _abilityHistory.RemoveAll(ab => ab == null || ab == ability);
            _abilityHistory.Add(ability);
        }

        private void OnAbilityDisabled(BaseAbility disabledAbility)
        {
            for (int i = _abilityHistory.Count - 1; i >= 0; i--)
            {
                BaseAbility ability = _abilityHistory[i];
                if (ability != null && ability != disabledAbility && ability.enabled)
                {
                    _abilityIndex.Set(ability.AbilityIndex);
                    return;
                }

                _abilityHistory.RemoveAt(i);
            }
            
            _abilityIndex.Set(0);
        }
    }
}