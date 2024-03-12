using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


namespace Omnix.Editor.Windows.Resources
{
    public class ResourcesWindow : EditorWindow
    {
        #region Statics

        private static string DataPathGuids => Application.persistentDataPath + "\\Guids.dat";
        private static string DataPathColors => Application.persistentDataPath + "\\GuidColors.dat";
        
        #endregion

        #region Properties
        private Vector2 scrollPos;
        #endregion

        #region Variables

        private RecWinDrawMod DrawMode;
        private float guiAlpha = 0.8f;
        public GUIStyle guiBackStyle;

        #endregion

        #region Unity Callbacks

        [MenuItem(OmnixMenu.WINDOW_MENU + "Resources")]
        private static void Init() => GetWindow(typeof(ResourcesWindow), false, "Resources").Show();

        [MenuItem(OmnixMenu.STORAGE_MENU + "Resources")]
        private static void SelectStorage() => EditorGUIUtility.PingObject(ResourcesStorage.Instance);

        
        protected virtual void OnEnable()
        {
            guiBackStyle = new GUIStyle();
            guiBackStyle.normal.background = EditorGUIUtility.whiteTexture;
            guiBackStyle.stretchWidth = true;
            guiBackStyle.margin = new RectOffset(0, 0, 7, 7);
            ResourcesStorage.Instance.UpdateAllLayers();
            SwitchToList();
        }

        protected virtual void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width));
            if (DrawMode == null) SwitchToList();
            DrawMode.Draw();
            GUILayout.EndScrollView();
            ResourcesStorage storage = ResourcesStorage.Instance;
            if (storage.isLayeredView)
            {
                if (GUILayout.Button("Switch To Simple View"))
                {
                    storage.isLayeredView = false;
                    SwitchDrawMode(new ModeSimpleResources(this));
                }    
            }
            else
            {
                if (GUILayout.Button("Switch To Layered View"))
                {
                    storage.isLayeredView = true;
                    SwitchDrawMode(new ModeLayeredResources(this));
                }
            }
        }

        #endregion

        #region DrawModes

        public void SwitchDrawMode(RecWinDrawMod newMode)
        {
            this.DrawMode = newMode;
        }

        public void SwitchToList()
        {
            if (ResourcesStorage.Instance.isLayeredView) SwitchDrawMode(new ModeLayeredResources(this));
            else SwitchDrawMode(new ModeSimpleResources(this));
        }
        
        public void StylizeGUI(int objCount, int offset, Color color)
        {
            Color restCol = GUI.color;
            GUI.color = color;
            Rect backPos = GUILayoutUtility.GetRect(GUIContent.none, guiBackStyle, GUILayout.Height(0));
            backPos.height = objCount * 19 + offset;
            GUI.DrawTexture(backPos, EditorGUIUtility.whiteTexture);
            restCol.a = guiAlpha;
            GUI.color = restCol;
        }

        private bool DrawColorButton(int i)
        {
            GUI.color = ResourcesStorage.Instance.GetColor(i);
            Rect backPos = GUILayoutUtility.GetRect(GUIContent.none, guiBackStyle, GUILayout.Height(19));
            backPos.width -= 6;
            backPos.center = new Vector2(backPos.center.x + 3, backPos.center.y);
            bool tr = GUI.Button(backPos, "");
            GUI.DrawTexture(backPos, EditorGUIUtility.whiteTexture);
            return tr;
        }
        
        public int DrawColorsPicker(int index)
        {
            Color normCol = GUI.color;


            // Selection            
            GUI.color = ResourcesStorage.Instance.GetColor(index);
            Rect backPos = GUILayoutUtility.GetRect(GUIContent.none, guiBackStyle, GUILayout.Height(57));
            GUI.DrawTexture(backPos, EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(backPos, "SELECTION");
            GUI.color = ResourcesStorage.Instance.GetColor(index);


            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < 4; i++)
                {
                    if (DrawColorButton(i))
                    {
                        GUI.color = normCol;
                        EditorGUILayout.EndHorizontal();
                        return i;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 4; i < 8; i++)
                {
                    if (DrawColorButton(i))
                    {
                        GUI.color = normCol;
                        EditorGUILayout.EndHorizontal();
                        return i;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 8; i < 12; i++)
                {
                    if (DrawColorButton(i))
                    {
                        GUI.color = normCol;
                        EditorGUILayout.EndHorizontal();
                        return i;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.color = normCol;
            return index;
        }

        #endregion

        #region Publics
        public void AddObjectToLayer(string layerName, Object selectedObject)
        {
            if (ResourcesStorage.Instance.ContainsObjectInLayer(layerName, selectedObject))
            {
                return;
            }

            // string displayName = selectedObject.name;
            // if (displayName.Length > 18) displayName = displayName.Substring(0, 15) + "...";
            ResourcesStorage.Instance.AddTo(layerName, new ObjectInfo(selectedObject.name, selectedObject));
        }
        #endregion
    }
}