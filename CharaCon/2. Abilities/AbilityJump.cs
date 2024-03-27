using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnix.CharaCon.Abilities
{
    [DefaultAbilityIndex(-2)]
    [DefaultInputHandling(InputHandling.Enable, InputHandling.Ignore, InputHandling.Ignore)]
    public class AbilityJump : BaseAbility
    {
        [Space] 
        public ForceMode forceMode;
        public float force = 50f;
        public float cooldownTime = 0.1f;
        
        protected override InputAction InputAction => InputMap.BasicAbilities.Jump;
        protected override bool CanEnable() => base.CanEnable() && Agent.IsGrounded;

        protected override void OnEnable()
        {
            base.OnEnable();
            Agent.AddForce(Vector3.up * force, forceMode);
            StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(cooldownTime);
            enabled = false;
        }
    }
}