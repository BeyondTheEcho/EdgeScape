﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.UI.Dragging;
using RPG.Inventories;

namespace UI.RPG.Inventories
{
    /// <summary>
    /// To be placed on icons representing the item in a slot. Allows the item
    /// to be dragged into other slots.
    /// </summary>
    public class InventoryDragItem : DragItem<InventoryItem>
    {
    }
}