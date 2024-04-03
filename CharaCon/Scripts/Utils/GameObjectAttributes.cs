using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omnix.CharaCon.Utils
{
    /// <summary>
    /// Represents different attributes that a GameObject has.
    /// See Extension Method <see cref="Omnix.CharaCon.Utils.Extensions.Copy"/>
    /// </summary>
    [Flags]
    public enum GameObjectAttributes
    {
        Position = 1 << 1, // bits: 
        LocalPosition = 1 << 2,
        Rotation = 1 << 3,
        LocalRotation = 1 << 4,
        Scale = 1 << 5,
        Parent = 1 << 6,
        Layer = 1 << 7,
        Tag = 1 << 8,
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GameObjectAttributes))]
    public class GameObjectAttributesDrawer : PropertyDrawer
    {
        private bool HasPositionConflict(int flags) => (flags & (int)GameObjectAttributes.Position) != 0 && (flags & (int)GameObjectAttributes.LocalPosition) != 0;
        private bool HasRotationConflict(int flags) => (flags & (int)GameObjectAttributes.Rotation) != 0 && (flags & (int)GameObjectAttributes.LocalRotation) != 0;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            int flags = property.intValue;
            if (HasPositionConflict(flags)) height += EditorGUIUtility.singleLineHeight;// + EditorGUIUtility.standardVerticalSpacing;
            if (HasRotationConflict(flags)) height += EditorGUIUtility.singleLineHeight;// + EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        // Debug.LogWarning("LocalPosition will be ignored as Position is Selected.");
        // Debug.LogWarning("LocalRotation will be ignored as Rotation is Selected.");
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            bool posCon = HasPositionConflict(property.intValue);
            bool rotCon = HasRotationConflict(property.intValue);
            Color guiColor = GUI.color;
            if (posCon || rotCon) GUI.color = new Color(1f, 0.73f, 0.55f);
            EditorGUI.PropertyField(rect, property, label, true);
            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;
            if (posCon)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                GUI.Label(rect, "LocalPosition will be ignored as Position is Selected.", EditorStyles.miniLabel);
            }

            if (rotCon)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                GUI.Label(rect, "LocalRotation will be ignored as Rotation is Selected.", EditorStyles.miniLabel);
            }
            GUI.color = guiColor;
            EditorGUI.EndProperty();
        }
    }

    #endif

    /*
   Just some Cool Stuff:
   1. Check if 'Position' flag is set in atts
       (atts & GameObjectAttributes.Position) != 0

   2. Combine 'Position' and 'Rotation' flags in single flag
       atts = GameObjectAttributes.Position | GameObjectAttributes.Rotation

   3. Toggle 'Position' flag
       atts ^= GameObjectAttributes.Position;

   4. Toggle both Position and Rotation flags
       atts ^= GameObjectAttributes.Position | GameObjectAttributes.Rotation;

   5. Clear 'Position' flag, i.e. if Position flag is set then unset it, keep everything else as it is
       atts &= ~GameObjectAttributes.Position;

   6. Clear both Position and Rotation flag
       atts &= ~(GameObjectAttributes.Position | GameObjectAttributes.Rotation);

   7. Check if either 'Position' or 'Rotation' (or both) flags are set
       (atts & (GameObjectAttributes.Position | GameObjectAttributes.Rotation)) != 0
       ((atts & GameObjectAttributes.Position) ^ (atts & GameObjectAttributes.Rotation)) != 0

   8. Check if exactly one of 'Position' and 'Rotation' flags is set
       ((attributes & GameObjectAttributes.Position) != 0) ^ ((attributes & GameObjectAttributes.Rotation) != 0)

   9. Check if exactly one flag is set in atts
       (atts & (atts - 1)) == 0 && atts != 0
    */
}