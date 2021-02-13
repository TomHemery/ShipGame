using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[Serializable]
public class Inventory : MonoBehaviour
{
    public InventoryCollection Contents { get; private set; } = new InventoryCollection();
    public int FilledCapacity { get; private set; } = 0;
    public int MaxCapacity = 100;

    public string prettyName = "";

    public event EventHandler InventoryChangedEvent;

    /// <summary>
    /// Checks if the inventory has space for an inventory item and its associated quantity, if it does then it stores it
    /// Alerts listeners
    /// </summary>
    /// <param name="item">The item we want to add</param>
    /// <param name="index">The index of the inventory space we want to populate</param>
    /// <returns>True if there is space and the item is added, false otehrwise</returns>
    public bool TryAddItem(InventoryItem item, int index = -1)
    {
        bool result = SilentTryAddItem(item, index);
        InventoryChangedEvent?.Invoke(this, null);
        return result;
    }

    /// <summary>
    /// Checks if the inventory has space for an inventory item and its associated quantity, if it does then it stores it
    /// Doesn't alert listeners
    /// </summary>
    /// <param name="item">The item we want to add</param>
    /// <param name="index">The index of the inventory space we want to populate</param>
    /// <returns>True if there is space and the item is added, false otehrwise</returns>
    public bool SilentTryAddItem(InventoryItem item, int index = -1)
    {
        if (FilledCapacity + item.quantity <= MaxCapacity)
        {
            AddToContents(item, index);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds as much of "item" as possible to the inventory
    /// Alerts listeners
    /// </summary>
    /// <param name="item">The item to be added</param>
    /// <param name="index">The index of the inventory space we want to populate</param>
    /// <returns>The total quantity of "item" that has been successfully stored in the inventory</returns>
    public int AddMaxOf(InventoryItem item, int index = -1)
    {
        int result = SilentAddMaxOf(item, index);
        InventoryChangedEvent?.Invoke(this, null);
        return result;
    }

    /// <summary>
    /// Adds as much of "item" as possible to the inventory
    /// Doesn't alert listeners
    /// </summary>
    /// <param name="item">The item to be added</param>
    /// <param name="index">The index of the inventory space we want to populate</param>
    /// <returns>The total quantity of "item" that has been successfully stored in the inventory</returns>
    public int SilentAddMaxOf(InventoryItem item, int index = -1)
    {
        if (FilledCapacity + item.quantity > MaxCapacity)
        {
            int quantityAdded = MaxCapacity - FilledCapacity;
            item.quantity -= quantityAdded;
            InventoryItem newItem = new InventoryItem(item)
            {
                quantity = quantityAdded
            };
            AddToContents(newItem, index);
            return quantityAdded;
        }
        else
        {
            AddToContents(item, index);
            return item.quantity;
        }
    }

    /// <summary>
    /// Attempts to add item to the contents dictionary. If index is negative this will add to the next available / matching slot.
    /// If index is positive the item will be added only at the specified index and only if possible.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool AddToContents(InventoryItem item, int index = -1)
    {
        if (item == null)
        { 
            return false;
        }
        else if (index < 0)
        {
            if (Contents.Contains(item.systemName) && Contents[item.systemName] != null)
            {
                InventoryItem inventoryItem = Contents[item.systemName];
                inventoryItem.quantity += item.quantity;
                Contents[item.systemName] = inventoryItem;
            }
            else
            {
                Contents[item.systemName] = item;
            }
        }
        else
        {
            if (Contents[index] != null)
            {
                if (Contents[index].systemName == item.systemName)
                {
                    InventoryItem inventoryItem = Contents[index];
                    inventoryItem.quantity += item.quantity;
                    Contents[index] = inventoryItem;
                }
                else return false;
            }
            else
            {
                Contents[index] = item;
            }
        }
        FilledCapacity += item.quantity;
        return true;
    }

    /// <summary>
    /// Checks if the inventory contains "quantity" of "item", if it does then it removes that amount of item from the inventory
    /// Alerts listeners
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity">The amount of the item we want</param>
    /// <returns>True if there is enough of the item in the inventory, false otherwise</returns>
    public bool TryRemove(string itemSystemName, int quantity = 1)
    {
        bool result = SilentTryRemove(itemSystemName, quantity);
        InventoryChangedEvent?.Invoke(this, null);
        return result;
    }

    /// <summary>
    /// Checks if the inventory contains "quantity" of "item", if it does then it removes that amount of item from the inventory
    /// Alerts listeners
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity">The amount of the item we want</param>
    /// <returns>True if there is enough of the item in the inventory, false otherwise</returns>
    public bool SilentTryRemove(string itemSystemName, int quantity = 1)
    {
        bool result = CountOf(itemSystemName) > 0;
        if (result)
        {
            FilledCapacity -= quantity;
            for (int i = Contents.Count - 1; i >= 0; i--)
            {
                InventoryItem item = Contents[i];
                if (item?.systemName == itemSystemName)
                {
                    item.quantity = item.quantity - quantity;
                    if (item.quantity <= 0)
                    {
                        Contents.RemoveAt(i);
                        quantity = -1 * item.quantity;
                    }
                    else
                    {
                        Contents[i] = item;
                        break;
                    }
                }
            }            
        }

        return result;
    }

    /// <summary>
    /// Checks if item.quantity of item.systemName exists, if it does then it removes that amount from the inventory
    /// Alerts listeners
    /// </summary>
    /// <param name="item">The item to be removed</param>
    /// <param name="index">The index from which we want to remove quantity of item</param>
    /// <returns></returns>
    public bool TryRemove(InventoryItem item)
    {
        return TryRemove(item.systemName, item.quantity);
    }

    /// <summary>
    /// Checks if item.quantity of item.systemName exists, if it does then it removes that amount from the inventory
    /// Doesn't alert listeners
    /// </summary>
    /// <param name="item">The item to be removed</param>
    /// <param name="index">The index from which we want to remove quantity of item</param>
    /// <returns></returns>
    public bool SilentTryRemove(InventoryItem item)
    {
        return SilentTryRemove(item.systemName, item.quantity);
    }

    public void RemoveItemAt(int i)
    {
        SilentRemoveItemAt(i);
        InventoryChangedEvent?.Invoke(this, null);
    }

    public void SilentRemoveItemAt(int i)
    {
        InventoryItem item = Contents[i];
        FilledCapacity -= item.quantity;
        Contents[i] = null;
    }

    /// <summary>
    /// Checks if an item with the specified index exists somewhere in the dictionary, checks at index if specified
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity"></param>
    /// <param name="index">The index to check at (if positive)</param>
    /// <returns></returns>
    public bool CheckForItem(string itemSystemName, int quantity = 1, int index = -1)
    {
        return index > 0 ?
            (Contents.Contains(itemSystemName) && Contents[itemSystemName].quantity >= quantity) :
                Contents[index] != null &&
                Contents[index].systemName == itemSystemName &&
                Contents[index].quantity >= quantity;
    }

    /// <summary>
    /// Gets the amount of item with passed in name in the inventory
    /// </summary>
    /// <param name="itemSystemName">Name of item to check for</param>
    /// <returns></returns>
    public int CountOf(string itemSystemName)
    {
        int count = 0;
        for(int i = 0; i < Contents.Count; i++)
        {
            if(Contents[i]?.systemName == itemSystemName)
            {
                count += Contents[i].quantity;
            }
        }
        return count;
    }

    /// <summary>
    /// Gets the amount of item in the inventory
    /// </summary>
    /// <param name="item">Item to check for</param>
    /// <returns></returns>
    public int CountOf(InventoryItem item)
    {
        return CountOf(item.systemName);
    }

    /// <summary>
    /// Invokes the inventory changed event
    /// </summary>
    public void ForceAlertListeners()
    {
        InventoryChangedEvent?.Invoke(this, null);
    }

    /// <summary>
    /// Clears the contents of the inventory
    /// Alerts listeners
    /// </summary>
    public void ClearContents() {
        Contents.Clear();
        FilledCapacity = 0;
        InventoryChangedEvent?.Invoke(this, null);
    }

    /// <summary>
    /// Sets the contents of the inventory from a list of pairs of item names and quantities 
    /// Alerts listeners
    /// </summary>
    /// <param name="newContents">List of items as names and quantites</param>
    public void SetContents(List<KeyValuePair<string, int>> newContents) {
        ClearContents();
        foreach (KeyValuePair<string, int> pair in newContents)
        {
            if (!pair.Equals(new KeyValuePair<string, int>()) && pair.Key != "" && pair.Value >= 0)
            {
                InventoryItem item = new InventoryItem(pair.Key)
                {
                    quantity = pair.Value
                };
                Contents.AddToEnd(item);
                FilledCapacity += pair.Value;
            }
            else
            {
                Contents.AddToEnd(null);
            }
        }
        InventoryChangedEvent?.Invoke(this, null);
    }
}