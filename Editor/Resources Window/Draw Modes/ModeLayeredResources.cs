using Omnix.Utils.EditorUtils;
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
            ButtonsRow.BeginRow(2);
            EditorGUILayout.EndHorizontal();

            Object objectToAdd = ButtonsRow.BigObjectField(null, typeof(Object), false);
            if (ButtonsRow.MiniButton("*"))
            {
                EditorGUILayout.EndHorizontal();
                Window.SwitchDrawMode(new ModeRenameLayer(Window, layerInfo));
                GUI.color = color;
                return (null, false);
            }

            if (ButtonsRow.MiniButton("X"))
            {
                EditorGUILayout.EndHorizontal();
                GUI.color = color;
                return (null, true);
            }

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
            if (storage.LayersCount != 0 && ButtonsRow.HeaderButton("Clear All"))
            {
                storage.Clear();
                return;
            }

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
            if (ButtonsRow.HeaderButton("Add Section"))
            {
                Window.SwitchDrawMode(new ModeAddLayer(Window));
            }
        }
    }
}