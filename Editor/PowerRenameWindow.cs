using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Omnix.Editor
{
    public class PowerRenameWindow : EditorWindow
    {
        private static Color TOGGLE_BUTTON_ACTIVE_COLOR = new Color(0.584f, 0.8f, 0.749f);
        private static Color TOGGLE_BUTTON_INACTIVE_COLOR = new Color(0.812f, 0.812f, 0.812f);

        public enum CaseFormatting
        {
            DoNothing,
            LowerCase,
            UpperCase,
            SentenceCase,
            TitleCase
        }

        private string _findWhat = "";
        private string _replaceWith = "";
        private string _prefix = "";
        private string _suffix = "";
        private bool _caseSensitive = false;
        private bool _useRegex = false;
        private bool _matchAllOccurrences = true;
        private bool _nicefyNames = false;
        private CaseFormatting _caseFormatting = CaseFormatting.DoNothing;

        private List<Object> _selectedObjects = new List<Object>();
        private List<string> _originalNames = new List<string>();
        private List<string> _renamedNames = new List<string>();
        
        private Vector2 _scrollPosition;
        private GUIStyle _boxStyle;
        private GUIStyle _bigButtonStyle;
        private Regex _findRegex;
        private Func<string, string> _caseFormatter;

        [MenuItem(OmnixMenu.OBJECT_MENU + "Power Rename", validate = true)]
        [MenuItem(OmnixMenu.SELECT_MENU + "Power Rename", validate = true)]
        private static bool Check() => Selection.gameObjects.Length > 0;


        [MenuItem(OmnixMenu.WINDOW_MENU + "Power Rename")]
        [MenuItem(OmnixMenu.OBJECT_MENU + "Power Rename")]
        [MenuItem(OmnixMenu.SELECT_MENU + "Power Rename")]
        private static void ShowWindow()
        {
            var window = GetWindow<PowerRenameWindow>("Power Rename");
            window.InitializeWithSelection();
        }

        private void InitializeWithSelection()
        {
            _selectedObjects = Selection.objects.ToList();
            Compile();
            UpdateNames();
        }

        private void OnGUI()
        {
            if (_boxStyle == null)
                _boxStyle = "box";

            // Top Area
            DrawSettings();
            GUILayout.Space(15f);
            DrawPreview();
        }

        private void DrawSettings()
        {
            if (_bigButtonStyle == null)
            {
                _bigButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
                _bigButtonStyle.fixedHeight = 30f;
            }

            EditorGUILayout.BeginVertical(_boxStyle);
            EditorGUI.BeginChangeCheck();
            
            _findWhat = EditorGUILayout.TextField("Find What", _findWhat);
            _replaceWith = EditorGUILayout.TextField("Replace With", _replaceWith);
            _prefix = EditorGUILayout.TextField("Prefix", _prefix);
            _suffix = EditorGUILayout.TextField("Suffix", _suffix);
            _caseFormatting = (CaseFormatting)EditorGUILayout.EnumPopup("Case Formatting", _caseFormatting);
            
            EditorGUILayout.BeginHorizontal();
            _nicefyNames = ToggleButton("Nicefy Names", _nicefyNames);
            _caseSensitive = ToggleButton("Case Sensitive", _caseSensitive);
            _useRegex = ToggleButton("Use Regex", _useRegex);
            _matchAllOccurrences = ToggleButton("Match All Occurrences", _matchAllOccurrences);
            EditorGUILayout.EndHorizontal();
            
            bool needUpdate = EditorGUI.EndChangeCheck();

            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", _bigButtonStyle, GUILayout.Height(30f)))
            {
                PerformRename();
            }
            if (GUILayout.Button("Apply and Close", _bigButtonStyle, GUILayout.Height(30f)))
            {
                PerformRename();
                Close();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);
            Object obj = EditorGUILayout.ObjectField(null, typeof(Object), true, GUILayout.Height(30f));
            if (obj != null && !_selectedObjects.Contains(obj))
            {
                _selectedObjects.Add(obj);
                needUpdate = true;
            }

            if (needUpdate)
            {
                Compile();
                UpdateNames();
            } 
        }

        private void DrawPreview()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            var width = GUILayout.Width(position.width / 2f - 5f);

            for (int i = 0; i < _originalNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(_boxStyle);
                EditorGUILayout.LabelField(_originalNames[i], width);
                EditorGUILayout.LabelField(_renamedNames[i], width);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2f);
            }
            EditorGUILayout.EndScrollView();
        }

        private bool ToggleButton(string text, bool toggle)
        {
            var color = GUI.backgroundColor;
            GUI.backgroundColor = toggle ? TOGGLE_BUTTON_ACTIVE_COLOR : TOGGLE_BUTTON_INACTIVE_COLOR;
            if (GUILayout.Button(text, _bigButtonStyle, GUILayout.Height(25f))) toggle = !toggle;
            GUI.backgroundColor = color;
            return toggle;
        }


        private void Compile()
        {
            string finding;
            if (_useRegex) finding = _findWhat;
            else finding = Regex.Escape(_findWhat);

            if (_caseSensitive) _findRegex = new Regex(finding);
            else _findRegex = new Regex(finding, RegexOptions.IgnoreCase);


            switch (_caseFormatting)
            {
                case CaseFormatting.LowerCase:
                    _caseFormatter = text => text.ToLower();
                    break;
                case CaseFormatting.UpperCase:
                    _caseFormatter = text => text.ToUpper();
                    break;
                case CaseFormatting.SentenceCase:
                    _caseFormatter = ToSentenceCase;
                    break;
                case CaseFormatting.TitleCase:
                    _caseFormatter = ToTitleCase;
                    break;
                default:
                    _caseFormatter = text => text;
                    break;
            }
        }

        private void PerformRename()
        {
            Compile();

            int undoGroupIndex = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName($"Power Renamed {_selectedObjects.Count} Objects");

            for (int i = 0; i < _selectedObjects.Count; i++)
            {
                var obj = _selectedObjects[i];
                if (obj == null) continue;

                string newName = GetRenamedName(_originalNames[i], i + 1);
                Undo.RecordObject(obj, "Rename Object");
                obj.name = newName;
            }

            Undo.CollapseUndoOperations(undoGroupIndex);
            UpdateNames();
        }

        private void UpdateNames()
        {
            _originalNames.Clear();
            _renamedNames.Clear();
            int index = 1;
            foreach (var obj in _selectedObjects)
            {
                if (obj == null) continue;

                string originalName = obj.name;
                _originalNames.Add(originalName);
                _renamedNames.Add(GetRenamedName(originalName, index));
                index++;
            }
        }

        private static string ToSentenceCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            input = input.ToLower(CultureInfo.InvariantCulture);
            return char.ToUpper(input[0], CultureInfo.InvariantCulture) + input.Substring(1);
        }

        private static string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }

        private string GetRenamedName(string originalName, int index)
        {
            string replaced;
            _replaceWith = _replaceWith.Replace("{i}", index.ToString());
            if (_matchAllOccurrences) replaced = _findRegex.Replace(originalName, _replaceWith);
            else replaced = _findRegex.Replace(originalName, _replaceWith, 1);
            
            replaced = _caseFormatter(replaced);
            if (_nicefyNames) replaced = ObjectNames.NicifyVariableName(replaced);
            return _prefix + replaced + _suffix;

            //if (_useRegex)
            //{
            //    if (_matchAllOccurrences)
            //        return _findRegex.Replace(originalName, _replaceWith);
            //    return _findRegex.Replace(originalName, _replaceWith, 1);
            //}

            //if (string.IsNullOrEmpty(_findWhat)) return originalName;

            //var comparison = _caseSensitive ? System.StringComparison.Ordinal : System.StringComparison.OrdinalIgnoreCase;
            //return originalName.Replace(_findWhat, _replaceWith, comparison);
        }
    }
}