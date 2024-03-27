using System;

namespace Omnix.CharaCon.Abilities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultAbilityIndex : Attribute
    {
        public int value;

        public DefaultAbilityIndex(int value)
        {
            this.value = value;
        }
    }
    
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultInputHandling : Attribute
    {
        public InputHandling start;
        public InputHandling performed;
        public InputHandling canceled;

        public DefaultInputHandling(InputHandling start, InputHandling performed, InputHandling canceled)
        {
            this.start = start;
            this.performed = performed;
            this.canceled = canceled;
        }
    }
}