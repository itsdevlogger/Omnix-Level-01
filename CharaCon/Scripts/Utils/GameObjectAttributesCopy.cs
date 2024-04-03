using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnix.CharaCon.Utils
{
    public class GameObjectAttributesCopy : MonoBehaviour
    {
        #region Classes
        private interface IFrameProcessor
        {
            public void Do(float t, Transform target);
        }

        private struct DoPosition : IFrameProcessor
        {
            public Vector3 start;
            public Vector3 end;
            public void Do(float t, Transform target) => target.position = Vector3.LerpUnclamped(start, end, t);
        }

        private struct DoLocalPosition : IFrameProcessor
        {
            public Vector3 start;
            public Vector3 end;
            public void Do(float t, Transform target) => target.localPosition = Vector3.LerpUnclamped(start, end, t);
        }

        private struct DoRotation : IFrameProcessor
        {
            public Quaternion start;
            public Quaternion end;
            public void Do(float t, Transform target) => target.rotation = Quaternion.LerpUnclamped(start, end, t);
        }

        private struct DoLocalRotation : IFrameProcessor
        {
            public Quaternion start;
            public Quaternion end;
            public void Do(float t, Transform target) => target.localRotation = Quaternion.LerpUnclamped(start, end, t);
        }

        private struct DoScale : IFrameProcessor
        {
            public Vector3 start;
            public Vector3 end;
            public void Do(float t, Transform target) => target.localScale = Vector3.LerpUnclamped(start, end, t);
        }
        #endregion

        #region Fields
        private GameObjectAttributes _attributes;
        private IFrameProcessor[] _processors;
        private AnimationCurve _curve;
        private Transform _source;
        private Transform _target;
        private Action _onComplete;
        private float _animationDuration;
        private float _time;
        #endregion


        private bool Copying(GameObjectAttributes atb)
        {
            return (_attributes & atb) != 0;
        }

        private void DoEverythingElse()
        {
            if (Copying(GameObjectAttributes.Parent) && _source.parent != null) _target.SetParent(_source.parent);
            if (Copying(GameObjectAttributes.Layer)) _target.gameObject.layer = _source.gameObject.layer;
            if (Copying(GameObjectAttributes.Tag)) _target.tag = _source.tag;
        }

        private void SetFrame(float t)
        {
            foreach (IFrameProcessor processor in _processors)
            {
                processor.Do(t, _target);
            }
        }
        
        private void Update()
        {
            _time += Time.deltaTime;
            if (_time < _animationDuration)
            {
                SetFrame(_curve.Evaluate(_time / _animationDuration));
            }
            else
            {
                SetFrame(1f);
                Destroy(gameObject);
                _onComplete?.Invoke();
            }
        }
        
        public void Init(GameObjectAttributes attributes, Transform source, Transform target, float animationDuration, AnimationCurve curve, Action onComplete)
        {
            _attributes = attributes;
            List<IFrameProcessor> processors = new List<IFrameProcessor>(3);
            if (Copying(GameObjectAttributes.Position)) processors.Add(new DoPosition() { start = target.position, end = source.position });
            else if (Copying(GameObjectAttributes.LocalPosition)) processors.Add(new DoLocalPosition() { start = target.localPosition, end = source.localPosition });

            if (Copying(GameObjectAttributes.Rotation)) processors.Add(new DoRotation() { start = target.rotation, end = source.rotation });
            else if (Copying(GameObjectAttributes.LocalRotation)) processors.Add(new DoLocalRotation() { start = target.localRotation, end = source.localRotation });

            if (Copying(GameObjectAttributes.Scale)) processors.Add(new DoScale() { start = target.localScale, end = source.localScale });

            // If none of position, rotation or scale is being copied
            // Then no need to animate
            if (processors.Count == 0)
            {
                attributes.Copy(source, target);
                Destroy(gameObject);
                onComplete.Invoke();
                return;
            }

            _curve = curve;
            _target = target;
            _source = source;
            _onComplete = onComplete;
            _animationDuration = animationDuration;
            _processors = new IFrameProcessor[processors.Count];
            
            int i = 0;
            foreach (IFrameProcessor processor in processors)
            {
                _processors[i] = processor;
                i++;
            }

            DoEverythingElse();
        }
    }
}