using System;
using System.Collections.Generic;
using System.Reflection;
using Omnix.CharaCon.InteractionSystem;
using UnityEditor;
using UnityEngine;

namespace Omnix.CharaCon.CustomEditors
{
    [CustomEditor(typeof(AgentInteraction))]
    public class AgentInteractionEditor : Editor
    {
        private FieldInfo _targetedObjects;
        private FieldInfo _interactingWithField;
        private static bool debugMode;
        private static bool targetsExpanded;
        private static bool interactExpanded;

        private void OnEnable()
        {
            _targetedObjects = typeof(AgentInteraction).GetField("_targetedObjects", BindingFlags.NonPublic | BindingFlags.Instance);
            _interactingWithField = typeof(AgentInteraction).GetField("_interactingWith", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            debugMode = EditorGUILayout.Toggle("Debug Mode", debugMode);
            if (debugMode)
            {
                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                targetsExpanded = DisplayHashSet(_targetedObjects, targetsExpanded, "Targeted Objects");
                interactExpanded = DisplayHashSet(_interactingWithField, interactExpanded, "Interacting With");
                GUI.enabled = guiEnabled;
            }
            
        }

        private bool DisplayHashSet(FieldInfo field, bool targetsExpanded, string fieldName)
        {
            if (field == null)
            {
                EditorGUILayout.LabelField($"Cannot access {fieldName} field. Make sure it's declared in AgentInteraction class as private.");
                return targetsExpanded;
            }
            

            targetsExpanded = EditorGUILayout.Foldout(targetsExpanded, fieldName);
            if (!targetsExpanded) return false;

            var interactingWith = (HashSet<InteractableBase>)field.GetValue(target);
            EditorGUI.indentLevel++;
            foreach (InteractableBase interactable in interactingWith)
            {
                EditorGUILayout.ObjectField(interactable, typeof(InteractableBase), true);
            }
            EditorGUI.indentLevel--;
            return true;
        }
    }
}