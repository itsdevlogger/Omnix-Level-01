using System.Collections;
using System.Collections.Generic;
using ThemedUi;
using UnityEngine;
using SceneAsset = UnityEditor.SceneAsset;

namespace Omnix.Editor
{
    public class ScenesStorage : EditorStorage<ScenesStorage>, IEnumerable<SceneAsset>
    {
        [SerializeField] private List<SceneAsset> _assets = new List<SceneAsset>();

        protected override void Init()
        {
        }

        public void Add(SceneAsset asset)
        {
            _assets.Add(asset);
            Save();
        }

        public bool Remove(SceneAsset scene)
        {
            var remove = _assets.Remove(scene);
            Save();
            return remove;
        }

        public int Count => _assets.Count;

        public IEnumerator<SceneAsset> GetEnumerator() => _assets.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}