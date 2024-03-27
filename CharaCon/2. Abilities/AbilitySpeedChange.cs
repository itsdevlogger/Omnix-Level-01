using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnix.CharaCon.Abilities
{
    [DefaultAbilityIndex(-3)]
    [DefaultInputHandling(InputHandling.Ignore, InputHandling.Toggle, InputHandling.Ignore)]
    public class AbilitySpeedChange : BaseAbility
    {
        [Space] 
        public float speedMultiplier = 1.5f;
        protected override InputAction InputAction => InputMap.Movement.Sprint;

        protected override void OnEnable()
        {
            base.OnEnable();
            Agent.moveSpeed *= speedMultiplier;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Agent.moveSpeed /= speedMultiplier;
        }
    }
}