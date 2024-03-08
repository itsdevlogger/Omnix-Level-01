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
            if (storage.LayersCount != 0 && GUILayout.Button("Clear All"))
            {
                storage.Clear();
                return;
            }

            Object objectToAdd = EditorGUILayout.ObjectField(null, typeof(Object), false);

            if (objectToAdd != null)
            {
                string layerName = storage.GetFirstLayerName();
                Window.AddObjectToLayer(layerName, objectToAdd);
                return;
            }

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