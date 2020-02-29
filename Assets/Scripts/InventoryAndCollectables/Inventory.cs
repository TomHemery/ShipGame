using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, InventoryItem> Contents { get; private set; } = new Dictionary<string, InventoryItem>();
    public int FilledCapacity { get; private set; } = 0;
    public int MaxCapacity = 100;

    public string prettyName = "";

    /// <summary>
    /// Checks if the inventory has space for an inventory item and its associated quantity, if it does then it stores it
    /// </summary>
    /// <param name="item">The item we want to add</param>
    /// <returns>True if there is space and the item is added, false otehrwise</returns>
    public bool TryAddItem(InventoryItem item) {
        if (FilledCapacity + item.quantity <= MaxCapacity)
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
            return true;
        }
        else
        {
            Debug.Log("Inventory full!!");
        }
        return false;
    }

    /// <summary>
    /// Adds as much of "item" as possible to the inventory
    /// </summary>
    /// <param name="item">The item to be added</param>
    /// <returns>The total quantity of "item" that has been successfully stored in the inventory</returns>
    public int AddMaxOf(InventoryItem item) {
        if (FilledCapacity + item.quantity > MaxCapacity)
            item.quantity = MaxCapacity - FilledCapacity;

        if (item.quantity > 0)
        {
            AddToContents(item);
            FilledCapacity += item.quantity;
        }

        return item.quantity;
    }

    private void AddToContents(InventoryItem item) {
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
    }

    /// <summary>
    /// Checks if the inventory contains "quantity" of "item", if it does then it removes that amount of item from the inventory
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity">The amount of the item we want</param>
    /// <returns>True if there is enough of the item in the inventory, false otherwise</returns>
    public bool TryRemoveItem(string itemSystemName, int quantity = 1)
    {
        bool result = CheckForItem(itemSystemName, quantity);
        if (result) {
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
    /// </summary>
    /// <param name="item">The item to be removed</param>
    /// <returns></returns>
    public bool TryRemoveItem(InventoryItem item)
    {
        return TryRemoveItem(item.systemName, item.quantity);
    }

    /// <summary>
    /// Checks if "quantity" of "item" are present in the inventory
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're looking for</param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public bool CheckForItem(string itemSystemName, int quantity = 1) {
        return (Contents.ContainsKey(itemSystemName) && Contents[itemSystemName].quantity >= quantity);
    }
}
