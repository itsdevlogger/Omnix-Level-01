using UnityEngine;
using UnityEditor;


namespace Omnix.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(RandomBetween<,>), true)]
    public class RandomBetweenDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty minimum = property.FindPropertyRelative("minimum");
            SerializedProperty maximum = property.FindPropertyRelative("maximum");
            SerializedProperty distribution = property.FindPropertyRelative("distribution");

            Rect rect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.PrefixLabel(rect, label);

            rect.x += rect.width;
            rect.width = ((position.width - rect.width - 20f - rect.height) / 2f);
            EditorGUI.PropertyField(rect, minimum, GUIContent.none);
            rect.x += rect.width + 10f;
            EditorGUI.PropertyField(rect, maximum, GUIContent.none);
            rect.x += rect.width + 10f;
            rect.width = rect.height;
            EditorGUI.PropertyField(rect, distribution, GUIContent.none);
            EditorGUI.EndProperty();
        }
    }


    [CustomPropertyDrawer(typeof(RandomRangeAttribute))]
    public class RandomBetweenRangeDrawer : PropertyDrawer
    {
        private float ONE_LINE = 18f;
        Rect rect;
        private float minVal;
        private float maxVal;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty min = property.FindPropertyRelative("minimum");
            SerializedProperty max = property.FindPropertyRelative("maximum");
            SerializedProperty distribution = property.FindPropertyRelative("distribution");

            if (min.propertyType == SerializedPropertyType.Float)
            {
                minVal = min.floatValue;
                maxVal = max.floatValue;
            }
            else if (min.propertyType == SerializedPropertyType.Integer)
            {
                minVal = min.intValue;
                maxVal = max.intValue;
            }
            else
            {
                EditorGUI.LabelField(position, $"Use \"RandomRangeAttribute\" on \"RandomBetweenFloat\" or \"RandomBetweenInt\" only.");
                return;
            }

            float ow = position.width - 85f - EditorGUIUtility.singleLineHeight;
            rect = new Rect(position.x, position.y, ow, ONE_LINE);
            rect.width -= 5f;

            EditorGUI.BeginChangeCheck();
            RandomRangeAttribute atter = (RandomRangeAttribute)attribute;
            EditorGUI.MinMaxSlider(rect, label, ref minVal, ref maxVal, atter.minimum, atter.maximum);

            rect.x += rect.width + 5f;
            rect.width = 80f;

            EditorGUI.LabelField(rect, $"({minVal:0.00}, {maxVal:0.00})");

            rect.x += rect.width + 5f;
            rect.width = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, distribution, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {

                if (min.propertyType == SerializedPropertyType.Float)
                {
                    min.floatValue = minVal;
                    max.floatValue = maxVal;
                }
                else if (min.propertyType == SerializedPropertyType.Integer)
                {
                    min.intValue = (int)minVal;
                    max.intValue = (int)maxVal;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}