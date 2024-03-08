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

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            layerInfo.isExpanded = EditorGUILayout.Foldout(layerInfo.isExpanded, $"{layerInfo.name} ({objCount})");
            if (EditorGUI.EndChangeCheck())
            {
            }

            Object objectToAdd = EditorGUILayout.ObjectField(null, typeof(Object), false, Window.DropObjectsWidth);
            if (GUILayout.Button("*", Window.MiniButtonWidth))
            {
                EditorGUILayout.EndHorizontal();
                Window.SwitchDrawMode(new ModeRenameLayer(Window, layerInfo));
                return (null, false);
            }

            if (GUILayout.Button("X", Window.MiniButtonWidth))
            {
                EditorGUILayout.EndHorizontal();
                return (null, true);
            }

            EditorGUILayout.LabelField("", Window.MiniButtonWidth);
            EditorGUILayout.EndHorizontal();

            if (objectToAdd != null)
            {
                Window.AddObjectToLayer(layerInfo.name, objectToAdd);
            }

            if (!layerInfo.isExpanded)
            {
                EditorGUILayout.Space(10);
                return (null, false);
            }

            GUILayout.Space(9);
            Color color = GUI.backgroundColor;
            color.a = 0.5f;
            GUI.backgroundColor = color;

            foreach (ObjectInfo objectInfo in ResourcesStorage.Instance[layerInfo.name].allObjects)
            {
                if (DrawSingleResource(objectInfo, 30, 40, layerInfo.name))
                {
                    return (objectInfo, false);
                }
            }

            EditorGUILayout.Space(20);
            return (null, false);
        }

        public override void Draw()
        {
            ResourcesStorage storage = ResourcesStorage.Instance;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (storage.LayersCount != 0 && GUILayout.Button("Clear All", Window.MegaButtonWidth))
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
            if (GUILayout.Button("Add Section", Window.MegaButtonWidth))
            {
                Window.SwitchDrawMode(new ModeAddLayer(Window));
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}