using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory : MonoBehaviour
{
    public Dictionary<string, InventoryItem> Contents { get; private set; } = new Dictionary<string, InventoryItem>();
    public int FilledCapacity { get; private set; } = 0;
    public int MaxCapacity = 100;

    public string prettyName = "";

    public event EventHandler InventoryChangedEvent;

    /// <summary>
    /// Checks if the inventory has space for an inventory item and its associated quantity, if it does then it stores it
    ///  Notifies listeners
    /// </summary>
    /// <param name="item">The item we want to add</param>
    /// <returns>True if there is space and the item is added, false otehrwise</returns>
    public bool TryAddItem(InventoryItem item)
    {
        bool result = SilentTryAddItem(item);
        InventoryChangedEvent?.Invoke(this, null);
        return result;
    }

    /// <summary>
    /// Checks if the inventory has space for an inventory item and its associated quantity, if it does then it stores it
    ///  Doesn't notify listeners
    /// </summary>
    /// <param name="item">The item we want to add</param>
    /// <returns>True if there is space and the item is added, false otehrwise</returns>
    public bool SilentTryAddItem(InventoryItem item)
    {
        if (FilledCapacity + item.quantity <= MaxCapacity)
        {
            AddToContents(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds as much of "item" as possible to the inventory
    ///  Notifies listeners
    /// </summary>
    /// <param name="item">The item to be added</param>
    /// <returns>The total quantity of "item" that has been successfully stored in the inventory</returns>
    public int AddMaxOf(InventoryItem item)
    {
        int result = SilentAddMaxOf(item);
        InventoryChangedEvent?.Invoke(this, null);
        return result;
    }

    /// <summary>
    /// Adds as much of "item" as possible to the inventory
    ///  Doesn't notify listeners
    /// </summary>
    /// <param name="item">The item to be added</param>
    /// <returns>The total quantity of "item" that has been successfully stored in the inventory</returns>
    public int SilentAddMaxOf(InventoryItem item)
    {
        if (FilledCapacity + item.quantity > MaxCapacity)
            item.quantity = MaxCapacity - FilledCapacity;

        if (item.quantity > 0) AddToContents(item);

        return item.quantity;
    }

    private void AddToContents(InventoryItem item)
    {
        if (Contents.ContainsKey(item.systemName))
        {
            InventoryItem inventoryItem = Contents[item.systemName];
            inventoryItem.quantity += item.quantity;
            Contents[item.systemName] = inventoryItem;
        }
        else
        {
            Contents.Add(item.systemName, item);
        }
        FilledCapacity += item.quantity;
    }

    /// <summary>
    /// Checks if the inventory contains "quantity" of "item", if it does then it removes that amount of item from the inventory
    ///  Notifies listeners
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity">The amount of the item we want</param>
    /// <returns>True if there is enough of the item in the inventory, false otherwise</returns>
    public bool TryRemoveItem(string itemSystemName, int quantity = 1)
    {
        bool result = SilentTryRemoveItem(itemSystemName, quantity);
        InventoryChangedEvent?.Invoke(this, null);
        return result;
    }

    /// <summary>
    /// Checks if the inventory contains "quantity" of "item", if it does then it removes that amount of item from the inventory
    ///  Doesn't notify listeners
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity">The amount of the item we want</param>
    /// <returns>True if there is enough of the item in the inventory, false otherwise</returns>
    public bool SilentTryRemoveItem(string itemSystemName, int quantity = 1)
    {
        bool result = CheckForItem(itemSystemName, quantity);
        if (result)
        {
            InventoryItem item = Contents[itemSystemName];
            item.quantity = item.quantity - quantity;
            if (item.quantity <= 0) Contents.Remove(itemSystemName);
            else Contents[itemSystemName] = item;
            FilledCapacity -= quantity;
        }

        return result;
    }

    /// <summary>
    /// Checks if item.quantity of item.systemName exists, if it does then it removes that amount from the inventory
    ///  Notifies listeners
    /// </summary>
    /// <param name="item">The item to be removed</param>
    /// <returns></returns>
    public bool TryRemoveItem(InventoryItem item)
    {
        return TryRemoveItem(item.systemName, item.quantity);
    }

    /// <summary>
    /// Checks if item.quantity of item.systemName exists, if it does then it removes that amount from the inventory
    ///  Doesn't notify listeners
    /// </summary>
    /// <param name="item">The item to be removed</param>
    /// <returns></returns>
    public bool SilentTryRemoveItem(InventoryItem item)
    {
        return SilentTryRemoveItem(item.systemName, item.quantity);
    }

    /// <summary>
    /// Checks if "quantity" of "item" are present in the inventory
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool CheckForItem(string itemSystemName, int quantity = 1)
    {
        return (Contents.ContainsKey(itemSystemName) && Contents[itemSystemName].quantity >= quantity);
    }

    public void ForceAlertListeners()
    {
        InventoryChangedEvent?.Invoke(this, null);
    }

    public void ClearContents() {
        Contents.Clear();
        FilledCapacity = 0;
        InventoryChangedEvent?.Invoke(this, null);
    }

    public void SetContents(Dictionary<string, int> newContents) {
        Debug.Log("Setting inventory contents");
        ClearContents();
        foreach (KeyValuePair<string, int> pair in newContents)
        {

            GameObject itemGameObject = PrefabDatabase.PrefabDictionary[pair.Key];
            if (itemGameObject.GetComponent<Weapon>() != null)
            {
                InventoryItem item = itemGameObject.GetComponent<Weapon>().m_inventoryItem;
                item.quantity = pair.Value;
                Debug.Log("Adding " + item);
                AddToContents(item);
            }
            else if (itemGameObject.GetComponent<PickUpOnContact>() != null)
            {
                InventoryItem item = itemGameObject.GetComponent<PickUpOnContact>().m_inventoryItem;
                item.quantity = pair.Value;
                Debug.Log("Adding " + item);
                AddToContents(item);
            }
        }
        InventoryChangedEvent?.Invoke(this, null);
    }
}