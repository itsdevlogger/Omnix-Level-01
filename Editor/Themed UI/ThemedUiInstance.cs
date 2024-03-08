using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThemedUi
{
    public class ThemedUiInstance : EditorWindow
    {
        private List<GameObject> _array;
        private RectTransform _currentInstance;
        private Transform _canvas;
        private int _index;
        private Color _guiDefaultColor;
        private Vector2 _scrollPos;

        public static void StartWith(List<GameObject> array, Transform canvas)
        {
            var window = GetWindow<ThemedUiInstance>();
            window._array = array;
            window._index = 0;
            window._canvas = canvas;
            window.Refresh();
            window.Show();
        }

        private void Refresh()
        {
            var target = Instantiate(_array[_index], _canvas).GetComponent<RectTransform>();
            if (_currentInstance != null)
            {
                target.pivot = _currentInstance.pivot;
                target.anchorMax = _currentInstance.anchorMax;
                target.anchorMin = _currentInstance.anchorMin;
                target.anchoredPosition = _currentInstance.anchoredPosition;
                target.sizeDelta = _currentInstance.sizeDelta;
                target.eulerAngles = _currentInstance.eulerAngles;
                target.localScale = _currentInstance.localScale;
                target.name = _array[_index].name;
                DestroyImmediate(_currentInstance.gameObject);
            }
            _currentInstance = target;
            Selection.activeObject = _currentInstance;
        }

        private void InitNext()
        {
            _index = (_index + 1) % _array.Count;
            Refresh();
        }

        private void InitPrev()
        {
            _index = (_index + _array.Count - 1) % _array.Count;
            Refresh();
        }

        private void InitAt(int index)
        {
            _index = index % _array.Count;
            Refresh();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Next")) InitNext();
            if (GUILayout.Button("Prev")) InitPrev();
            EditorGUILayout.EndHorizontal();

            int i = 0;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            foreach (GameObject go in _array)
            {
                if (i == _index)
                {
                    _guiDefaultColor = GUI.color;
                    GUI.color = Color.green;
                    EditorGUILayout.LabelField(go.name, EditorStyles.toolbarButton);
                    GUI.color = _guiDefaultColor;
                }
                else if (GUILayout.Button(go.name, EditorStyles.toolbarButton))
                {
                    InitAt(i);
                }

                i++;
            }

            EditorGUILayout.EndScrollView();
        }
    }
}