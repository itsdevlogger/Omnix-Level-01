using UnityEditor;
using UnityEngine;
using Omnix.Utils.EditorUtils;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Omnix.Editor.Windows.Resources
{
    public abstract class RecWinDrawMod
    {
        private static readonly float LINE_HEIGHT = 20;
        private static GUIStyle _buttonStyle;
        protected ResourcesWindow Window;

        public abstract void Draw();

        protected bool DrawSingleResource(ObjectInfo resource, float spaceBefore, float spaceAfter, string layerName)
        {
            if (resource.referenceObject == null) return true;

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
                _buttonStyle.alignment = TextAnchor.MiddleLeft;
                _buttonStyle.padding  = new RectOffset(30, 0, 0, 0);
            }
            
            
            EditorGUILayout.Space(5);

            Rect posRect = GUILayoutUtility.GetRect(Window.position.width, LINE_HEIGHT - 4);
            posRect.x += spaceBefore;
            posRect.height += 3;
            posRect.width -= LINE_HEIGHT * 3 + 30 + spaceAfter;
            if (GUI.Button(posRect, resource.displayName, _buttonStyle))
            {
                if (resource.referenceObject.GetType().IsAssignableFrom(typeof(UnityEditor.SceneAsset)))
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(resource.referenceObject));
                }
                else
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject(resource.referenceObject.GetFirstAsset());
                }
            }

            posRect.width = LINE_HEIGHT;
            posRect.x += 6;
            GUI.Label(posRect, resource.icon);
            posRect.x += Window.position.width - LINE_HEIGHT * 3 - 30 - spaceAfter;

            if (GUI.Button(posRect, "*"))
            {
                Window.SwitchDrawMode(new ModeRenameResource(resource, Window, layerName));
                return false;
            }

            posRect.x += LINE_HEIGHT + 4;
            if (GUI.Button(posRect, "X")) return true;
            posRect.x += LINE_HEIGHT + 4;
            Object toMove = EditorGUI.ObjectField(posRect, null, typeof(Object), false);
            if (toMove) toMove.MoveAssetTo(resource.referenceObject);
            return false;
        }
    }
}