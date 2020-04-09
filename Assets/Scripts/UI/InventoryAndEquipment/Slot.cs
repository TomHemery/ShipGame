using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public EquipType m_equipType = EquipType.None;
    public string acceptedItemName = "";
    public Inventory associatedInventory = null;
    public Transform associatedEquipPoint = null;
    [SerializeField]
    public GameObject itemFramePrefab;

    public Text slotText;

    public ItemFrame StoredItemFrame { get; private set; } = null;

    private RectTransform m_rectTransform;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Attempts to store the passed frame and updates the associated inventories 
    /// </summary>
    /// <param name="newFrame">The frame to be stored</param>
    /// <returns>The quantity of inventoryItems associated with the passed frame that this slot has stored</returns>
    public int StoreItemFrame(ItemFrame newFrame) {
        if (StoredItemFrame == null) //currently empty
        {
            if (m_equipType == EquipType.None) //we are not an equipment slot
            {
                int quantityStored = associatedInventory.SilentAddMaxOf(newFrame.m_InventoryItem);
                if (quantityStored > 0)
                {
                    StoredItemFrame = Instantiate(newFrame.gameObject, m_rectTransform).GetComponent<ItemFrame>();
                    StoredItemFrame.parentSlot = this;
                    StoredItemFrame.SetQuantity(quantityStored);
                }
                //outputs the "Inventory changed" event
                associatedInventory.ForceAlertListeners();
                return quantityStored;
            }
            else if (m_equipType == EquipType.SetItem && newFrame.m_InventoryItem.systemName == acceptedItemName) {
                int quantityStored = associatedInventory == null ? 
                    newFrame.m_InventoryItem.quantity : associatedInventory.SilentAddMaxOf(newFrame.m_InventoryItem);

                if (quantityStored > 0)
                {
                    StoredItemFrame = Instantiate(newFrame.gameObject, m_rectTransform).GetComponent<ItemFrame>();
                    StoredItemFrame.parentSlot = this;
                    StoredItemFrame.SetQuantity(quantityStored);
                }

                //outputs the "Inventory changed" event
                if(associatedInventory != null) associatedInventory.ForceAlertListeners();
                return quantityStored;
            }
            //we are an equipment slot of matching type and this is equipment
            else if (m_equipType == newFrame.m_InventoryItem.equipType && newFrame.m_InventoryItem.equipable)  
            {
                GameObject equipment = Instantiate(PrefabDatabase.PrefabDictionary[newFrame.m_InventoryItem.systemName], associatedEquipPoint);
                StoredItemFrame = Instantiate(newFrame.gameObject, m_rectTransform).GetComponent<ItemFrame>();
                StoredItemFrame.parentSlot = this;
                StoredItemFrame.SetQuantity(1);
                return 1;
            }
        }
        //not empty, but the passed frame matches the type of the already stored frame and it isn't equipment
        else if ((m_equipType == EquipType.None || m_equipType == EquipType.SetItem) && StoredItemFrame.m_InventoryItem.systemName == newFrame.m_InventoryItem.systemName) {
            int quantityStored = associatedInventory == null ? 
                newFrame.m_InventoryItem.quantity : 
                associatedInventory.SilentAddMaxOf(newFrame.m_InventoryItem);
            if (quantityStored > 0)
            {
                StoredItemFrame.SetQuantity(StoredItemFrame.m_InventoryItem.quantity + quantityStored);
            }
            if(associatedInventory != null) associatedInventory.ForceAlertListeners();
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
        //if we haven't stored something already, and we are of a matching equip type, or the "none" type
        if(StoredItemFrame == null && (m_equipType == EquipType.None || item.equipType == m_equipType)){
            //create a frame for the item
            StoredItemFrame = Instantiate(itemFramePrefab, m_rectTransform).GetComponent<ItemFrame>();
            StoredItemFrame.SetInventoryItem(item);
            StoredItemFrame.parentSlot = this;
            return true;
        }
        return false;
    }

    public void CreateFrameForEquipPoint() {
        if (StoredItemFrame == null) {
            if (m_equipType == EquipType.Weapon) {
                slotText.text = associatedEquipPoint.name;
                if (associatedEquipPoint.childCount > 0)
                {
                    StoredItemFrame = Instantiate(itemFramePrefab, m_rectTransform).GetComponent<ItemFrame>();
                    Weapon w = associatedEquipPoint.GetChild(0).GetComponent<Weapon>();
                    StoredItemFrame.SetInventoryItem(w.m_inventoryItem);
                    StoredItemFrame.parentSlot = this;
                }
            }
        }
    }


    /// <summary>
    /// Removes (sets null) the stored item frame from this slot
    ///  Updates associated inventories
    /// </summary>
    public void RemoveItemFrame() {
        if (StoredItemFrame != null)
        {
            if (associatedInventory != null)
                associatedInventory.TryRemoveItem(StoredItemFrame.m_InventoryItem.systemName,
                                                  StoredItemFrame.m_InventoryItem.quantity);

            if (associatedEquipPoint != null) Destroy(associatedEquipPoint.GetChild(0).gameObject);

            StoredItemFrame = null;
        }
    }

    /// <summary>
    /// Removes (sets null) the stored item frame from this slot
    /// Does not update associated inventories
    /// </summary>
    public void SilentRemoveItemFrame() {
        StoredItemFrame = null;
    }
}
