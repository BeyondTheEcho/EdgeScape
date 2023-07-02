using System.Collections;
using System.Collections.Generic;
using Core.UI.Dragging;
using Inventories;
using UnityEngine;

namespace UI.Inventories
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] int index = 0;

        // CACHE
        ActionStore store;

        // LIFECYCLE METHODS
        private void Awake()
        {
            store = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
            store.a_StoreUpdated += UpdateIcon;
        }

        // PUBLIC

        public void AddItems(InventoryItem item, int number)
        {
            store.AddAction(item, index, number);
        }

        public InventoryItem GetItem()
        {
            return store.GetAction(index);
        }

        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return store.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        // PRIVATE

        void UpdateIcon()
        {
            icon.SetItem(GetItem(), GetNumber());
        }
    }
}
