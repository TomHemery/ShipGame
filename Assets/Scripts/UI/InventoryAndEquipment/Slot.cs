using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public EquipType m_equipType = EquipType.None;
    public Inventory associatedInventory = null;
    public Transform associatedEquipPoint = null;
    [SerializeField]
    public GameObject itemFramePrefab;

    public Text slotText;

    public ItemFrame StoredItemFrame { get; private set; } = null;

    /// <summary>
    /// Attempts to store the passed frame 
    /// </summary>
    /// <param name="newFrame">The frame to be stored</param>
    /// <returns>The quantity of inventoryItems associated with the passed frame that this slot has stored</returns>
    public int StoreItemFrame(ItemFrame newFrame) {
        if (StoredItemFrame == null) //currently empty
        {
            if (m_equipType == EquipType.None) //we are not an equipment slot
            {
                int quantityStored = associatedInventory.AddMaxOf(newFrame.m_inventoryItem);
                if (quantityStored > 0) {
                    StoredItemFrame = Instantiate(newFrame.gameObject, transform).GetComponent<ItemFrame>();
                    StoredItemFrame.m_inventoryItem.quantity = quantityStored;
                }
                return quantityStored;
            }
            else if (m_equipType == newFrame.m_inventoryItem.equipType) //we are an equipment slot of matching type 
            {
                GameObject equipment = Instantiate(PrefabDatabase.PrefabDictionary[newFrame.m_inventoryItem.systemName], associatedEquipPoint);
                StoredItemFrame = Instantiate(newFrame.gameObject, transform).GetComponent<ItemFrame>();
                StoredItemFrame.m_inventoryItem.quantity = 1;
                return 1;
            }
        }
        //not empty, but the passed frame matches the type of the already stored frame and it isn't equipment
        else if (m_equipType == EquipType.None && StoredItemFrame.m_inventoryItem.systemName == newFrame.m_inventoryItem.systemName) {
            int quantityStored = associatedInventory.AddMaxOf(newFrame.m_inventoryItem);
            if (quantityStored > 0)
            {
                StoredItemFrame.m_inventoryItem.quantity += quantityStored;
            }
            return quantityStored;
        }
        return 0;
    }


    /// <summary>
    /// Creates an item frame for a passed in inventory item, without attempting to update any associated inventories
    /// </summary>
    /// <param name="item">The item from which to spawn an item frame</param>
    /// <returns>True if successful</returns>
    public bool TryCreateFrameFor(InventoryItem item) {
        if(StoredItemFrame == null && item.equipType == m_equipType){
            if (item.equipType == EquipType.None)
            {
                StoredItemFrame = Instantiate(itemFramePrefab, transform).GetComponent<ItemFrame>();
                StoredItemFrame.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
                StoredItemFrame.m_inventoryItem = item;
                return true;
            }
            else if(item.quantity == 1)
            {
                StoredItemFrame = Instantiate(itemFramePrefab, transform).GetComponent<ItemFrame>();
                StoredItemFrame.m_inventoryItem = item;
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Removes (sets null) the stored item frame from this slot 
    /// </summary>
    public void RemoveItemFrame() {
        StoredItemFrame = null;
    }
}
