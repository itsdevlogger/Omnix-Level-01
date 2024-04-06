#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Omnix.Monos
{
    public class SelectObjectInGame : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private LayerMask _layers;
        [SerializeField] private InputAction _action = new InputAction("Middle Mouse Button", InputActionType.Value, "<Mouse>/middleButton");
        
        private readonly RaycastHit[] _hits = new RaycastHit[10];

        private void OnEnable()
        {
            _action.started += UpdateSelection;
        }

        private void UpdateSelection(InputAction.CallbackContext _)
        {
            Selection.activeTransform = GetObjectToSelect(Selection.activeTransform);
        }

        private Transform GetObjectToSelect(Transform currentlyActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (currentlyActive == null)
            {
                return Physics.Raycast(ray, out RaycastHit hit, 100f, _layers) ? hit.transform : null;
            }

            int count = Physics.RaycastNonAlloc(ray, _hits, 100f, _layers);
            if (count == 0) return null;
            if (count == 1) return _hits[0].transform;

            QuickSortHitsDistance(0, count);
            var foundActive = false;
            foreach (RaycastHit hit in _hits)
            {
                if (foundActive) return hit.transform;
                if (hit.transform == currentlyActive) foundActive = true;
            }

            return _hits[0].transform;
        }

        private void QuickSortHitsDistance(int low, int high)
        {
            if (low >= high) return;

            float distance = _hits[high].distance;
            int i1 = low;
            for (int i2 = low; i2 < high; ++i2)
            {
                if (_hits[i2].distance < distance)
                {
                    (_hits[i1], _hits[i2]) = (_hits[i2], _hits[i1]);
                    ++i1;
                }
            }

            (_hits[i1], _hits[high]) = (_hits[high], _hits[i1]);
            QuickSortHitsDistance(low, i1 - 1);
            QuickSortHitsDistance(i1 + 1, high);
        }
        #endif
    }
}