using UnityEngine;
using Random = UnityEngine.Random;

namespace Omnix.CharaCon
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundControl : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip[] _footsteps;
        [SerializeField] private AudioClip[] _land;

        private static AudioClip GetRandom(AudioClip[] array)
        {
            if (array.Length == 1) return array[0];
            return array[Random.Range(0, array.Length - 1)];
        }

        private void Reset()
        {
            _source = GetComponent<AudioSource>();
        }

        public void OnStep()
        {
            AudioClip ac = GetRandom(_footsteps);
            _source.clip = ac;
            _source.Play();
        }

        public void OnLand()
        {
            AudioClip ac = GetRandom(_land);
            _source.clip = ac;
            _source.Play();
        }
    }
}