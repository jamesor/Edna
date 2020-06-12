using System;
using System.Collections.Generic;
using JamesOR.Edna.Interactables;
using UnityEngine;

namespace JamesOR.Edna.Player
{
    public static class InventoryEvtType
    {
        public static string Changed = "Inventory.Changed";
        public static string AddItems = "Inventory.AddItems";
        public static string RemoveItems = "Inventory.RemoveItems";
    }

    public class InventoryEvtArgs : EventArgs
    {
        public List<Item> Items { get; set; }
    }

    public class Inventory
    {
        public List<Item> Items = new List<Item>();

        private int m_space;

        public Inventory(int space = 30)
        {
            m_space = space;
            EventManager.StartListening(InventoryEvtType.AddItems, OnAddItems);
            EventManager.StartListening(InventoryEvtType.RemoveItems, OnRemoveItems);
        }

        ~Inventory()
        {
            EventManager.StopListening(InventoryEvtType.AddItems, OnAddItems);
            EventManager.StopListening(InventoryEvtType.RemoveItems, OnRemoveItems);
        }

        public bool Add(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("Trying to add nothing to the Inventory; action ignored.");
                return false;
            }

            if (Items.Count >= m_space)
            {
                Debug.LogWarning("Inventory is full. Increase the size of the inventory or enable the player to drop items they no longer need.");
                return false;
            }

            Debug.Log(item.name + " added to the Inventory.");
            Items.Add(item);

            InventoryEvtArgs args = new InventoryEvtArgs();
            args.Items = Items;
            EventManager.TriggerEvent(InventoryEvtType.Changed, args);

            return true;
        }

        public void Remove(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("Trying to remove nothing from the Inventory; action ignored.");
                return;
            }

            if (Items.Remove(item))
            {
                Debug.Log(item.name + " removed from the Inventory.");
                InventoryEvtArgs args = new InventoryEvtArgs();
                args.Items = Items;
                EventManager.TriggerEvent(InventoryEvtType.Changed, args);
            }
            else
            {
                Debug.Log(item.name + " was not found in the Inventory.");
            }
        }

        #region Event Handlers
        private void OnAddItems(EventArgs e)
        {
            if (e is InventoryEvtArgs)
            {
                InventoryEvtArgs evt = (InventoryEvtArgs)e;
                foreach (var item in evt.Items)
                {
                    Add(item);
                }
            }
        }

        private void OnRemoveItems(EventArgs e)
        {
            if (e is InventoryEvtArgs)
            {
                InventoryEvtArgs evt = (InventoryEvtArgs)e;
                foreach (var item in evt.Items)
                {
                    Remove(item);
                }
            }
        }
        #endregion
    }
}
