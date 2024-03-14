using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using ThemedUi;


namespace Omnix.Editor.Windows.Resources
{
    public class ResourcesStorage : EditorStorage<ResourcesStorage>
    {
        public bool isLayeredView;
        [SerializeField] private List<LayerInfo> _resourcesLayers = new List<LayerInfo>();
        [SerializeField] private Color[] _layerBackgroundColors = new Color[0];
        private Dictionary<string, LayerInfo> _allLayers = new Dictionary<string, LayerInfo>();

        private int RandomColor => Random.Range(0, Instance._layerBackgroundColors.Length);
        public int LayersCount => Instance._resourcesLayers.Count;
        public IEnumerable<LayerInfo> AllLayers => _resourcesLayers;
        public LayerInfo this[string layerName] => _allLayers[layerName];
        public Color GetColor(int index) => _layerBackgroundColors[index];
        public bool ContainsLayer(string layerName) => !string.IsNullOrEmpty(layerName) && _allLayers.ContainsKey(layerName);
        public bool ContainsObjectInLayer(string layerName, Object obj) => ContainsLayer(layerName) && this[layerName].allObjects.Any(objectInfo => objectInfo.referenceObject.Equals(obj));
        public void AddTo(string layerName, ObjectInfo obj) => AddLayer(layerName).allObjects.Add(obj);

        private LayerInfo AddLayer(string layerName)
        {
            if (_allLayers.ContainsKey(layerName)) return _allLayers[layerName];
            LayerInfo info = new LayerInfo(layerName, RandomColor);
            _allLayers.Add(info.name, info);
            Instance._resourcesLayers.Add(info);
            return info;
        }

        protected override void Init()
        {
            if (_layerBackgroundColors == null)
            {
                _layerBackgroundColors = new Color[]
                {
                    new Color(0.322f, 0.188f, 0.188f),
                    new Color(0.322f, 0.231f, 0.188f),
                    new Color(0.263f, 0.322f, 0.188f),
                    new Color(0.219f, 0.321f, 0.188f),
                    new Color(0.200f, 0.322f, 0.188f),
                    new Color(0.188f, 0.322f, 0.220f),
                    new Color(0.188f, 0.322f, 0.286f),
                    new Color(0.188f, 0.290f, 0.322f),
                    new Color(0.188f, 0.231f, 0.322f),
                    new Color(0.239f, 0.188f, 0.322f),
                    new Color(0.298f, 0.188f, 0.322f),
                    new Color(0.322f, 0.188f, 0.278f)
                };
            }

            if (_resourcesLayers == null) _resourcesLayers = new List<LayerInfo>();
        }

        public string GetFirstLayerName()
        {
            if (_resourcesLayers.Count > 0) return _resourcesLayers[0].name;
            AddLayer("Default");
            return "Default";
        }

        public void UpdateAllLayers()
        {
            _allLayers = new Dictionary<string, LayerInfo>();
            foreach (LayerInfo layerInfo in _resourcesLayers)
            {
                _allLayers.Add(layerInfo.name, layerInfo);
            }
        }

        public void Clear()
        {
            _resourcesLayers.Clear();
            _allLayers.Clear();
        }

        public void RenameLayer(string oldName, string newName)
        {
            if (!ContainsLayer(oldName)) return;
            if (ContainsLayer(newName)) return;
            var layer = this._allLayers[oldName];
            layer.name = newName;
            this._allLayers.Add(newName, layer);
            this._allLayers.Remove(oldName);
        }

        public void RemoveFrom(string layerName, ObjectInfo obj)
        {
            LayerInfo layerInfo = this[layerName];
            if (layerInfo == null) return;

            if (layerInfo.allObjects.Contains(obj))
            {
                layerInfo.allObjects.Remove(obj);
            }
        }

        public void RemoveLayer(string layerName)
        {
            LayerInfo layerInfo = this[layerName];
            if (layerInfo == null) return;

            _allLayers.Remove(layerInfo.name);
            Instance._resourcesLayers.Remove(layerInfo);
        }
        
        public void AddLayer(LayerInfo info)
        {
            if (ContainsLayer(info.name)) return;
            this._allLayers.Add(info.name, info);
            Instance._resourcesLayers.Add(info);
        }
        
        public void MoveResourceLayer(ObjectInfo target, string oldLayer, string newLayer)
        {
            if (oldLayer == newLayer) return;

            if (!string.IsNullOrEmpty(oldLayer) && _allLayers.ContainsKey(oldLayer) && _allLayers[oldLayer].allObjects.Contains(target))
            {
                _allLayers[oldLayer].allObjects.Remove(target);
            }

            if (!_allLayers.ContainsKey(newLayer))
            {
                _allLayers.Add(newLayer, new LayerInfo(newLayer, RandomColor));
            }

            _allLayers[newLayer].allObjects.Add(target);
        }
    }
}