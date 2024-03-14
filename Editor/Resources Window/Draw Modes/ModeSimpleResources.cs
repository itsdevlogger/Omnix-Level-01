using UnityEditor;
using UnityEngine;

namespace Omnix.Editor.Windows.Resources
{
    public class ModeSimpleResources : RecWinDrawMod
    {
        public ModeSimpleResources(ResourcesWindow window)
        {
            Window = window;
        }

        public override void Draw()
        {
            ResourcesStorage storage = ResourcesStorage.Instance;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool clearAll = GUILayout.Button("Clear All", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f), GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (storage.LayersCount != 0 && clearAll)
            {
                storage.Clear();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            Object objectToAdd = EditorGUILayout.ObjectField(null, typeof(Object), false, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));
            EditorGUILayout.EndHorizontal();

            if (objectToAdd != null)
            {
                string layerName = storage.GetFirstLayerName();
                Window.AddObjectToLayer(layerName, objectToAdd);
                return;
            }

            EditorGUILayout.Space(20);

            foreach (LayerInfo layerInfo in storage.AllLayers)
            {
                foreach (ObjectInfo objectInfo in layerInfo.allObjects)
                {
                    bool deleteObject = DrawSingleResource(objectInfo, 10, 20, layerInfo.name);
                    if (deleteObject)
                    {
                        storage.RemoveFrom(layerInfo.name, objectInfo);
                        break;
                    }
                }
            }
        }
    }
}