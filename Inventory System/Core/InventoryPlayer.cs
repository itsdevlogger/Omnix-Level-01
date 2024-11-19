using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryPlayer : MonoBehaviour
    {
        public static InventoryPlayer Instance { get; private set; }
        private static HashSet<InventoryItem> _itemsToRegister = new HashSet<InventoryItem>();


        [SerializeField]
        [Tooltip("IDs of the items player will be carrying when the scene loads")]
        private string[] _inventory;

        private readonly Dictionary<string, InventoryItem> _ownedItems = new ();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                foreach (var item in _itemsToRegister)
                {
                    RegisterItem(item);
                }
                _itemsToRegister = null;
            }
            else
            {
                Debug.LogError($"There are multiple {nameof(InventoryPlayer)}s in this scene.");
                Destroy(gameObject);
            }
        }

        public void UseItem(InventoryItem item)
        {
            if (item == null) return;

            var result = item.TryUse(this);
            if (result.IsError)
                Debug.LogError($"Error using item: {result.ErrorMessage}");
        }

        public void PickupItem(InventoryItem item)
        {
            if (item == null) return;
            
            if (_ownedItems.TryGetValue(item.ID, out var existingItem) && existingItem != null)
            {
                if (existingItem != item)
                {
                    Debug.LogError($"Found multiple items with same id: {item.ID}. Click to ping first.", item);
                    Debug.LogError($"Found multiple items with same id: {item.ID}. Click to ping second.", existingItem);
                }

                return;
            }

            var result = item.TryPickup(this);
            if (result.IsSuccess)
                _ownedItems.Add(item.ID, item);
        }

        public void DropItem(InventoryItem item)
        {
            if (item == null) return;

            item.Drop(this);
            _ownedItems.Remove(item.ID);
        }

        public static void RegisterItem(InventoryItem item)
        {
            if (item == null) return;
           
            if (Instance == null)
            {
                _itemsToRegister.Add(item);
                return;
            }

            if (item.IsPicked)
            {
                Instance._ownedItems.Add(item.ID, item);
                return;
            }

            if (Instance._inventory.Contains(item.ID))
            {
                Instance._ownedItems.Add(item.ID, item);
                item.SetPickedFlag();
            }
        }

        public bool HasItemInInventory(string keyId)
        {
            return _ownedItems.ContainsKey(keyId);
        }
    }
}