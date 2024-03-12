using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnix.Editor.Windows.Resources
{
    [Serializable]
    public class LayerInfo
    {
        public string name;
        public bool isExpanded;
        public List<ObjectInfo> allObjects;
        private Color _color;
        private Color _tinted = Color.clear;
        [field: SerializeField] public int ColorIndex { get; private set; }


        public Color Color
        {
            get
            {
                if (_color == Color.clear) _color = ResourcesStorage.Instance.GetColor(ColorIndex);
                return _color;
            }
        }

        public Color FaintColor
        {
            get
            {
                if (_tinted == Color.clear)
                {
                    _tinted = _color;
                    _tinted.r *= 5f;
                    _tinted.g *= 5f;
                    _tinted.b *= 5f;
                    _tinted.a = 1f;
                }
                return _tinted;
            }
        }

        public void SetColorIndex(int index)
        {
            ColorIndex = index;
            _color = ResourcesStorage.Instance.GetColor(ColorIndex);
            _tinted = _color;
            _tinted.r *= 5f;
            _tinted.g *= 5f;
            _tinted.b *= 5f;
            _tinted.a = 1f;
        }

        public LayerInfo()
        {
        }

        public LayerInfo(string name, int colorIndex, bool isExpanded = false)
        {
            this.ColorIndex = colorIndex;
            this.isExpanded = isExpanded;
            this.name = name;
            allObjects = new List<ObjectInfo>();
        }
    }
}