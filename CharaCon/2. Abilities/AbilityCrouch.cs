using UnityEngine.InputSystem;

namespace Omnix.CharaCon.Abilities
{
    [DefaultAbilityIndex(-1)]
    [DefaultInputHandling(InputHandling.Enable, InputHandling.Ignore, InputHandling.Disable)]
    public class AbilityCrouch : BaseAbility
    {
        protected override InputAction InputAction => InputMap.BasicAbilities.Crouch;
    }
}