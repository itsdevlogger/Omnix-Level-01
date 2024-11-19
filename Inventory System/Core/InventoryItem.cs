using UnityEngine;
using SaveSystem;

namespace InventorySystem
{
    public abstract class InventoryItem : MonoBehaviour
    {
        #region Properties
        [field: SerializeField]
        public string ID { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Name to be shown to the player")]
        public string DisplayName { get; private set; }

        [SerializeField]
        [Tooltip("How many times this item can be used? -ve means infinite times, 0 means cant be used.")]
        private int _initialAmount = -1;

        [SerializeField] private bool _autoSave;

        /// <summary>
        /// How many times this object is used so far?
        /// </summary>
        public int UsedCount { get; protected set; } = 0;

        /// <summary>
        /// How many times this item can be used? -ve means infinite times, 0 means cant be used.
        /// </summary>
        public int AmountLeft { get; protected set; } = 0;

        /// <summary>
        /// Is this item inside any player's inventory?
        /// </summary>
        public bool IsPicked { get; private set; }
        #endregion

        #region Abstract
        protected abstract void OnPicked(InventoryPlayer player);
        protected abstract void OnDropped(InventoryPlayer player);
        protected abstract void OnUse(InventoryPlayer player);
        #endregion

        #region Unity Callbacks
        protected virtual void Start()
        {
            if (_autoSave) Deserialize(new BinaryDeserializer(ID));
            else Deserialize(null);
            InventoryPlayer.RegisterItem(this);
        }

        protected virtual void OnDestroy()
        {
            if (_autoSave)
            {
                var ser = new BinarySerializer();
                Serialize(ser);
                ser.Save(ID);
            }
        }
        
        protected virtual void Reset()
        {
            ID = SirHe.GetUniqueId();
        }
        #endregion

        #region Functionaly
        protected virtual UsageResult CanBeUsed(InventoryPlayer player)
        {
            if (IsPicked == false) return new UsageResult("Item not owned");
            if (AmountLeft == 0) return new UsageResult("Not Enough Left");
            return new UsageResult();
        }

        protected virtual UsageResult CanBePicked(InventoryPlayer player)
        {
            return new UsageResult();
        }
        
        /// <summary>
        /// Save the item data to player prefs.
        /// Order is cruitial.
        /// </summary>
        protected virtual void Serialize(BinarySerializer serializer)
        {
            serializer.WriteInt(_initialAmount);
            serializer.WriteBool(IsPicked);
        }

        /// <summary>
        /// Load the item data from player prefs.
        /// Order must be same as Serialize.
        /// i.e. If 1st property to be serialized, must be the first property to get deserialized.
        /// </summary>
        protected virtual void Deserialize(BinaryDeserializer serializer)
        {
            if (serializer == null)
            {
                AmountLeft = _initialAmount;
                IsPicked = false;
                return;
            }

            AmountLeft = serializer.ReadInt(_initialAmount);
            IsPicked = serializer.ReadBool(false);
        }
        
        public UsageResult TryUse(InventoryPlayer player)
        {
            var result = CanBeUsed(player);
            if (result.IsError) return result;

            UsedCount++;
            if (AmountLeft > 0) AmountLeft--;
            OnUse(player);
            return result;
        }

        public UsageResult TryPickup(InventoryPlayer player)
        {
            var result = CanBePicked(player);
            if (result.IsError) return result;

            IsPicked = true;
            OnPicked(player);
            return result;
        }

        public void Drop(InventoryPlayer player)
        {
            IsPicked = false;
            OnDropped(player);
        }

        internal void SetPickedFlag()
        {
            IsPicked = true;
        }
        #endregion
    }
}