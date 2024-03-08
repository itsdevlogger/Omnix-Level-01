using UnityEngine;
using UnityEditor;


namespace Omnix.Editor
{
    public class WindowEditTransform : EditorWindow
    {
        private const string EDIT_TRANSFORM = "Edit Transform";
        
        
        private GameObject[] selection;
        private bool _editPosition = false;
        private bool _editRot = false;
        private bool _editScale = false;
        private Vector3 _positionOffset;
        private Vector3 _rotationOffset;
        private Vector3 _scaleOffset;

        [MenuItem(OmnixMenu.OBJECT_MENU + EDIT_TRANSFORM, true)]
        [MenuItem(OmnixMenu.SELECT_MENU + EDIT_TRANSFORM, true)]
        public static bool IsAnythingSelected() => Selection.gameObjects.Length > 0;
        
        [MenuItem(OmnixMenu.OBJECT_MENU + EDIT_TRANSFORM)]
        [MenuItem(OmnixMenu.SELECT_MENU + EDIT_TRANSFORM)]
        private static void Init()
        {
            
            if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
            {
                WindowEditTransform window = GetWindow<WindowEditTransform>(utility: true, focus: true, title: "Edit Transform Window");
                if (window.selection == null)
                {
                    window.ShowModal();
                }
            }
        }

        private void OnEnable()
        {
            selection = Selection.gameObjects;
        }
        
        private void OnDestroy()
        {
            selection = null;
        }

        private void OnGUI()
        {
            _editPosition = EditorGUILayout.ToggleLeft("Position", _editPosition);
            _editRot = EditorGUILayout.ToggleLeft("Rotation", _editRot);
            _editScale = EditorGUILayout.ToggleLeft("Scale", _editScale);

            GUI.enabled = _editPosition;
            _positionOffset = EditorGUILayout.Vector3Field("Position", _positionOffset);
            GUI.enabled = _editRot;
            _rotationOffset = EditorGUILayout.Vector3Field("Rotation", _rotationOffset);
            GUI.enabled = _editScale;
            _scaleOffset = EditorGUILayout.Vector3Field("Scale", _scaleOffset);
            GUI.enabled = true;

            if (GUILayout.Button("Apply")) OffsetSelection();
            if (GUILayout.Button("Cancel")) Close();
        }

        private void OffsetSelection()
        {
            if (!_editPosition && !_editRot && !_editScale)
            {
                Close();
                return;
            }
            
            foreach (GameObject gameObject in selection)
            {
                Transform transform = gameObject.transform;
                Undo.RecordObject(transform, $"Offset {transform.name}");
                if (_editPosition) transform.position += _positionOffset;
                if (_editRot) transform.Rotate(_rotationOffset);
                if (_editScale) transform.localScale += _scaleOffset;
                EditorUtility.SetDirty(transform);
            }

            Close();
        }
    }
}