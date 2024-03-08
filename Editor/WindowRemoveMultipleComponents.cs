using System.Collections.Generic;
using Omnix.Utils.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace Omnix.Editor
{
    public class WindowRemoveMultipleComponents : EditorWindow
    {
        private const string REMOVE_MULTIPLE_COMPONENTS = "Remove Multiple Components";
        private Dictionary<string, bool> _components;
        private GameObject _selectedObject;
        private Vector2 _scrollPos;

        [MenuItem(OmnixMenu.OBJECT_MENU + REMOVE_MULTIPLE_COMPONENTS)]
        [MenuItem(OmnixMenu.SELECT_MENU + REMOVE_MULTIPLE_COMPONENTS)]
        private static void Init() => GetWindow<WindowRemoveMultipleComponents>();

        [MenuItem(OmnixMenu.OBJECT_MENU + REMOVE_MULTIPLE_COMPONENTS, true)]
        [MenuItem(OmnixMenu.SELECT_MENU + REMOVE_MULTIPLE_COMPONENTS, true)]
        public static bool IsSingleObjectSelected() => Selection.activeGameObject != null && Selection.gameObjects.Length == 1;

        
        public void OnEnable()
        {
            _selectedObject = Selection.activeGameObject;
            UpdateList();
        }

        private void UpdateList()
        {
            _components = new Dictionary<string, bool>();
            foreach (Component component in _selectedObject.GetComponents<Component>())
            {
                if (component is Transform) continue;
                _components.Add(component.ClassName(), false);
            }
        }

        private void OnGUI()
        {
            List<string> changed = new List<string>();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);
            foreach (KeyValuePair<string, bool> pair in _components)
            {
                bool newVal = EditorGUILayout.ToggleLeft(pair.Key, pair.Value);
                if (newVal != pair.Value)
                {
                    changed.Add(pair.Key);
                }
            }
            GUILayout.EndScrollView();

            foreach (string component in changed)
            {
                _components[component] = !_components[component];
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Selected"))
            {
                EditorMenuHelpers.RemoveComponents(_selectedObject, c => _components[c.ClassName()]);
            }

            if (GUILayout.Button("Keep Selected"))
            {
                EditorMenuHelpers.RemoveComponents(_selectedObject, c => !_components[c.ClassName()]);
            }
            EditorGUILayout.EndHorizontal();
            changed.Clear();
        }

    }
}