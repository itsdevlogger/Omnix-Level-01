using UnityEngine;
using UnityEditor;
using Omnix.Inputs;
using Omnix.Utils.EditorUtils;


namespace Omnix.Editor
{
    [CustomPropertyDrawer(typeof(UnitInput))]
    public class UnitInputDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float gap = position.width / 3f;
            Rect rect = new Rect(position.x, position.y, (position.width / 3f) - 3f, position.height);
            EditorGUI.PropertyField(rect, property.FindPropertyChild("modifier"), GUIContent.none);
            rect.x += gap;
            EditorGUI.PropertyField(rect, property.FindPropertyChild("key"), GUIContent.none);
            rect.x += gap;
            EditorGUI.PropertyField(rect, property.FindPropertyChild("keyCheckType"), GUIContent.none);
            EditorGUI.EndProperty();
        }
    }
}