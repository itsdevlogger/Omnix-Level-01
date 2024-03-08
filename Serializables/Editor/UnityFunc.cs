using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using Omnix.Extensions;
using Omnix.Utils.EditorUtils;
using UnityEditor;

namespace Omnix.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(UnityFunc<>))]
    public class UnityFuncDrawer : PropertyDrawer
    {
        private interface IMethodArgInfo
        {
            public void Draw(Rect rect, SerializedProperty defaultArgument, SerializedProperty referanceType);
            public void SetDefault(SerializedProperty property);
        }

        private struct MethodArgsInfo<T> : IMethodArgInfo
        {
            public T currentValue;
            public Func<Rect, string, T, T> DrawerAction;
            public Action<SerializedProperty, T> ValueSetterAction;
            public string label;
            public int enumReferanceValue;

            public MethodArgsInfo(int enumReferanceValue, string label, T currentValue, Func<Rect, string, T, T> drawerAction, Action<SerializedProperty, T> valueSetterAction)
            {
                this.enumReferanceValue = enumReferanceValue;
                this.label = label;
                this.currentValue = currentValue;
                DrawerAction = drawerAction;
                ValueSetterAction = valueSetterAction;
            }

            public void Draw(Rect rect, SerializedProperty defaultArgument, SerializedProperty referanceType)
            {
                EditorGUI.BeginChangeCheck();
                currentValue = DrawerAction.Invoke(rect, label, currentValue);
                if (EditorGUI.EndChangeCheck()) ValueSetterAction.Invoke(defaultArgument, currentValue);
                referanceType.enumValueIndex = enumReferanceValue;
            }

            public void SetDefault(SerializedProperty property)
            {
                ValueSetterAction.Invoke(property, currentValue);
            }
        }

        private static class ValueSetters
        {
            public static void Set_int(SerializedProperty property, int value)
            {
                if (property == null)
                {
                    return;
                }

                property.FindPropertyChild("intArgument").intValue = value;
            }

            public static void Set_float(SerializedProperty property, float value)
            {
                if (property == null)
                {
                    return;
                }

                property.FindPropertyChild("floatArgument").floatValue = value;
            }

            public static void Set_string(SerializedProperty property, string value)
            {
                if (property == null)
                {
                    return;
                }

                property.FindPropertyChild("stringArgument").stringValue = value;
            }

            public static void Set_bool(SerializedProperty property, bool value)
            {
                if (property == null)
                {
                    return;
                }

                property.FindPropertyChild("boolArgument").boolValue = value;
            }

            public static void Set_Object(SerializedProperty property, Object value)
            {
                if (property == null)
                {
                    return;
                }

                property.FindPropertyChild("objectArgument").objectReferenceValue = value;
            }
        }


        private static float LINE_HEIGHT => 16f;
        private static float LINE_SPACE => 4f;

        SerializedProperty targetObject;
        SerializedProperty targetCompFunc;
        SerializedProperty genericTypeProperty;
        SerializedProperty defaultArgument;
        SerializedProperty referanceType;
        SerializedProperty currentIndex;
        private string[] funcOptions;
        private Dictionary<int, IMethodArgInfo> invokeWithArgOptions;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            referanceType = property.FindPropertyChild("ReferanceType");

            if (property.isExpanded)
            {
                if (referanceType != null && referanceType.enumValueIndex != 0) return LINE_HEIGHT * 4 + LINE_SPACE * 3;
                return LINE_HEIGHT * 3 + LINE_SPACE * 2;
            }
            return LINE_HEIGHT;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            
            
            
            Rect foldoutRect = new Rect(position.x, position.y, position.width, LINE_HEIGHT);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
            if (!property.isExpanded)
            {
                return;
            }

            targetObject = property.FindPropertyRelative("targetObject");
            targetCompFunc = property.FindPropertyRelative("targetCompFunc");
            defaultArgument = property.FindPropertyRelative("defaultArgument");
            currentIndex = property.FindPropertyRelative("currentIndex");
            genericTypeProperty = property.FindPropertyRelative("___EDITOR_ONLY_VARIABLE___");
            
            
            Rect ofRect = new Rect(position.x + 10f, position.y + LINE_HEIGHT + LINE_SPACE, position.width - 10f, LINE_HEIGHT);
            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(ofRect, targetObject);
            UpdateFuncOptions();

            if (currentIndex.intValue < 0)
            {
                currentIndex.intValue = Array.IndexOf(funcOptions, targetCompFunc.stringValue);
            }

            ofRect.y += LINE_HEIGHT + LINE_SPACE;
            EditorGUI.BeginChangeCheck();
            currentIndex.intValue = EditorGUI.Popup(ofRect, "Method", currentIndex.intValue, funcOptions);
            if (EditorGUI.EndChangeCheck() || funcOptions == null) UpdateMethodName();
            if (currentIndex.intValue >= 0 && invokeWithArgOptions != null && invokeWithArgOptions.ContainsKey(currentIndex.intValue))
            {
                ofRect.y += LINE_HEIGHT + LINE_SPACE;
                ofRect.x += LINE_HEIGHT;
                ofRect.width -= LINE_HEIGHT;
                invokeWithArgOptions[currentIndex.intValue].Draw(ofRect, defaultArgument, referanceType);
            }
            else
            {
                referanceType.enumValueIndex = 0;
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private void UpdateMethodName()
        {
            if (currentIndex.intValue > 0)
            {
                targetCompFunc.stringValue = funcOptions[currentIndex.intValue];
            }
        }

        private void UpdateFuncOptions()
        {
            if (targetObject.objectReferenceValue == null)
            {
                funcOptions = new string[0];
                return;
            }

            List<string> supportedMethods = new List<string>();
            if (invokeWithArgOptions == null) invokeWithArgOptions = new Dictionary<int, IMethodArgInfo>();
            else invokeWithArgOptions.Clear();

            Type propertyTypeChecker = null;
            try
            {
                propertyTypeChecker = genericTypeProperty.GetProperType();
            }
            catch
            {
            }

            if (propertyTypeChecker == null || !propertyTypeChecker.IsSerializable)
            {
                Debug.LogError("Type not supported for UnityFunc must be Serializable");
                return;
            }


            Type exclude1 = typeof(System.Object);
            Type exclude2 = typeof(UnityEngine.Component);
            Type exclude3 = typeof(UnityEngine.Object);
            Type exclude4 = typeof(UnityEngine.MonoBehaviour);
            Type exclude5 = typeof(UnityEngine.Behaviour);

            foreach (Component component in ((GameObject)targetObject.objectReferenceValue).GetComponents<Component>())
            {
                foreach (Type type in component.SelfAndBaseTypes())
                {
                    if (type == exclude1 || type == exclude2 || type == exclude3 || type == exclude4 || type == exclude5) continue;
                    foreach (MethodInfo methodInfo in type.GetMethods())
                    {
                        if (propertyTypeChecker.IsAssignableFrom(methodInfo.ReturnType))
                        {
                            ParameterInfo[] parameterInfo = methodInfo.GetParameters();
                            int paramCount = parameterInfo.Length;
                            if (paramCount == 0)
                            {
                                supportedMethods.Add($"{type}/{methodInfo.Name} ()");
                            }
                            else if (paramCount == 1)
                            {
                                (string, IMethodArgInfo) methodArgInfo = GetMethodArgInfo(parameterInfo[0]);
                                if (methodArgInfo.Item2 != null)
                                {
                                    supportedMethods.Add($"{type}/{methodInfo.Name} ({methodArgInfo.Item1})");
                                    invokeWithArgOptions.Add(supportedMethods.Count - 1, methodArgInfo.Item2);
                                    // methodArgInfo.Item2.SetDefault(defaultArgument);
                                }
                            }
                        }
                    }
                }
            }

            funcOptions = supportedMethods.ToArray();
        }

        private (string, IMethodArgInfo) GetMethodArgInfo(ParameterInfo parameterInfo)
        {
            Type paraType = parameterInfo.ParameterType;
            string label = parameterInfo.Name;

            // @formatter:off
            if (paraType == typeof(int)) return ("int", new MethodArgsInfo<int>(enumReferanceValue: 1, label: label, currentValue: defaultArgument.FindPropertyChild("intArgument").intValue, drawerAction: EditorGUI.IntField, valueSetterAction: ValueSetters.Set_int));
            if (paraType == typeof(float)) return ("float", new MethodArgsInfo<float>(enumReferanceValue: 2, label: label, currentValue: defaultArgument.FindPropertyChild("floatArgument").floatValue, drawerAction: EditorGUI.FloatField, valueSetterAction: ValueSetters.Set_float));
            if (paraType == typeof(string)) return ("string", new MethodArgsInfo<string>(enumReferanceValue: 3, label: label, currentValue: defaultArgument.FindPropertyChild("stringArgument").stringValue, drawerAction: EditorGUI.TextField, valueSetterAction: ValueSetters.Set_string));
            if (paraType == typeof(bool)) return ("bool", new MethodArgsInfo<bool>(enumReferanceValue: 4, label: label, currentValue: defaultArgument.FindPropertyChild("boolArgument").boolValue, drawerAction: EditorGUI.Toggle, valueSetterAction: ValueSetters.Set_bool));
            if (typeof(Object).IsAssignableFrom(paraType)) return ("Object", new MethodArgsInfo<Object>(enumReferanceValue: 5, label: label, currentValue: defaultArgument.FindPropertyChild("objectArgument").objectReferenceValue, drawerAction: ((rect, str, refer) => EditorGUI.ObjectField(rect, str, refer, paraType, true)), valueSetterAction: ValueSetters.Set_Object));
            // @formatter:on

            return ("Not Supported", null);
        }
    }
}