using System.Collections;
using Omnix.CharaCon.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omnix.CharaCon.InteractionSystem
{
    public class Door : InteractableBase
    {
        // @formatter:off
        [Space, Header("References")]
        [SerializeField] private Transform _targetObject;
        [SerializeField] private Transform _openedPosition;
        [SerializeField] private Transform _closedPosition;

        [Space, Header("Settings")] 
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private GameObjectAttributes _attributes;
        [SerializeField] private float _autoCloseDelay;
        // @formatter:on

        public override void OnInteract(AgentInteraction agentInteraction)
        {
            _attributes.Copy(_openedPosition, _targetObject, _duration, _curve, () => StartCoroutine(AutoClose(agentInteraction)));
        }

        public override void OnUnInteract(AgentInteraction agentInteraction)
        {
            _attributes.Copy(_closedPosition, _targetObject, _duration, _curve);
        }

        private IEnumerator AutoClose(AgentInteraction agentInteraction)
        {
            if (_autoCloseDelay > 0) yield return new WaitForSeconds(_autoCloseDelay);
            agentInteraction.EndInteraction();
        }

        private void Reset()
        {
            #if UNITY_EDITOR
            _duration = 2f;
            _autoCloseDelay = 1f;
            _targetObject = transform;
            _attributes = GameObjectAttributes.LocalPosition | GameObjectAttributes.LocalRotation | GameObjectAttributes.Scale;
            _openedPosition = new GameObject("Door Opened").transform;
            _closedPosition = new GameObject("Door Closed").transform;
            _curve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe(0f, 0f),
                new Keyframe(1f, 1f),
            });

            
            Transform parent = _targetObject.parent;
            _openedPosition.SetParent(parent);
            _openedPosition.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _openedPosition.localScale = Vector3.one;
            
            _closedPosition.SetParent(parent);
            _closedPosition.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _closedPosition.localScale = Vector3.one;

            bool isOpened = EditorUtility.DisplayDialog("Door Current Position", "Select the current position of the door", "Opened", "Closed");
            _attributes.Copy(transform, isOpened ? _openedPosition : _closedPosition);
            #endif
        }
    }
}