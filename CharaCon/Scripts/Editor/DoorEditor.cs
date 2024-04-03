using UnityEditor;

namespace Omnix.CharaCon.CustomEditors
{
    [CustomEditor(typeof(DoorEditor))]
    public class DoorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("If Layer attribute is selected, then make sure that the new layer is also interactable (In AgentInteraction component)", MessageType.Info);
        }
    }
}