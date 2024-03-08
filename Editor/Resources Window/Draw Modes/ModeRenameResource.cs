using UnityEditor;
using UnityEngine;

namespace Omnix.Editor.Windows.Resources
{
    public class ModeRenameResource : RecWinDrawMod
    {
        private ObjectInfo _target;
        private string _currentName;
        private string[] _allLayers;
        private int _selectedLayer;
        private string _originalLayer;

        public ModeRenameResource(ObjectInfo target, ResourcesWindow window, string layerName)
        {
            _target = target;
            Window = window;
            _currentName = target.displayName;
            _allLayers = new string[ResourcesStorage.Instance.LayersCount];
            _originalLayer = layerName;
            int index = 0;
            foreach (LayerInfo layerInfo in ResourcesStorage.Instance.AllLayers)
            {
                if (layerInfo.name == layerName) _selectedLayer = index;
                _allLayers[index] = layerInfo.name;
                index++;
            }
        }

        public override void Draw()
        {
            _currentName = EditorGUILayout.TextField("New Name", _currentName);
            _selectedLayer = EditorGUILayout.Popup(_selectedLayer, _allLayers);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                _target.displayName = _currentName;
                if (_selectedLayer >= 0 && _selectedLayer < _allLayers.Length)
                {
                    ResourcesStorage.Instance.MoveResourceLayer(_target, _originalLayer, _allLayers[_selectedLayer]);
                }

                Window.SwitchToList();
            }

            if (GUILayout.Button("Cancel"))
            {
                Window.SwitchToList();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}