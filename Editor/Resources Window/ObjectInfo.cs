using System;
using Omnix.Utils.EditorUtils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Omnix.Editor.Windows.Resources
{
    [Serializable]
    public class ObjectInfo
    {
        public string displayName;
        public Object referenceObject;
        public Texture icon;
        private GUIContent _content;

        public GUIContent Content
        {
            get
            {
                if (_content == null || string.IsNullOrEmpty(_content.text)) _content = new GUIContent(displayName, icon);
                return _content;
            }
        }

        public ObjectInfo()
        {
        }

        public ObjectInfo(string displayName, Object referenceObject)
        {
            this.displayName = displayName;
            this.referenceObject = referenceObject;
            icon = this.referenceObject.GetIcon();
        }
    }
}