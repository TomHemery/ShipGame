using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    /// <summary>
    /// The equip type of this slot (items stored must match)
    /// </summary>
    public EquipType equipType = EquipType.None;

    /// <summary>
    /// The name of the item this slot accepts, or "" if any
    /// </summary>
    public string acceptedItemName = "";

    /// <summary>
    /// The inventory associated with this slot, or null if none
    /// </summary>
    public Inventory associatedInventory = null;

    /// <summary>
    /// The equip point associated with this slot, or null if none
    /// </summary>
    public Transform associatedEquipPoint = null;

    /// <summary>
    /// The index of this slot in relation to a target inventory
    /// </summary>
    public int index;

    /// <summary>
    /// Any item frame stored in this slot can be shift clicked to this target
    /// </summary>
    public AutoMoveTarget autoMoveTarget;

    /// <summary>
    /// an outputOnly slot cannot store frames, but can create frames for inventory items (used in crafting system)
    /// </summary>
    public bool outputOnly = false;

    [SerializeField]
    public GameObject itemFramePrefab;

    public Text slotText;

    public ItemFrame StoredItemFrame { get; private set; } = null;

    private RectTransform rectTransform;

    public event System.Action SlotContentsChanged;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Attempts to store the passed frame and updates the associated inventories, returns the number of stored items 
    /// </summary>
    /// <param name="newFrame">The frame to be stored</param>
    /// <returns>The quantity of inventoryItems associated with the passed frame that this slot has stored</returns>
    public int StoreItemFrame(ItemFrame newFrame) {
        if (StoredItemFrame == null && !outputOnly) //currently empty
        {
            if (equipType == EquipType.None || (equipType == EquipType.SetItem && newFrame.inventoryItem.systemName == acceptedItemName)) 
            {
                int quantityStored = associatedInventory == null ?
                    newFrame.inventoryItem.quantity : associatedInventory.SilentAddMaxOf(newFrame.inventoryItem, index);

                if (quantityStored > 0)
                    SetFrame(newFrame, quantityStored);

                //outputs the "Inventory changed" event
                if (associatedInventory != null) associatedInventory.ForceAlertListeners();
                return quantityStored;
            }
            //we are a slot of some matching equipType
            else if (equipType == newFrame.inventoryItem.equipType)
            {
                //this is equipment
                if (newFrame.inventoryItem.equipable)
                {
                    Instantiate(PrefabDatabase.Get(newFrame.inventoryItem.systemName), associatedEquipPoint);
                    SetFrame(newFrame, 1);
                    return 1;
                }

                //this is a blueprint
                else if (equipType == EquipType.Blueprint) {
                    SetFrame(newFrame, newFrame.inventoryItem.quantity);
                    return newFrame.inventoryItem.quantity;
                }
            }
        }
        //not empty, but the passed frame matches the type of the already stored frame and it isn't equipment
        else if (!outputOnly && (equipType == EquipType.None || equipType == EquipType.SetItem) && StoredItemFrame.inventoryItem.systemName == newFrame.inventoryItem.systemName) {
            int quantityStored = associatedInventory == null ? 
                newFrame.inventoryItem.quantity : 
                associatedInventory.SilentAddMaxOf(newFrame.inventoryItem, index);
            if (quantityStored > 0)
            {
                SlotContentsChanged?.Invoke();
            }
            if(associatedInventory != null) associatedInventory.ForceAlertListeners();
            return quantityStored;
        }
        return 0;
    }

    public void RestoreChildFrame(ItemFrame frame) {
        if (associatedInventory != null) associatedInventory.SilentAddMaxOf(frame.inventoryItem, index);
        else if(equipType == EquipType.Weapon) Instantiate(PrefabDatabase.Get(frame.inventoryItem.systemName), associatedEquipPoint);
        SetFrame(frame, frame.inventoryItem.quantity);
        if (associatedInventory != null) associatedInventory.ForceAlertListeners();
    }

    private void SetFrame(ItemFrame frame, int quantity) {
        StoredItemFrame = Instantiate(frame.gameObject, rectTransform).GetComponent<ItemFrame>();
        StoredItemFrame.parentSlot = this;
        StoredItemFrame.SetQuantity(quantity);
        SlotContentsChanged?.Invoke();
    }


    /// <summary>
    /// Creates an item frame for a passed in inventory item, without attempting to update any associated inventories
    /// </summary>
    /// <param name="item">The item from which to spawn an item frame</param>
    /// <returns>True if successful</returns>
    public bool TryCreateFrameFor(InventoryItem item) {
        //if we haven't stored something already, and we are of a matching equip type, or the "none" type
        if(StoredItemFrame == null && 
            (equipType == EquipType.None || equipType == item.equipType || equipType == EquipType.SetItem && item.systemName == acceptedItemName)){
                //create a frame for the item
                StoredItemFrame = Instantiate(itemFramePrefab, rectTransform).GetComponent<ItemFrame>();
                StoredItemFrame.SetInventoryItem(item);
                StoredItemFrame.parentSlot = this;
                return true;
        }
        return false;
    }

    public void CreateFrameForEquipPoint() {
        if (!outputOnly && StoredItemFrame == null) {
            if (equipType == EquipType.Weapon) {
                slotText.text = associatedEquipPoint.name;
                if (associatedEquipPoint.childCount > 0)
                {
                    StoredItemFrame = Instantiate(itemFramePrefab, rectTransform).GetComponent<ItemFrame>();
                    Weapon w = associatedEquipPoint.GetChild(0).GetComponent<Weapon>();
                    StoredItemFrame.SetInventoryItem(new InventoryItem(w.inventoryItem));
                    StoredItemFrame.parentSlot = this;
                }
            }
        }
    }


    /// <summary>
    /// Removes (sets null) the stored item frame from this slot
    /// Updates associated inventories
    /// </summary>
    public void RemoveItemFrame() {
        if (StoredItemFrame != null)
        {
            InventoryItem item = StoredItemFrame.inventoryItem;
            StoredItemFrame = null;

            if (associatedInventory != null)
                associatedInventory.RemoveItemAt(index);

            if (associatedEquipPoint != null) Destroy(associatedEquipPoint.GetChild(0).gameObject);
            
            SlotContentsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Removes (sets null) the stored item frame from this slot
    /// Does not update associated inventories
    /// </summary>
    public void SilentRemoveItemFrame() {
        StoredItemFrame = null;
    }

    /// <summary>
    /// Removes and destroys the stored item frame from this slot 
    /// Updates associated inventories and alerts listeners
    /// </summary>
    public void DestroyItemFrame() {
        if (StoredItemFrame != null)
        {
            GameObject frame = StoredItemFrame.gameObject;
            RemoveItemFrame();
            Destroy(frame);
        }
    }

    /// <summary>
    /// Removes and destroys the stored item frame from this slot 
    /// Does not update associated inventories or alert listeners
    /// </summary>
    public void SilentDestroyItemFrame() {
        if (StoredItemFrame != null)
        {
            Destroy(StoredItemFrame.gameObject);
            StoredItemFrame = null;
        }
    }
}
