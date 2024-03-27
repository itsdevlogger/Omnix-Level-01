using UnityEngine;

namespace Omnix.CharaCon
{
    public abstract class AnimatorParameter<T>
    {
        protected T value;
        public abstract void Set(T newValue);

        protected readonly int hash;
        protected readonly bool hasAnimator;
        protected readonly Animator animator;

        protected AnimatorParameter(string parameterName, T value, Animator animator)
        {
            this.value = value;
            this.animator = animator;
            
            hash = Animator.StringToHash(parameterName);
            hasAnimator = this.animator != null;
        }

        public static implicit operator T(AnimatorParameter<T> parameter) => parameter.value;
    }
    
    public class ApBool : AnimatorParameter<bool> 
    {
        public ApBool(string parameterName, Animator animator) : base(parameterName, false, animator) { }
        public ApBool(string parameterName, bool value, Animator animator) : base(parameterName, value, animator) { }
        public override void Set(bool newValue)
        {
            value = newValue;
            if (hasAnimator) animator.SetBool(hash, newValue);
        }
    }

    public class ApFloat : AnimatorParameter<float> 
    {
        public ApFloat(string parameterName, Animator animator) : base(parameterName, 0f, animator) { }
        public ApFloat(string parameterName, float value, Animator animator) : base(parameterName, value, animator) { }
        public override void Set(float newValue)
        {
            value = newValue;
            if (hasAnimator) animator.SetFloat(hash, newValue);
        }
    }
    
    public class ApInt : AnimatorParameter<int> 
    {
        public ApInt(string parameterName, Animator animator) : base(parameterName, 0, animator) { }
        public ApInt(string parameterName, int value, Animator animator) : base(parameterName, value, animator) { }
        public override void Set(int newValue)
        {
            value = newValue;
            if (hasAnimator) animator.SetInteger(hash, newValue);
        }
    }
}