using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Omnix.Serializables
{
    public abstract class RandomBetween<t, TRet>
    {
        [SerializeField] protected t minimum;
        [SerializeField] protected t maximum;
        [SerializeField] protected AnimationCurve distribution;

        protected RandomBetween(t minimum, t maximum, AnimationCurve distribution)
        {
            this.minimum = minimum;
            this.maximum = maximum;
            this.distribution = distribution;
        }

        protected RandomBetween(t minimum, t maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
            this.distribution = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }

        protected RandomBetween() { }

        public TRet value => Lerp(distribution.Evaluate(Random.value));
        protected abstract TRet Lerp(float t);
        public static implicit operator TRet(RandomBetween<t, TRet> randomBetween) => randomBetween.value;
    }


    [Serializable]
    public class RandomBetweenInt : RandomBetween<int, int>
    {
        protected override int Lerp(float t) => (int)(t * maximum + (1 - t) * minimum);
    }


    [Serializable]
    public class RandomBetweenFloat : RandomBetween<float, float>
    {
        protected override float Lerp(float t) => (t * maximum + (1 - t) * minimum);
    }


    [Serializable]
    public class RandomBetweenV2 : RandomBetween<Vector2, Vector2>
    {
        protected override Vector2 Lerp(float t) => Vector2.Lerp(minimum, maximum, t);
    }


    [Serializable]
    public class RandomBetweenV3 : RandomBetween<Vector3, Vector3>
    {
        protected override Vector3 Lerp(float t) => Vector3.Lerp(minimum, maximum, t);
    }


    [Serializable]
    public class RandomBetweenColor : RandomBetween<Color, Color>
    {
        protected override Color Lerp(float t) => Color.Lerp(minimum, minimum, t);
    }


    [Serializable]
    public class RandomBetweenPosition : RandomBetween<Transform, Vector3>
    {
        [SerializeField] bool useLocal;
        protected override Vector3 Lerp(float t)
        {
            if (useLocal) return Vector3.Lerp(minimum.localPosition, maximum.localPosition, t);
            return Vector3.Lerp(minimum.position, maximum.position, t); ;
        }
    }


    [Serializable]
    public class RandomBetweenRotation : RandomBetween<Transform, Quaternion>
    {
        [SerializeField] bool useLocal;
        protected override Quaternion Lerp(float t)
        {
            if (useLocal) return Quaternion.Lerp(minimum.localRotation, maximum.localRotation, t);
            return Quaternion.Lerp(minimum.rotation, maximum.rotation, t);
        }
    }


    [Serializable]
    public class RandomBetweenScale : RandomBetween<Transform, Vector3>
    {
        protected override Vector3 Lerp(float t) => Vector3.Lerp(minimum.localScale, maximum.localScale, t);
    }



    [AttributeUsage(AttributeTargets.Field)]
    public class RandomRangeAttribute : PropertyAttribute
    {
        public float minimum;
        public float maximum;

        public RandomRangeAttribute(float minimum, float maximum)
        {
            this.minimum = minimum;
            this.maximum = maximum;
        }
    }
}


