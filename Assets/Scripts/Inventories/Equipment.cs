using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using Newtonsoft.Json.Linq;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, IJsonSaveable
    {
        // STATE
        Dictionary<EquipLocation, EquipableItem> m_EquippedItems = new Dictionary<EquipLocation, EquipableItem>();

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action a_EquipmentUpdated;

        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!m_EquippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return m_EquippedItems[equipLocation];
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.GetAllowedEquipLocation() == slot);

            m_EquippedItems[slot] = item;

            if (a_EquipmentUpdated != null)
            {
                a_EquipmentUpdated();
            }
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation slot)
        {
            m_EquippedItems.Remove(slot);
            if (a_EquipmentUpdated != null)
            {
                a_EquipmentUpdated();
            }
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return m_EquippedItems.Keys;
        }

        // PRIVATE

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (var pair in m_EquippedItems)
            {
                stateDict[pair.Key.ToString()] = JToken.FromObject(pair.Value.GetItemID());
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                m_EquippedItems.Clear();
                IDictionary<string, JToken> stateDict = stateObject;
                foreach (var pair in stateObject)
                {
                    if (Enum.TryParse(pair.Key, true, out EquipLocation key))
                    {
                        if (InventoryItem.GetFromID(pair.Value.ToObject<string>()) is EquipableItem item)
                        {
                            m_EquippedItems[key] = item;
                        }
                    }
                }
            }
            a_EquipmentUpdated?.Invoke();
        }

    }
}