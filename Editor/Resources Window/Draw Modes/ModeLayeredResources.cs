using UnityEditor;
using UnityEngine;

namespace Omnix.Editor.Windows.Resources
{
    public class ModeLayeredResources : RecWinDrawMod
    {
        public ModeLayeredResources(ResourcesWindow window)
        {
            this.Window = window;
        }

        (ObjectInfo, bool) DrawLayerResources(LayerInfo layerInfo)
        {
            int objCount = layerInfo.allObjects.Count;
            if (layerInfo.isExpanded) Window.StylizeGUI(objCount + 2, 22, layerInfo.Color);
            else Window.StylizeGUI(2, 0, layerInfo.Color);

            Color color = GUI.color;
            GUI.color = layerInfo.FaintColor;

            EditorGUILayout.BeginHorizontal();
            layerInfo.isExpanded = EditorGUILayout.Foldout(layerInfo.isExpanded, $"{layerInfo.name} ({objCount})");

            Object objectToAdd = EditorGUILayout.ObjectField(null, typeof(Object), false, MiniButtonLayoutOptions(3f));
            GUILayoutOption[] layoutOptions = MiniButtonLayoutOptions(1f);
            if (GUILayout.Button("*", layoutOptions))
            {
                EditorGUILayout.EndHorizontal();
                Window.SwitchDrawMode(new ModeRenameLayer(Window, layerInfo));
                GUI.color = color;
                return (null, false);
            }

            if (GUILayout.Button("X", layoutOptions))
            {
                EditorGUILayout.EndHorizontal();
                GUI.color = color;
                return (null, true);
            }

            EditorGUILayout.LabelField("", layoutOptions);
            EditorGUILayout.EndHorizontal();

            if (objectToAdd != null)
            {
                Window.AddObjectToLayer(layerInfo.name, objectToAdd);
            }

            if (!layerInfo.isExpanded)
            {
                EditorGUILayout.Space(10);
                GUI.color = color;
                return (null, false);
            }

            GUILayout.Space(9);

            foreach (ObjectInfo objectInfo in ResourcesStorage.Instance[layerInfo.name].allObjects)
            {
                if (DrawSingleResource(objectInfo, 30, 40, layerInfo.name))
                {
                    GUI.color = color;
                    return (objectInfo, false);
                }
            }

            EditorGUILayout.Space(20);
            GUI.color = color;
            return (null, false);
        }

        public override void Draw()
        {
            ResourcesStorage storage = ResourcesStorage.Instance;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var layoutOptions = MiniButtonLayoutOptions(3f);
            if (storage.LayersCount != 0 && GUILayout.Button("Clear All", layoutOptions))
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                storage.Clear();
                return;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            foreach (LayerInfo layer in storage.AllLayers)
            {
                (ObjectInfo, bool) variable = DrawLayerResources(layer);
                if (variable.Item1 != null)
                {
                    storage.RemoveFrom(layer.name, variable.Item1);
                    break;
                }

                if (variable.Item2)
                {
                    storage.RemoveLayer(layer.name);
                    break;
                }
            }

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Section", layoutOptions))
            {
                Window.SwitchDrawMode(new ModeAddLayer(Window));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}