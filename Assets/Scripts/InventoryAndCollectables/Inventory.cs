using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, InventoryItem> Contents { get; private set; } = new Dictionary<string, InventoryItem>();
    public int FilledCapacity { get; private set; } = 0;
    public int MaxCapacity = 100;

    public string prettyName = "";

    [HideInInspector]
    public List<InventoryUIController> uiControllers = new List<InventoryUIController>();

    /// <summary>
    /// Checks if the inventory has space for "quantity" of "item", if it does then it increases the quantity of "item" by "quantity"
    /// Not recommended, try to add item using an actual InventoryItem struct if possible
    /// </summary>
    /// <param name="itemSystemName">The system name (key) of the item we're adding</param>
    /// <param name="quantity">The amount we're adding (default 1)</param>
    /// <returns>True if there is space, false otehrwise</returns>
    public bool TryAddItem(string itemSystemName, int quantity = 1) {
        if (FilledCapacity + quantity <= MaxCapacity)
        {
            if (Contents.ContainsKey(itemSystemName))
            {
                InventoryItem inventoryItem = Contents[itemSystemName];
                inventoryItem.quantity += quantity;
                Contents[itemSystemName] = inventoryItem;
            }
            else
            {
                InventoryItem newItem = new InventoryItem
                {
                    quantity = quantity,
                    systemName = itemSystemName
                };
                Contents.Add(itemSystemName, newItem);
            }
            FilledCapacity += quantity;
            UpdateUIControllers();
            return true;
        }
        else {
            Debug.Log("Inventory full!!");
        }
        return false;
    }

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
            else Contents.Add(item.systemName, item);
            FilledCapacity += item.quantity;
            UpdateUIControllers();
            return true;
        }
        else
        {
            Debug.Log("Inventory full!!");
        }
        return false;
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
            UpdateUIControllers();
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
        bool result = CheckForItem(item.systemName, item.quantity);
        if (result)
        {
            InventoryItem oldItem = Contents[item.systemName];
            oldItem.quantity = oldItem.quantity - item.quantity;
            if (oldItem.quantity <= 0) Contents.Remove(item.systemName);
            else Contents[item.systemName] = oldItem;
            FilledCapacity -= item.quantity;
            UpdateUIControllers();
        }
        return result;
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

    private void UpdateUIControllers() {
        foreach(InventoryUIController uic in uiControllers)
            uic.UpdateContents();
    }
}
