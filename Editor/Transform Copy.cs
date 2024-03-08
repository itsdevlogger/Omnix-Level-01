using UnityEditor;
using UnityEngine;

namespace Omnix.Editor.CopyPasteHelpers
{
    public interface ISourceHolder
    {
        public void Paste(Transform target);
    }

    public class TransformHolder : ISourceHolder
    {
        private readonly Vector3 _position;
        private readonly Quaternion _rotation;
        private readonly Vector3 _localScale;


        public TransformHolder(Transform source)
        {
            _position = source.position;
            _rotation = source.rotation;
            _localScale = source.localScale;
        }

        public void Paste(Transform target)
        {
            if (target is RectTransform rect)
            {
                Undo.RecordObject(rect, $"Paste Transform {target.name}");
                rect.position = _position;
                rect.rotation = _rotation;
                rect.localScale = _localScale;
                EditorUtility.SetDirty(rect);
            }
            else
            {
                Undo.RecordObject(target, $"Paste Transform {target.name}");
                target.position = _position;
                target.rotation = _rotation;
                target.localScale = _localScale;
                EditorUtility.SetDirty(target);
            }
            
        }
    }

    public class RectTransformHolder : ISourceHolder
    {
        private readonly Vector2 _pivot;
        private readonly Vector2 _anchorMax;
        private readonly Vector2 _anchorMin;
        private readonly Vector2 _anchoredPosition;
        private readonly Vector2 _sizeDelta;
        
        private readonly Vector3 _position;
        private readonly Quaternion _rotation;
        private readonly Vector3 _localScale;

        public RectTransformHolder(RectTransform target)
        {
            _pivot = target.pivot;
            _anchorMax = target.anchorMax;
            _anchorMin = target.anchorMin;
            _anchoredPosition = target.anchoredPosition;
            _sizeDelta = target.sizeDelta;
            _position = target.position;
            _rotation = target.rotation;
            _localScale = target.localScale;
        }

        public void Paste(Transform target)
        {
            if (target is RectTransform rect)
            {
                Undo.RecordObject(rect, $"Paste RectTransform {target.name}");
                rect.pivot = _pivot;
                rect.anchorMax = _anchorMax;
                rect.anchorMin = _anchorMin;
                rect.anchoredPosition = _anchoredPosition;
                rect.sizeDelta = _sizeDelta;
                rect.rotation = _rotation;
                rect.localScale = _localScale;
                EditorUtility.SetDirty(rect);
            }
            else
            {
                Undo.RecordObject(target, $"Paste Transform {target.name}");
                target.position = _position;
                target.rotation = _rotation;
                target.localScale = _localScale;
                EditorUtility.SetDirty(target);
            }
        }
    }
    
    public static class CopyPasteHelpers
    {
        private const string COPY_TRANSFORM = "Copy Transform";
        private const string PASTE_TRANSFORM = "Paste Transform";

        private static ISourceHolder copied;

        [MenuItem(OmnixMenu.SELECT_MENU + COPY_TRANSFORM + " &C")]
        [MenuItem(OmnixMenu.OBJECT_MENU + COPY_TRANSFORM)]
        private static void CopyTransform()
        {
            Transform source = Selection.activeTransform;
            if (source == null) return;
            
            if (source.TryGetComponent(out RectTransform sourceRect))
            {
                copied = new RectTransformHolder(sourceRect);
            }
            else
            {
                copied = new TransformHolder(source);
            }
        }

        [MenuItem(OmnixMenu.SELECT_MENU + COPY_TRANSFORM, true)]
        [MenuItem(OmnixMenu.OBJECT_MENU + COPY_TRANSFORM, true)]
        public static bool IsSingleObjectSelected() => Selection.activeGameObject != null && Selection.gameObjects.Length == 1;

        
        [MenuItem(OmnixMenu.SELECT_MENU + PASTE_TRANSFORM + " &V")]
        [MenuItem(OmnixMenu.OBJECT_MENU + PASTE_TRANSFORM)]
        private static void PasteTransform()
        {
            Transform[] targets = Selection.transforms;
            if (targets != null && targets.Length > 0 && copied != null)
            {
                OmnixMenu.WrapInUndo("Paste RectTransform", targets, copied.Paste);
            }
        }
    }
}