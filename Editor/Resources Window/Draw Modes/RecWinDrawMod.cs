using System.IO;
using UnityEditor;
using UnityEngine;
using Omnix.Utils.EditorUtils;

namespace Omnix.Editor.Windows.Resources
{
    public abstract class RecWinDrawMod
    {
        protected ResourcesWindow Window;

        public abstract void Draw();

        private static void OpenAsset(Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (Directory.Exists(assetPath))
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(asset.GetFirstAsset());
            }
            else
            {
                EditorGUIUtility.PingObject(asset);
                AssetDatabase.OpenAsset(asset);
            }
        }

        protected bool DrawSingleResource(ObjectInfo resource, float spaceBefore, float spaceAfter, string layerName)
        {
            if (resource.referenceObject == null) return true;

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            ButtonsRow.BeginRow(3, spaceBefore, spaceAfter);
            if (ButtonsRow.MiniButton("\u23ce"))
            {
                OpenAsset(resource.referenceObject);
            }

            if (ButtonsRow.BigButton(resource.Content))
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(resource.referenceObject.GetFirstAsset());
            }

            if (ButtonsRow.MiniButton("*"))
            {
                Window.SwitchDrawMode(new ModeRenameResource(resource, Window, layerName));
                return false;
            }

            return ButtonsRow.MiniButton("X");
        }
    }
}