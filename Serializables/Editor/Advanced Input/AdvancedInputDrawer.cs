using Omnix.Inputs;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AdvancedInput))]
public class AdvancedInputDrawer : PropertyDrawer
{
    private SerializedProperty combinations;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (combinations == null)
        {
            combinations = property.FindPropertyRelative("combinations");
        }

        return EditorGUI.GetPropertyHeight(combinations);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        // EditorGUI.PropertyField(position, combinations, label);
        EditorGUI.PropertyField(position, combinations, label, true);
        EditorGUI.EndProperty();
    }
}
