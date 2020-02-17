using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    public Dictionary<GameObject, int> Contents { get; private set; } = new Dictionary<GameObject, int>();
    public int FilledCapacity { get; private set; } = 0;
    public int MaxCapacity = 100;

    /// <summary>
    /// Checks if the inventory has space for "quantity" of "item", if it does then it increases the quantity of "item" by "quantity"
    /// </summary>
    /// <param name="item">The item we're adding</param>
    /// <param name="quantity">The amount we're adding (default 1)</param>
    /// <returns>True if there is space, false otehrwise</returns>
    public bool TryAddItem(GameObject item, int quantity = 1) {
        if (MaxCapacity > FilledCapacity + quantity)
        {
            Contents[item] += quantity;
            FilledCapacity += quantity;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the inventory contains "quantity" of "item", if it does then it removes that amount of item from the inventory
    /// </summary>
    /// <param name="item">The GameObject we're looking for</param>
    /// <param name="quantity">The amount of the item we want</param>
    /// <returns>True if there is enough of the item in the inventory, false otherwise</returns>
    public bool TryRemoveItem(GameObject item, int quantity = 1)
    {
        bool result = CheckForItem(item, quantity);
        if (result) {
            Contents[item] -= quantity;
            if (Contents[item] <= 0) Contents.Remove(item);
            FilledCapacity -= quantity;
        }
        return result;
    }

    public bool CheckForItem(GameObject item, int quantity = 1) {
        return (Contents.ContainsKey(item) && Contents[item] >= quantity);
    }
}
