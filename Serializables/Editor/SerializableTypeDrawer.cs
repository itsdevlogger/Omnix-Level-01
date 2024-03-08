using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Omnix.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(SerializableTypeDrawer))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float nextWidth = position.width - EditorGUIUtility.labelWidth;
            
            position.width = EditorGUIUtility.labelWidth;
            EditorGUI.PrefixLabel(position, label);

            position.x += position.width;
            position.width = nextWidth;
            SerializedProperty nameProp = property.FindPropertyRelative("guid");
            
            if (GUI.Button(position, nameProp.stringValue, EditorStyles.toolbarDropDown))
            {
                SerializableTypeSearchProvider.CreatePopup(UpdateDetails, null);
            }
        }

        private void UpdateDetails(Type obj)
        {
            
        }
    }
    
    
    public class SerializableTypeSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private static List<SearchTreeEntry> _searchTreeEntries = new List<SearchTreeEntry>();
        private static Type _treeEntriesAssignableFrom;
        
        private Action<Type> _onClick;
        private Type _assignableFrom;
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => _searchTreeEntries;

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            _onClick?.Invoke((Type)searchTreeEntry.userData);
            return true;
        }

        public static void CreatePopup(Action<Type> onClick, Type assignableFrom)
        {
            var current = CreateInstance<SerializableTypeSearchProvider>();
            current._onClick = onClick;
            current._assignableFrom = assignableFrom;
            if (_treeEntriesAssignableFrom != assignableFrom) current.UpdateTreeEntries();
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), current);
        }

        private void UpdateTreeEntries()
        {
            _treeEntriesAssignableFrom = _assignableFrom;
            _searchTreeEntries.Clear();
            
            _searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Select Type")));
            bool checkAssign = (_assignableFrom != null);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                _searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(assembly.FullName), 1));
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface) continue;
                    if (checkAssign && !type.IsAssignableFrom(_treeEntriesAssignableFrom)) continue;

                    SearchTreeEntry ste = new SearchTreeEntry(new GUIContent($"{type.Name} ({type.Namespace})"))
                    {
                        level = 2,
                        userData = type
                    };
                    _searchTreeEntries.Add(ste);
                }
            }
        }
    }
}