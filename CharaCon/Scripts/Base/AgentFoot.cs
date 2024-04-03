using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnix.CharaCon
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public class AgentFoot : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;

        private HashSet<Transform> _parents;
        
        private void Reset()
        {
            _source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _parents = new HashSet<Transform>();
            Transform p = transform;
            while (p != null)
            {
                _parents.Add(p);
                p = p.parent;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_parents.Contains(other.transform) == false)
            {
                Debug.Log($"Play For: {other.transform}");
                _source.Play();
            }
            else
            {
                Debug.Log($"Not For: {other.transform}");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            Debug.Log($"Staying: {other}");
        }


        private void OnTriggerExit(Collider other)
        {
            Debug.Log($"Exit: {other}");
        }
    }
}