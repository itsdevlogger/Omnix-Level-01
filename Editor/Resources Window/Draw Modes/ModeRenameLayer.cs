using UnityEditor;
using UnityEngine;

namespace Omnix.Editor.Windows.Resources
{
    public class ModeRenameLayer : RecWinDrawMod
    {
        private string _curSelSecName;
        private int _curSelColorIndex = 0;
        private LayerInfo _layerInfo;

        public ModeRenameLayer(ResourcesWindow window, LayerInfo layerInfo)
        {
            this._layerInfo = layerInfo;
            this._curSelColorIndex = layerInfo.ColorIndex;
            this._curSelSecName = layerInfo.name;
            this.Window = window;
        }

        public override void Draw()
        {
            GUILayout.Space(20);
            _curSelSecName = EditorGUILayout.TextField("New Name", _curSelSecName);
            _curSelColorIndex = Window.DrawColorsPicker(_curSelColorIndex);
            EditorGUILayout.LabelField($"Index: {_curSelColorIndex}");
            bool notAlowed = ResourcesStorage.Instance.ContainsLayer(_curSelSecName) && _curSelColorIndex == _layerInfo.ColorIndex;
            if (notAlowed)
            {
                EditorGUILayout.HelpBox($"Change either color or name to confirm", MessageType.Error);
                GUI.enabled = false;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                if (_layerInfo.name != _curSelSecName)
                {
                    ResourcesStorage.Instance.RenameLayer(_layerInfo.name, _curSelSecName);
                }

                _layerInfo.SetColorIndex(_curSelColorIndex);
                Window.SwitchToList();
            }

            GUI.enabled = true;
            if (GUILayout.Button("Cancel"))
            {
                Window.SwitchToList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}