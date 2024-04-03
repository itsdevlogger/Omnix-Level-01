using System;
using System.Collections.Generic;
using System.Linq;
using Omnix.CharaCon.HealthSystem;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Omnix.CharaCon.Utils
{
    public static class Extensions
    {
        /// <summary> Attempts to play a random AudioClip from the list on the provided AudioSource. </summary>
        public static void TryPlayRandom(this List<AudioClip> list, AudioSource source)
        {
            if (list.Count == 0) return;

            if (list.TryGetClip(out AudioClip clip))
            {
                source.clip = clip;
                source.Play();
            }
        }

        /// <summary> Tries to get a random AudioClip from the list </summary>
        public static bool TryGetClip(this List<AudioClip> list, out AudioClip clip)
        {
            switch (list.Count)
            {
                case 0:
                    clip = null;
                    return false;
                case 1:
                    clip = list[0];
                    return clip != null;
            }

            for (int _ = 0; _ < 3; _++)
            {
                clip = list[Random.Range(0, list.Count)];
                if (clip != null) return true;
            }

            clip = null;
            return false;
        }

        /// <summary> Sets the active state of each GameObject in the collection </summary>
        public static void SetActive(this IEnumerable<GameObject> list, bool value)
        {
            foreach (GameObject gameObject in list)
            {
                gameObject.SetActive(value);
            }
        }

        /// <summary> Spawns each GameObject in the list. </summary>
        public static void SpawnSelf(this IEnumerable<GameObject> list, Vector3 position, Quaternion rotation, Transform parent)
        {
            foreach (GameObject gameObject in list)
            {
                Object.Instantiate(gameObject, position, rotation, parent);
            }
        }

        /// <summary> Destroys each GameObject in the list immediately and clears the list. </summary>
        public static void DestroySelf(this List<GameObject> list)
        {
            foreach (GameObject gameObject in list)
            {
                Object.DestroyImmediate(gameObject);
            }

            list.Clear();
        }

        /// <summary> Applies damage to a collection of Shield objects, until the total damage is exhausted or all shields are depleted </summary>
        /// <returns> Remaining damage. </returns>
        public static float Damage(this IEnumerable<Shield> list, float totalAmount)
        {
            if (totalAmount <= 0) return totalAmount;

            foreach (Shield shield in list)
            {
                totalAmount = shield.Damage(totalAmount);
                if (totalAmount <= 0f) return 0f;
            }

            return totalAmount;
        }

        /// <summary> Calls <see cref="Shield.Init"/> method for each Shield object in the list </summary>
        public static void Init(this IEnumerable<Shield> list)
        {
            foreach (Shield shield in list)
            {
                shield.Init();
            }
        }

        /// <summary> Calls  <see cref="Shield.Restore"/> method for each Shield object in the list </summary>
        public static void Restore(this IEnumerable<Shield> list)
        {
            foreach (Shield shield in list)
            {
                shield.Restore();
            }
        }

        /// <summary> Calls <see cref="Shield.Init"/> on Shield, and adds it to the list </summary>
        public static void InitAndAdd(this List<Shield> list, Shield toAdd)
        {
            toAdd.Init();
            list.Add(toAdd);
        }

        /// <summary> Calls <see cref="Shield.Init"/> on each Shield, and adds them to the list </summary>
        public static void InitAndAdd(this List<Shield> list, IEnumerable<Shield> collection)
        {
            // Convert to list first, as its possible to have a IEnumerable function that yields different values everytime.
            List<Shield> enumerable = collection as List<Shield> ?? collection.ToList();
            enumerable.Init();
            list.AddRange(enumerable);
        }

        /// <summary> Copies the given attributes from source to target </summary>
        public static void Copy(this GameObjectAttributes attributes, Transform copyFrom, Transform copyTo)
        {
            if (Copying(GameObjectAttributes.Parent) && copyFrom.parent != null) copyTo.SetParent(copyFrom.parent);

            if (Copying(GameObjectAttributes.Position)) copyTo.position = copyFrom.position;
            else if (Copying(GameObjectAttributes.LocalPosition)) copyTo.localPosition = copyFrom.localPosition;

            if (Copying(GameObjectAttributes.Rotation)) copyTo.rotation = copyFrom.rotation;
            else if (Copying(GameObjectAttributes.LocalRotation)) copyTo.localRotation = copyFrom.localRotation;

            if (Copying(GameObjectAttributes.Scale)) copyTo.rotation = copyFrom.rotation;
            if (Copying(GameObjectAttributes.Layer)) copyTo.gameObject.layer = copyFrom.gameObject.layer;
            if (Copying(GameObjectAttributes.Tag)) copyTo.tag = copyFrom.tag;
            return;

            bool Copying(GameObjectAttributes atb) => (attributes & atb) != 0;
        }

        /// <summary> Copies the given attributes from source to target, and animates the movement </summary>
        public static void Copy(this GameObjectAttributes attributes, Transform copyFrom, Transform copyTo, float animationDuration, AnimationCurve curve = null, Action onComplete = null)
        {
            GameObject go = new GameObject("[Copy GameObjectAttributes]");
            go.AddComponent<GameObjectAttributesCopy>().Init(attributes, copyFrom, copyTo, animationDuration, curve, onComplete);
        }
    }
}