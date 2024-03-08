using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThemedUi
{
    public class ThemedUiStorage : EditorStorage<ThemedUiStorage>
    {
        [SerializeField] private List<GameObject> _images;
        [SerializeField] private List<GameObject> _buttons;
        [SerializeField] private List<GameObject> _toggles;
        [SerializeField] private List<GameObject> _texts;
        [SerializeField] private List<GameObject> _scrollviews;
        [SerializeField] private List<GameObject> _dropdowns;
        [SerializeField] private List<GameObject> _inputFields;
        [SerializeField] private List<GameObject> _panels;
        [SerializeField] private List<GameObject> _extras;

        protected override void Init()
        {
        }
        
        private static bool Check(bool displayDialogue = true)
        {
            if (Instance == null)
            {
                if (displayDialogue) EditorUtility.DisplayDialog("Error", "Cannot find the storage", "Okay");
                return false;
            }

            return true;
        }

        [MenuItem("Themed UI/Select Storage")]
        private static void SelectStorage()
        {
            if (Check())
            {
                Selection.activeObject = Instance;
                EditorGUIUtility.PingObject(Instance);
            }
        }

        #region Create
        private static Transform GetCanvas()
        {
            Transform active = Selection.activeTransform;
            if (active != null)
            {
                if (active.TryGetComponent(out Canvas _)) return active;
                if (active.GetComponentInParent<Canvas>() != null) return active;
            }

            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null) canvas = new GameObject("Canvas").AddComponent<Canvas>();
            return canvas.transform;
        }

        
        private static void InitFrom(List<GameObject> array, string myType)
        {
            if (array.Count == 0)
            {
                bool shouldSelect = EditorUtility.DisplayDialog("Error", $"No {myType} defined in the storage", "Select Storage", "Okay");
                if (shouldSelect) SelectStorage();
            }
            else if (array.Count == 1)
            {
                Transform canvas = GetCanvas();
                Selection.activeGameObject = Instantiate(array[0], canvas);
            }
            else
            {
                Transform canvas = GetCanvas();
                ThemedUiInstance.StartWith(array, canvas);
            }
        }

        
        [MenuItem("GameObject/Themed UI/Image", priority = 8)]
        private static void CreateImages()
        {
            if (Check())
            {
                InitFrom(Instance._images, "Image");
            }
        }

        [MenuItem("GameObject/Themed UI/Button", priority = 8)]
        private static void CreateButtons()
        {
            if (Check())
            {
                InitFrom(Instance._buttons, "Button");
            }
        }

        [MenuItem("GameObject/Themed UI/Toggle", priority = 8)]
        private static void CreateToggles()
        {
            if (Check())
            {
                InitFrom(Instance._toggles, "Toggle");
            }
        }

        [MenuItem("GameObject/Themed UI/Text", priority = 8)]
        private static void CreateTexts()
        {
            if (Check())
            {
                InitFrom(Instance._texts, "Text");
            }
        }

        [MenuItem("GameObject/Themed UI/Scrollview", priority = 8)]
        private static void CreateScrollviews()
        {
            if (Check())
            {
                InitFrom(Instance._scrollviews, "Scrollview");
            }
        }

        [MenuItem("GameObject/Themed UI/Dropdown", priority = 8)]
        private static void CreateDropdowns()
        {
            if (Check())
            {
                InitFrom(Instance._dropdowns, "Dropdown");
            }
        }

        [MenuItem("GameObject/Themed UI/InputField", priority = 8)]
        private static void CreateInputFields()
        {
            if (Check())
            {
                InitFrom(Instance._inputFields, "InputField");
            }
        }

        [MenuItem("GameObject/Themed UI/Panel", priority = 8)]
        private static void CreatePanels()
        {
            if (Check())
            {
                InitFrom(Instance._panels, "Panel");
            }
        }

        [MenuItem("GameObject/Themed UI/Extra", priority = 9)]
        private static void CreateExtras()
        {
            if (Check())
            {
                InitFrom(Instance._extras, "Extra");
            }
        }
        #endregion

        #region Check Create
        private static void RemoveEmptyValues(List<GameObject> array)
        {
            if (array == null || array.Count == 0) return;
            int count = array.RemoveAll(v => v == null);
            if (count >= 1) EditorUtility.SetDirty(Instance);
        }
        
        private static bool CheckCreate(List<GameObject> array)
        {
            if (array == null) return false;
            RemoveEmptyValues(array);
            return array.Count > 0;
        }
        
        [MenuItem("GameObject/Themed UI/Image", isValidateFunction: true)]
        private static bool CheckCreateImages() => Check(false) && CheckCreate(Instance._images);

        [MenuItem("GameObject/Themed UI/Button", isValidateFunction: true)]
        private static bool CheckCreateButtons() => Check(false) && CheckCreate(Instance._buttons);

        [MenuItem("GameObject/Themed UI/Toggle", isValidateFunction: true)]
        private static bool CheckCreateToggles() => Check(false) && CheckCreate(Instance._toggles);

        [MenuItem("GameObject/Themed UI/Text", isValidateFunction: true)]
        private static bool CheckCreateTexts() => Check(false) && CheckCreate(Instance._texts);

        [MenuItem("GameObject/Themed UI/Scrollview", isValidateFunction: true)]
        private static bool CheckCreateScrollviews() => Check(false) && CheckCreate(Instance._scrollviews);

        [MenuItem("GameObject/Themed UI/Dropdown", isValidateFunction: true)]
        private static bool CheckCreateDropdowns() => Check(false) && CheckCreate(Instance._dropdowns);

        [MenuItem("GameObject/Themed UI/InputField", isValidateFunction: true)]
        private static bool CheckCreateInputFields() => Check(false) && CheckCreate(Instance._inputFields);

        [MenuItem("GameObject/Themed UI/Panel", isValidateFunction: true)]
        private static bool CheckCreatePanels() => Check(false) && CheckCreate(Instance._panels);

        [MenuItem("GameObject/Themed UI/Extra", isValidateFunction: true)]
        private static bool CheckCreateExtras() => Check(false) && CheckCreate(Instance._extras);
        #endregion
        
        #region Send To
        private static void SendItemTo(List<GameObject> array, GameObject item)
        {
            if (item.TryGetComponent(out RectTransform _) == false)
            {
                EditorUtility.DisplayDialog("Error", "Item does not contain RectTransform", "Okay");
                return;
            }

            if (array.Contains(item))
            {
                return;
            }

            array.Add(item);
            EditorUtility.SetDirty(Instance);
        }

        private static void SendSelectionTo(List<GameObject> array)
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0)
            {
                EditorUtility.DisplayDialog("Nothing Selected", "Select at least one item", "Okay");
                return;
            }

            foreach (GameObject gameObject in selected)
            {
                SendItemTo(array, gameObject);
            }
        }
        
        [MenuItem("Assets/Add To Themed UI/Image")]
        private static void SendToImages()
        {
            if (Check())
            {
                SendSelectionTo(Instance._images);
            }
        }

        [MenuItem("Assets/Add To Themed UI/Button")]
        private static void SendToButtons()
        {
            if (Check())
            {
                SendSelectionTo(Instance._buttons);
            }
        }

        [MenuItem("Assets/Add To Themed UI/Toggle")]
        private static void SendToToggles()
        {
            if (Check())
            {
                SendSelectionTo(Instance._toggles);
            }
        }

        [MenuItem("Assets/Add To Themed UI/Text")]
        private static void SendToTexts()
        {
            if (Check())
            {
                SendSelectionTo(Instance._texts);
            }
        }

        [MenuItem("Assets/Add To Themed UI/Scrollview")]
        private static void SendToScrollviews()
        {
            if (Check())
            {
                SendSelectionTo(Instance._scrollviews);
            }
        }

        [MenuItem("Assets/Add To Themed UI/Dropdown")]
        private static void SendToDropdowns()
        {
            if (Check())
            {
                SendSelectionTo(Instance._dropdowns);
            }
        }

        [MenuItem("Assets/Add To Themed UI/InputField")]
        private static void SendToInputFields()
        {
            if (Check())
            {
                SendSelectionTo(Instance._inputFields);
            }
        }

        [MenuItem("Assets/Add To Themed UI/Panel")]
        private static void SendToPanels()
        {
            if (Check())
            {
                SendSelectionTo(Instance._panels);
            }
        }

        [MenuItem("Assets/Add To Themed UI/Extra")]
        private static void SendToExtras()
        {
            if (Check())
            {
                SendSelectionTo(Instance._extras);
            }
        }
        #endregion
    }
}