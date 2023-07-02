﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Saving;

namespace Inventories
{
    /// <summary>
    /// Provides the storage for an action bar. The bar has a finite number of
    /// slots that can be filled and actions in the slots can be "used".
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class ActionStore : MonoBehaviour, IJsonSaveable
    {
        // STATE
        Dictionary<int, DockedItemSlot> m_DockedItems = new Dictionary<int, DockedItemSlot>();

        private class DockedItemSlot 
        {
            public ActionItem item;
            public int number;
        }

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action a_StoreUpdated;

        /// <summary>
        /// Get the action at the given index.
        /// </summary>
        public ActionItem GetAction(int index)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                return m_DockedItems[index].item;
            }
            return null;
        }

        /// <summary>
        /// Get the number of items left at the given index.
        /// </summary>
        /// <returns>
        /// Will return 0 if no item is in the index or the item has
        /// been fully consumed.
        /// </returns>
        public int GetNumber(int index)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                return m_DockedItems[index].number;
            }
            return 0;
        }

        /// <summary>
        /// Add an item to the given index.
        /// </summary>
        /// <param name="item">What item should be added.</param>
        /// <param name="index">Where should the item be added.</param>
        /// <param name="number">How many items to add.</param>
        public void AddAction(InventoryItem item, int index, int number)
        {
            if (m_DockedItems.ContainsKey(index))
            {  
                if (object.ReferenceEquals(item, m_DockedItems[index].item))
                {
                    m_DockedItems[index].number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.item = item as ActionItem;
                slot.number = number;
                m_DockedItems[index] = slot;
            }
            if (a_StoreUpdated != null)
            {
                a_StoreUpdated();
            }
        }

        /// <summary>
        /// Use the item at the given slot. If the item is consumable one
        /// instance will be destroyed until the item is removed completely.
        /// </summary>
        /// <param name="user">The character that wants to use this action.</param>
        /// <returns>False if the action could not be executed.</returns>
        public bool Use(int index, GameObject user)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                m_DockedItems[index].item.Use(user);
                if (m_DockedItems[index].item.isConsumable())
                {
                    RemoveItems(index, 1);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a given number of items from the given slot.
        /// </summary>
        public void RemoveItems(int index, int number)
        {
            if (m_DockedItems.ContainsKey(index))
            {
                m_DockedItems[index].number -= number;
                if (m_DockedItems[index].number <= 0)
                {
                    m_DockedItems.Remove(index);
                }
                if (a_StoreUpdated != null)
                {
                    a_StoreUpdated();
                }
            }
            
        }

        /// <summary>
        /// What is the maximum number of items allowed in this slot.
        /// 
        /// This takes into account whether the slot already contains an item
        /// and whether it is the same type. Will only accept multiple if the
        /// item is consumable.
        /// </summary>
        /// <returns>Will return int.MaxValue when there is not effective bound.</returns>
        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;

            if (m_DockedItems.ContainsKey(index) && !object.ReferenceEquals(item, m_DockedItems[index].item))
            {
                return 0;
            }
            if (actionItem.isConsumable())
            {
                return int.MaxValue;
            }
            if (m_DockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }

        /// PRIVATE

        [System.Serializable]
        private struct DockedItemRecord
        {
            public string itemID;
            public int number;
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (var pair in m_DockedItems)
            {
                JObject dockedState = new JObject();
                IDictionary<string, JToken> dockedStateDict = dockedState;
                dockedStateDict["item"] = JToken.FromObject(pair.Value.item.GetItemID());
                dockedStateDict["number"] = JToken.FromObject(pair.Value.number);
                stateDict[pair.Key.ToString()] = dockedState;
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                IDictionary<string, JToken> stateDict = stateObject;
                foreach (var pair in stateDict)
                {
                    if (pair.Value is JObject dockedState)
                    {
                        int key = Int32.Parse(pair.Key);
                        IDictionary<string, JToken> dockedStateDict = dockedState;
                        var item = InventoryItem.GetFromID(dockedStateDict["item"].ToObject<string>());
                        int number = dockedStateDict["number"].ToObject<int>();
                        AddAction(item, key, number);
                    }
                }
            }
        }

    }
}