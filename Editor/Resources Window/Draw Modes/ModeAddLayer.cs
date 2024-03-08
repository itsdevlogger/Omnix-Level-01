using UnityEditor;
using UnityEngine;

namespace Omnix.Editor.Windows.Resources
{
    public class ModeAddLayer : RecWinDrawMod
    {
        private string _curSelSecName;
        private int _curSelColorIndex = 0;

        public ModeAddLayer(ResourcesWindow window)
        {
            Window = window;
            _curSelSecName = "";
        }


        public override void Draw()
        {
            GUILayout.Space(20);
            _curSelSecName = EditorGUILayout.TextField("Name", _curSelSecName);
            _curSelColorIndex = Window.DrawColorsPicker(_curSelColorIndex);
            bool notAlowed = string.IsNullOrEmpty(_curSelSecName) || ResourcesStorage.Instance.ContainsLayer(_curSelSecName);

            if (notAlowed)
            {
                EditorGUILayout.HelpBox($"Duplicate or Null layer names are not allowed", MessageType.Error);
                GUI.enabled = false;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm") && !notAlowed)
            {
                ResourcesStorage.Instance.AddLayer(new LayerInfo(_curSelSecName, _curSelColorIndex));
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