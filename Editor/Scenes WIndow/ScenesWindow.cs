using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SceneAsset = UnityEditor.SceneAsset;

namespace Omnix.Editor
{
    public class ScenesWindow : EditorWindow
    {
        private GUILayoutOption MiniButtonWidth => GUILayout.Width(25);
        private GUIStyle _buttonStyleInactive;
        private GUIStyle _buttonStyleActive;

        private List<SceneAsset> _buildScenes;
        private Vector2 _scrollPos;
        private SceneAsset _activeSceneAsset;
        private Scene _activeScene;
        private bool _needAssignButtonStyles = true;
        
        [MenuItem(OmnixMenu.WINDOW_MENU + "Scenes")]
        private static void Init() => GetWindow(typeof(ScenesWindow), false, "Scene").Show();

        [MenuItem(OmnixMenu.STORAGE_MENU + "Scenes")]
        private static void Select() => EditorGUIUtility.PingObject(ScenesStorage.Instance);

        private void OnEnable()
        {
            ReloadScenesList();
            UpdateActiveScene();
        }
        
        public void OnGUI()
        {
            UpdateActiveScene();
            if (_needAssignButtonStyles) UpdateButtonStyles();
            DrawGui(position.width);
        }

        private void UpdateButtonStyles()
        {
            if (_buttonStyleInactive == null)
            {
                _buttonStyleInactive = new GUIStyle(EditorStyles.toolbarButton);
                _buttonStyleInactive.alignment = TextAnchor.MiddleLeft;
            }
            if (_buttonStyleActive == null)
            {
                _buttonStyleActive = new GUIStyle(EditorStyles.toolbarButton);
                _buttonStyleActive.alignment = TextAnchor.MiddleLeft;
                _buttonStyleActive.normal.textColor = Color.green;
            }

            _needAssignButtonStyles = false;
        }
        
        private void ReloadScenesList()
        {
            _buildScenes = new List<SceneAsset>();
            foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            {
                if (s.enabled && s.path != "")
                {
                    _buildScenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path));
                }
            }
        }
        
        private void UpdateActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene != _activeScene)
            {
                _activeScene = activeScene;
                _activeSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);
            }
        }

        private bool DrawSceneButton(SceneAsset scene, bool isGameNotPlaying, bool isAdded, GUILayoutOption width)
        {
            if (scene == _activeSceneAsset)
            {
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("@", _buttonStyleInactive, MiniButtonWidth))
                {
                    EditorGUIUtility.PingObject(scene);
                }

                GUILayout.Label(scene.name, _buttonStyleActive, width);
                if (isGameNotPlaying && GUILayout.Button("▶", _buttonStyleInactive, MiniButtonWidth)) EditorApplication.isPlaying = true;
                if (isAdded && GUILayout.Button("X", _buttonStyleInactive, MiniButtonWidth))
                {
                    return true;
                }
                GUILayout.EndHorizontal();
                return false;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("@", _buttonStyleInactive, MiniButtonWidth)) EditorGUIUtility.PingObject(scene);


            if (GUILayout.Button(scene.name, _buttonStyleInactive, width))
            {
                if (isGameNotPlaying) AssetDatabase.OpenAsset(scene);
                else SceneManager.LoadScene(scene.name);
            }

            if (isGameNotPlaying && GUILayout.Button("▶", _buttonStyleInactive, MiniButtonWidth))
            {
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
                EditorApplication.isPlaying = true;
            }

            if (isAdded && GUILayout.Button("X", _buttonStyleInactive, MiniButtonWidth))
            {
                return true;
            }

            GUILayout.EndHorizontal();
            return false;
        }
        
        private void DrawGui(float totalWidth)
        {
            if (GUILayout.Button("Refresh List"))
            {
                ReloadScenesList();
                return;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false, GUILayout.Width(totalWidth));
            bool isGameNotPlaying = !Application.isPlaying;

            GUILayoutOption megaWidth;
            if (isGameNotPlaying) megaWidth = GUILayout.Width(totalWidth - 78);
            else megaWidth = GUILayout.Width(totalWidth - 53);

            if (_buildScenes.Count != 0)
            {
                EditorGUILayout.LabelField("Build Scenes");
                foreach (SceneAsset scene in _buildScenes)
                {
                    if (scene == null) continue;
                    DrawSceneButton(scene, isGameNotPlaying, false, megaWidth);
                }
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Added Scenes");
            
            SceneAsset asset = (SceneAsset)EditorGUILayout.ObjectField(null, typeof(SceneAsset), false, GUILayout.Width(totalWidth - 20));
            ScenesStorage storage = ScenesStorage.Instance;
            
            if (asset != null)
            {
                storage.Add(asset);
            }
            
            
            if (storage.Count == 0)
            {
                GUILayout.EndScrollView();
                return;
            }

            if (isGameNotPlaying) megaWidth = GUILayout.Width(totalWidth - 103);
            else megaWidth = GUILayout.Width(totalWidth - 53);
            foreach (SceneAsset scene in storage)
            {
                if (scene == null)
                {
                    storage.Remove(scene);
                    break;
                }
                bool shouldDelete = DrawSceneButton(scene, isGameNotPlaying, true, megaWidth);
                if (shouldDelete)
                {
                    storage.Remove(scene);
                    break;
                }
            }
            GUILayout.EndScrollView();
        }
    }
}