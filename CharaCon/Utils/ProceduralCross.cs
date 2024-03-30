using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omnix.CharaCon.Utils
{
    /// <summary> A Unity component for creating a procedural cross. </summary>
    public class ProceduralCross : MonoBehaviour
    {
        [SerializeField] private Image _centerImage;
        [SerializeField] private Image _leftImage;
        [SerializeField] private Image _rightImage;
        [SerializeField] private Image _upImage;
        [SerializeField] private Image _downImage;
        [SerializeField] private float _centerSize;
        [SerializeField] private float _armLength;
        [SerializeField] private float _armThickness;
        [SerializeField] private float _armOffset;
        [SerializeField] private bool _showArms;
        [SerializeField] private bool _showCenter;
        [SerializeField] private Color _centerColor;
        [SerializeField] private Color _armColor;

        private RectTransform _centerRect;
        private RectTransform _leftRect;
        private RectTransform _rightRect;
        private RectTransform _upRect;
        private RectTransform _downRect;

        /// <summary> Size of the center part of the cross. </summary>
        public float CenterSize
        {
            get => _centerSize;
            set
            {
                if (_centerSize < 0) return;
                _centerSize = value;
                _centerRect.sizeDelta = Vector2.one * _centerSize;
                SetArmPosition(_armOffset);
            }
        }

        /// <summary> Whether the center dot of the cross is visible. </summary>
        public bool ShowCenter
        {
            get => _showCenter;
            set
            {
                _centerRect.gameObject.SetActive(value);
                _showCenter = value;
            }
        }


        /// <summary> Whether the arms of the cross are visible. </summary>
        public bool ShowArm
        {
            get => _showArms;
            set => SetArmActive(value);
        }

        /// <summary> Length of the cross arms. </summary>
        public float ArmLength
        {
            get => _armLength;
            set => SetArmSize(value, _armThickness);
        }

        /// <summary> Thickness of the cross arms. </summary>
        public float ArmThickness
        {
            get => _armThickness;
            set => SetArmSize(_armLength, value);
        }

        /// <summary> Offset of the cross arms from the center. </summary>
        public float ArmOffset
        {
            get => _armOffset;
            set => SetArmPosition(value);
        }

        /// <summary> Color of the cross arms. </summary>
        public Color ArmColor
        {
            get => _armColor;
            set => SetArmColors(value);
        }

        /// <summary> Vector2 with x-component representing Length & y-component representing ArmThickness. Its faster to use this if you want to set both. </summary>
        public Vector2 ArmSize
        {
            get => new Vector2(_armLength, _armThickness);
            set => SetArmSize(value.x, value.y);
        }

        /// <summary> Color of the center part of the cross. </summary>
        public Color CenterColor
        {
            get => _centerColor;
            set
            {
                _centerColor = value;
                _centerImage.color = value;
            }
        }


        #if UNITY_EDITOR
        /// <summary> Called when the component is added in the Unity Editor. Initializes default values and settings. </summary>
        private void Reset()
        {
            _centerImage = new GameObject("center").AddComponent<Image>();
            _leftImage = new GameObject("left").AddComponent<Image>();
            _rightImage = new GameObject("right").AddComponent<Image>();
            _upImage = new GameObject("up").AddComponent<Image>();
            _downImage = new GameObject("down").AddComponent<Image>();

            _centerImage.transform.SetParent(transform);
            _leftImage.transform.SetParent(transform);
            _rightImage.transform.SetParent(transform);
            _upImage.transform.SetParent(transform);
            _downImage.transform.SetParent(transform);

            _centerSize = 7f;
            _armLength = 15f;
            _armThickness = 7f;
            _armOffset = 5f;
            _showArms = true;
            _showCenter = true;
            _centerColor = Color.white;
            _armColor = Color.white;

            ValidateRectTransforms();
            TotalRefresh();
        }
        #endif

        /// <summary>  </summary>
        private void Awake()
        {
            ValidateRectTransforms();
            TotalRefresh();
        }

        /// <summary> Makes sure that all rectTransforms are set properly. </summary>
        public void ValidateRectTransforms()
        {
            _centerRect = _centerImage.GetComponent<RectTransform>();
            _leftRect = _leftImage.GetComponent<RectTransform>();
            _rightRect = _rightImage.GetComponent<RectTransform>();
            _upRect = _upImage.GetComponent<RectTransform>();
            _downRect = _downImage.GetComponent<RectTransform>();

            var half = new Vector2(0.5f, 0.5f);
            _centerRect.anchorMin = half;
            _centerRect.anchorMax = half;
            _centerRect.anchoredPosition = Vector2.zero;
            _centerRect.localScale = Vector3.one;

            _leftRect.anchorMin = half;
            _leftRect.anchorMax = half;
            _leftRect.localScale = Vector3.one;

            _rightRect.anchorMin = half;
            _rightRect.anchorMax = half;
            _rightRect.localScale = Vector3.one;

            _upRect.anchorMin = half;
            _upRect.anchorMax = half;
            _upRect.localScale = Vector3.one;

            _downRect.anchorMin = half;
            _downRect.anchorMax = half;
            _downRect.localScale = Vector3.one;
        }

        /// <summary> Sets the color of the cross arms. </summary>
        private void SetArmColors(Color color)
        {
            _armColor = color;
            _leftImage.color = _armColor;
            _rightImage.color = _armColor;
            _upImage.color = _armColor;
            _downImage.color = _armColor;
        }

        /// <summary> Sets the position of the cross arms based on the given offset. </summary>
        private void SetArmPosition(float offset)
        {
            if (offset < 0) return;

            _armOffset = offset;
            float posOffset = (_centerSize + _armLength) * 0.5f + _armOffset;
            _leftRect.anchoredPosition = new Vector2(-posOffset, 0f);
            _rightRect.anchoredPosition = new Vector2(posOffset, 0f);
            _upRect.anchoredPosition = new Vector2(0f, posOffset);
            _downRect.anchoredPosition = new Vector2(0f, -posOffset);
        }

        /// <summary> Sets whether the arms of the cross are visible. </summary>
        private void SetArmActive(bool value)
        {
            _leftImage.gameObject.SetActive(_showArms);
            _rightImage.gameObject.SetActive(_showArms);
            _upImage.gameObject.SetActive(_showArms);
            _downImage.gameObject.SetActive(_showArms);
            _showArms = value;
        }

        /// <summary> Sets length & thickness of the arms  </summary>
        public void SetArmSize(float length, float thickness)
        {
            if (length <= 0 || thickness <= 0) return;

            _armLength = length;
            _armThickness = thickness;
            _leftRect.sizeDelta = new Vector2(_armLength, _armThickness);
            _rightRect.sizeDelta = new Vector2(_armLength, _armThickness);
            _upRect.sizeDelta = new Vector2(_armThickness, _armLength);
            _downRect.sizeDelta = new Vector2(_armThickness, _armLength);
            SetArmPosition(_armOffset);
        }

        /// <summary> Refreshes all the attributes of the cursor </summary>
        public void TotalRefresh()
        {
            if (_showCenter)
            {
                _centerRect.gameObject.SetActive(true);
                _centerImage.color = _centerColor;
                _centerRect.sizeDelta = Vector2.one * _centerSize;
            }
            else
            {
                _centerRect.gameObject.SetActive(false);
            }

            SetArmActive(_showArms);

            if (_showArms)
            {
                SetArmColors(_armColor);
                SetArmSize(_armLength, _armThickness); // Will call SetArmPosition
            }
        }

        /// <summary> Refreshes all the attributes of the cursor </summary>
        public void TotalRefresh(bool showArms, bool showCenter, float centerSize = -1f, float armLength = -1f, float armThickness = -1f, float armOffset = -1f)
        {
            _showArms = showArms;
            _showCenter = showCenter;
            _centerSize = centerSize;
            _armLength = armLength;
            _armThickness = armThickness;
            _armOffset = armOffset;
            TotalRefresh();
        }

        /// <summary> Refreshes all the attributes of the cursor </summary>
        public void TotalRefresh(bool showArms, bool showCenter, Color centerColor, Color armColor, float centerSize = -1f, float armLength = -1f, float armThickness = -1f, float armOffset = -1f)
        {
            _showArms = showArms;
            _showCenter = showCenter;
            _centerSize = centerSize;
            _armLength = armLength;
            _armThickness = armThickness;
            _armOffset = armOffset;
            _centerColor = centerColor;
            _armColor = armColor;
            TotalRefresh();
        }
    }


    #if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ProceduralCross))]
    public class ProceduralCrossEditor : Editor
    {
        private SerializedProperty _centerImage;
        private SerializedProperty _leftImage;
        private SerializedProperty _rightImage;
        private SerializedProperty _upImage;
        private SerializedProperty _downImage;
        private SerializedProperty _centerSize;
        private SerializedProperty _armLength;
        private SerializedProperty _armThickness;
        private SerializedProperty _armOffset;
        private SerializedProperty _showArms;
        private SerializedProperty _showCenter;
        private SerializedProperty _centerColor;
        private SerializedProperty _armColor;


        private void OnEnable()
        {
            _centerImage = serializedObject.FindProperty("_centerImage");
            _leftImage = serializedObject.FindProperty("_leftImage");
            _rightImage = serializedObject.FindProperty("_rightImage");
            _upImage = serializedObject.FindProperty("_upImage");
            _downImage = serializedObject.FindProperty("_downImage");

            _showCenter = serializedObject.FindProperty("_showCenter");
            _centerSize = serializedObject.FindProperty("_centerSize");
            _centerColor = serializedObject.FindProperty("_centerColor");

            _showArms = serializedObject.FindProperty("_showArms");
            _armLength = serializedObject.FindProperty("_armLength");
            _armThickness = serializedObject.FindProperty("_armThickness");
            _armOffset = serializedObject.FindProperty("_armOffset");
            _armColor = serializedObject.FindProperty("_armColor");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            if (StylizeBoolField(_showArms))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_armLength);
                EditorGUILayout.PropertyField(_armThickness);
                EditorGUILayout.PropertyField(_armOffset);
                EditorGUILayout.PropertyField(_armColor);
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (StylizeBoolField(_showCenter))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_centerSize);
                EditorGUILayout.PropertyField(_centerColor);
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_centerImage);
            EditorGUILayout.PropertyField(_leftImage);
            EditorGUILayout.PropertyField(_rightImage);
            EditorGUILayout.PropertyField(_upImage);
            EditorGUILayout.PropertyField(_downImage);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                var cross = ((ProceduralCross)target);
                cross.ValidateRectTransforms();
                cross.TotalRefresh();
                EditorUtility.SetDirty(cross.gameObject);
            }
        }

        private static bool StylizeBoolField(SerializedProperty property)
        {
            Color guiColor = GUI.color;
            bool current = property.boolValue;
            GUI.color = current ? Color.green : Color.gray;
            if (GUILayout.Button(property.displayName, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.25f)))
            {
                property.boolValue = !current;
                GUI.color = guiColor;
                return !current;
            }

            GUI.color = guiColor;
            return current;
        }
    }
    #endif
}