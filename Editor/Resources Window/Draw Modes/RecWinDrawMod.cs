using UnityEditor;
using UnityEngine;
using Omnix.Utils.EditorUtils;

namespace Omnix.Editor.Windows.Resources
{
    public abstract class RecWinDrawMod
    {
        private static GUIStyle _buttonLeftAligned;
        private static GUIStyle _buttonCenterAligned;
        protected ResourcesWindow Window;

        private static readonly Vector2 PADDING = new Vector2(10f, 3f);
        private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
        private static readonly float MINI_BUTTON_WIDTH = LINE_HEIGHT * 2f;

        private static GUIStyle ButtonLeftAligned
        {
            get
            {
                if (_buttonLeftAligned != null) return _buttonLeftAligned;

                int x = (int)(PADDING.x * 0.5f);
                int y = (int)(PADDING.y * 0.5f);
                _buttonLeftAligned = new GUIStyle(EditorStyles.toolbarButton)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(x, x, y, y)
                };
                return _buttonLeftAligned;
            }
        }

        private static GUIStyle ButtonCenterAligned
        {
            get
            {
                if (_buttonCenterAligned != null) return _buttonCenterAligned;

                int x = (int)(PADDING.x * 0.5f);
                int y = (int)(PADDING.y * 0.5f);
                _buttonCenterAligned = new GUIStyle(EditorStyles.toolbarButton)
                {
                    alignment = TextAnchor.MiddleCenter,
                    padding = new RectOffset(x, x, y, y)
                };

                return _buttonCenterAligned;
            }
        }


        public abstract void Draw();

        protected bool DrawSingleResource(ObjectInfo resource, float spaceBefore, float spaceAfter, string layerName)
        {
            if (resource.referenceObject == null) return true;

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            Rect original = GUILayoutUtility.GetRect(Window.position.width, LINE_HEIGHT + PADDING.y);
            original.x += spaceBefore;
            original.width -= spaceBefore + spaceAfter;

            Rect posRect = new Rect(original);
            posRect.width = MINI_BUTTON_WIDTH;
            if (GUI.Button(posRect, "\u23ce", ButtonCenterAligned))
            {
                AssetDatabase.OpenAsset(resource.referenceObject);
            }


            posRect.x += posRect.width + EditorGUIUtility.standardVerticalSpacing;
            posRect.width = original.width - (MINI_BUTTON_WIDTH + EditorGUIUtility.standardVerticalSpacing) * 3f;
            if (GUI.Button(posRect, resource.Content, ButtonLeftAligned))
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(resource.referenceObject.GetFirstAsset());
            }

            posRect.x += posRect.width + EditorGUIUtility.standardVerticalSpacing;
            posRect.width = MINI_BUTTON_WIDTH;
            if (GUI.Button(posRect, "*", ButtonCenterAligned))
            {
                Window.SwitchDrawMode(new ModeRenameResource(resource, Window, layerName));
                return false;
            }

            posRect.x += posRect.width + EditorGUIUtility.standardVerticalSpacing;
            if (GUI.Button(posRect, "X", ButtonCenterAligned)) return true;
            return false;
        }

        protected static GUILayoutOption[] MiniButtonLayoutOptions(float widthMultiple)
        {
            return new[] { GUILayout.Width(MINI_BUTTON_WIDTH * widthMultiple), GUILayout.Height(LINE_HEIGHT + PADDING.y) };
        }
    }
}