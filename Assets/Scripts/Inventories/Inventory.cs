using System;
using UnityEngine;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Inventories
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour, IJsonSaveable
    {
        // CONFIG DATA
        [Tooltip("Allowed size")]
        [SerializeField] int m_InventorySize = 16;

        // STATE
        InventorySlot[] m_Slots;

        public struct InventorySlot
        {
            public InventoryItem m_Item;
            public int m_Number;
        }

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action a_InventoryUpdated;

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }

        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        /// <summary>
        /// How many slots are in the inventory?
        /// </summary>
        public int GetSize()
        {
            return m_Slots.Length;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(InventoryItem item, int number)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            m_Slots[i].m_Item = item;
            m_Slots[i].m_Number += number;
            if (a_InventoryUpdated != null)
            {
                a_InventoryUpdated();
            }
            return true;
        }

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < m_Slots.Length; i++)
            {
                if (object.ReferenceEquals(m_Slots[i].m_Item, item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public InventoryItem GetItemInSlot(int slot)
        {
            return m_Slots[slot].m_Item;
        }

        /// <summary>
        /// Get the number of items in the given slot.
        /// </summary>
        public int GetNumberInSlot(int slot)
        {
            return m_Slots[slot].m_Number;
        }

        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int slot, int number)
        {
            m_Slots[slot].m_Number -= number;
            if (m_Slots[slot].m_Number <= 0)
            {
                m_Slots[slot].m_Number = 0;
                m_Slots[slot].m_Item = null;
            }
            if (a_InventoryUpdated != null)
            {
                a_InventoryUpdated();
            }
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            if (m_Slots[slot].m_Item != null)
            {
                return AddToFirstEmptySlot(item, number); ;
            }

            var i = FindStack(item);
            if (i >= 0)
            {
                slot = i;
            }

            m_Slots[slot].m_Item = item;
            m_Slots[slot].m_Number += number;
            if (a_InventoryUpdated != null)
            {
                a_InventoryUpdated();
            }
            return true;
        }

        // PRIVATE

        private void Awake()
        {
            m_Slots = new InventorySlot[m_InventorySize];
        }

        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            int i = FindStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }
            return i;
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < m_Slots.Length; i++)
            {
                if (m_Slots[i].m_Item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find an existing stack of this item type.
        /// </summary>
        /// <returns>-1 if no stack exists or if the item is not stackable.</returns>
        private int FindStack(InventoryItem item)
        {
            if (!item.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < m_Slots.Length; i++)
            {
                if (object.ReferenceEquals(m_Slots[i].m_Item, item))
                {
                    return i;
                }
            }
            return -1;
        }

        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int number;
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            for (int i = 0; i < m_InventorySize; i++)
            {
                if (m_Slots[i].m_Item != null)
                {
                    JObject itemState = new JObject();
                    IDictionary<string, JToken> itemStateDict = itemState;
                    itemState["item"] = JToken.FromObject(m_Slots[i].m_Item.GetItemID());
                    itemState["number"] = JToken.FromObject(m_Slots[i].m_Number);
                    stateDict[i.ToString()] = itemState;
                }
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                m_Slots = new InventorySlot[m_InventorySize];
                IDictionary<string, JToken> stateDict = stateObject;
                for (int i = 0; i < m_InventorySize; i++)
                {
                    if (stateDict.ContainsKey(i.ToString()) && stateDict[i.ToString()] is JObject itemState)
                    {
                        IDictionary<string, JToken> itemStateDict = itemState;
                        m_Slots[i].m_Item = InventoryItem.GetFromID(itemStateDict["item"].ToObject<string>());
                        m_Slots[i].m_Number = itemStateDict["number"].ToObject<int>();
                    }
                }

                a_InventoryUpdated?.Invoke();
            }
        }

    }
}