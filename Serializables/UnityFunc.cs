using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Omnix.Serializables
{
    [Serializable]
    public struct UnityFunc<T> where T : new()
    {
        [SerializeField] private GameObject targetObject;
        [SerializeField] private string targetCompFunc;
        [SerializeField] private ArgumentCache defaultArgument;

        private MethodInfo methodInfo;
        private Component component;

        #if UNITY_EDITOR
        [SerializeField] private T ___EDITOR_ONLY_VARIABLE___;
        [SerializeField] private int currentIndex;
        #endif

        private bool Init()
        {
            try
            {
                string targetComponent, targetFunction;

                if (string.IsNullOrEmpty(targetCompFunc))
                    return false;


                int index = targetCompFunc.IndexOf("/");
                targetComponent = targetCompFunc.Substring(0, index);
                targetFunction = targetCompFunc.Substring(index + 1);
                targetFunction = targetFunction.Substring(0, targetFunction.LastIndexOf(" "));

                component = targetObject.GetComponent(targetComponent);
                methodInfo = component.GetType().GetMethod(targetFunction);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T Invoke()
        {
            TryInvoke(out T t);
            return t;
        }

        public bool TryInvoke(out T value)
        {
            if (methodInfo == null || component == null)
            {
                bool isSuccess = Init();
                if (isSuccess) value = (T)methodInfo.Invoke(component, defaultArgument.GetArray());
                else value = new T();
                return isSuccess;
            }

            value = (T)methodInfo.Invoke(component, defaultArgument.GetArray());
            return true;
        }
    }

    [Serializable]
    public class ArgumentCache
    {
        public int intArgument;
        public float floatArgument;
        public string stringArgument;
        public bool boolArgument;
        public Object objectArgument;
        public ArgumentReferanceType ReferanceType;

        public object[] GetArray()
        {
            if (ReferanceType == ArgumentReferanceType.Void) return null;
            if (ReferanceType == ArgumentReferanceType.Int) return new object[] { intArgument };
            if (ReferanceType == ArgumentReferanceType.Float) return new object[] { floatArgument };
            if (ReferanceType == ArgumentReferanceType.String) return new object[] { stringArgument };
            if (ReferanceType == ArgumentReferanceType.Bool) return new object[] { boolArgument };
            if (ReferanceType == ArgumentReferanceType.UnityObject) return new object[] { objectArgument };
            return null;
        }
    }

    public enum ArgumentReferanceType
    {
        Void = 0,
        Int = 1,
        Float = 2,
        String = 3,
        Bool = 4,
        UnityObject = 5
    }
}