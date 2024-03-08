using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Omnix.Utils.EditorUtils;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Omnix.Editor
{
    public class WindowCopyComponent : EditorWindow
    {
        public const string COPY_COMPONENTS_TO = "Copy Components";

        private Dictionary<string, bool> _components;
        private Vector2 _componentsScrollPos;
        private Vector2 _targetsScrollPos;
        private bool _copyNonSerialized;
        private GameObject _selectedObject;
        List<GameObject> _targets;
        List<string> _changed;

        [MenuItem(OmnixMenu.OBJECT_MENU + COPY_COMPONENTS_TO, true)]
        [MenuItem(OmnixMenu.SELECT_MENU + COPY_COMPONENTS_TO, true)]
        private static bool IsSingleObjectSelected() => Selection.activeGameObject != null && Selection.gameObjects.Length == 1;

        [MenuItem(OmnixMenu.OBJECT_MENU + COPY_COMPONENTS_TO)]
        [MenuItem(OmnixMenu.SELECT_MENU + COPY_COMPONENTS_TO)]
        private static void Init() => GetWindow<WindowCopyComponent>();

        private void OnEnable()
        {
            _selectedObject = Selection.activeGameObject;
            _changed = new List<string>();
            _targets = new List<GameObject>();

            UpdateList();
        }

        private void OnDestroy()
        {
            _selectedObject = null;
        }

        private void OnGUI()
        {
            _copyNonSerialized = EditorGUILayout.ToggleLeft("Copy NonSerialized Fields As Well", _copyNonSerialized);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                DrawComponents();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                DrawTargets();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = _targets.Count >= 1;
            if (GUILayout.Button("Copy Selected"))
            {
                CopySelected();
                Close();
            }
            GUI.enabled = true;
        }

        private void DrawComponents()
        {
            _changed = new List<string>();
            _componentsScrollPos = GUILayout.BeginScrollView(_componentsScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            foreach (KeyValuePair<string, bool> pair in _components)
            {
                bool newVal = EditorGUILayout.ToggleLeft(pair.Key, pair.Value);
                if (newVal != pair.Value)
                {
                    _changed.Add(pair.Key);
                }
            }

            GUILayout.EndScrollView();

            foreach (string component in _changed)
            {
                _components[component] = !_components[component];
            }

            _changed.Clear();
        }

        private void DrawTargets()
        {
            EditorGUILayout.LabelField("Target Objects:");
            GameObject toAdd = (GameObject) EditorGUILayout.ObjectField(null, typeof(GameObject), true);
            if (toAdd != null) _targets.Add(toAdd.gameObject);
            
            _targetsScrollPos = GUILayout.BeginScrollView(_targetsScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            {
                int index = 0;
                int deleteAt = -1;
                foreach (GameObject go in _targets)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{index:00}. {go.name}");
                    if (GUILayout.Button("X"))
                    {
                        deleteAt = index;
                        break;
                    }

                    index++;
                    EditorGUILayout.EndHorizontal();
                }

                if (deleteAt >= 0)
                {
                    _targets.RemoveAt(deleteAt);
                }
            }
            GUILayout.EndScrollView();
        }

        private void CopySelected()
        {
            if (!_components.Any(pair => pair.Value))
            {
                Debug.LogError("Select at least 1 component");
                return;
            }

            Object[] targetObjects = new Object[_targets.Count];
            for (int i = 0; i < _targets.Count; i++)
            {
                targetObjects[i] = _targets[i];
            }
            Undo.RecordObjects(targetObjects, "Paste components");
            
            foreach (GameObject target in _targets)
            {
                foreach (Component component in _selectedObject.GetComponents<Component>())
                {
                    string cName = component.ClassName();
                    if (_components.ContainsKey(cName) && _components[cName])
                    {
                        component.CopyComponentTo(target);
                    }
                }
                EditorUtility.SetDirty(target);
            }
        }
        
        private void UpdateList()
        {
            List<string> comps = new List<string>();
            foreach (Component component in _selectedObject.GetComponents<Component>())
            {
                comps.Add(component.ClassName());
            }

            comps.Sort();

            _components = new Dictionary<string, bool>();
            foreach (string s in comps)
            {
                _components.Add(s, false);
            }
        }
    }
}