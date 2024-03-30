using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Omnix.Utils.EditorUtils;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SceneAsset = UnityEditor.SceneAsset;

namespace Omnix.Editor
{
    public class ScenesWindow : EditorWindow
    {
        private List<SceneAsset> _buildScenes;
        private Vector2 _scrollPos;
        private SceneAsset _activeSceneAsset;
        private Scene _activeScene;

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
            DrawGui(position.width);
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

        private bool DrawSceneButton(SceneAsset scene, bool isAdded)
        {
            int numberOfMiniButtons = 1;
            if (!Application.isPlaying) numberOfMiniButtons++;
            if (isAdded) numberOfMiniButtons++;
            
            if (scene == _activeSceneAsset)
            {
                Color guiColor = GUI.color;
                GUI.color = Color.green;
                ButtonsRow.BeginRow(numberOfMiniButtons, 10f);
                if (ButtonsRow.MiniButton("@")) EditorGUIUtility.PingObject(scene);
                ButtonsRow.BigButton(scene.name);
                if (!Application.isPlaying && ButtonsRow.MiniButton("▶")) EditorApplication.isPlaying = true;
                bool removeScene = isAdded && ButtonsRow.MiniButton("X");
                GUI.color = guiColor;
                return removeScene;
            }

            ButtonsRow.BeginRow(numberOfMiniButtons, 10f);
            if (ButtonsRow.MiniButton("@")) EditorGUIUtility.PingObject(scene);
            if (ButtonsRow.BigButton(scene.name))
            {
                if (Application.isPlaying) SceneManager.LoadScene(scene.name);
                else AssetDatabase.OpenAsset(scene);
            }

            if (!Application.isPlaying && ButtonsRow.MiniButton("▶"))
            {
                AssetDatabase.OpenAsset(scene);
                EditorApplication.isPlaying = true;
            }
            return isAdded && ButtonsRow.MiniButton("X");
        }

        private void DrawGui(float totalWidth)
        {
            if (ButtonsRow.HeaderButton("Refresh List"))
            {
                ReloadScenesList();
                return;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false, GUILayout.Width(totalWidth));
            
            if (_buildScenes.Count != 0)
            {
                EditorGUILayout.LabelField("Build Scenes");
                foreach (SceneAsset scene in _buildScenes)
                {
                    if (scene == null) continue;
                    DrawSceneButton(scene, false);
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

            foreach (SceneAsset scene in storage)
            {
                if (scene == null)
                {
                    storage.Remove(scene);
                    break;
                }
                bool shouldDelete = DrawSceneButton(scene, true);
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