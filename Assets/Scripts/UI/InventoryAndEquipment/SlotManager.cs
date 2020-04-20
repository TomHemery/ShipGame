using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public Inventory associatedInventory;
    public bool attachToPlayerInventory;
    public GameObject slotPrefab;

    private void Awake()
    {
        if (attachToPlayerInventory) associatedInventory = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>();

        foreach (Transform child in transform) { //set up slots associated inventories
            child.GetComponent<Slot>().associatedInventory = associatedInventory;
        }
    }

    private void OnEnable()
    {
        associatedInventory.InventoryChangedEvent += OnAssociatedInventoryChanged;
        SilentClearAllSlots();
        UpdateSlots();
    }


    private void OnDisable()
    {
        associatedInventory.InventoryChangedEvent -= OnAssociatedInventoryChanged;
    }

    private void OnAssociatedInventoryChanged(object sender, EventArgs e)
    {
        UpdateSlots();
    }

    private void SilentClearAllSlots()
    {
        //go through every slot
        foreach(Transform child in transform) {
            //if something is stored, silently destroy it
            if (child.GetComponent<Slot>().StoredItemFrame != null)
            {
                GameObject frame = child.GetComponent<Slot>().StoredItemFrame.gameObject;
                child.GetComponent<Slot>().SilentRemoveItemFrame();
                Destroy(frame);
            }
        }
    }

    void UpdateSlots()
    {
        //make sure that no slots exist for items that aren't there any more
        foreach (Transform child in transform)
        {
            Slot s = child.GetComponent<Slot>();
            if (s.StoredItemFrame != null && !associatedInventory.Contents.ContainsKey(s.StoredItemFrame.m_InventoryItem.systemName))
            {
                GameObject itemFrame = s.StoredItemFrame.gameObject;
                s.SilentRemoveItemFrame();
                Destroy(itemFrame);
            }
        }

        //check every item in the inventory is fully represented in the slots
        foreach (KeyValuePair<string, InventoryItem> pair in associatedInventory.Contents)
        {
            //check how much is stored in the inventory slots currently
            Slot s;
            int totalInSlots = 0;
            foreach (Transform child in transform)
            {
                s = child.GetComponent<Slot>();
                if (s.StoredItemFrame != null && s.StoredItemFrame.m_InventoryItem.systemName == pair.Key) {
                    totalInSlots += s.StoredItemFrame.m_InventoryItem.quantity;
                }
            }

            //if we haven't stored it all then store the rest
            if (totalInSlots < pair.Value.quantity)
            {
                //create an item with the right quantity
                InventoryItem toAdd = pair.Value;
                toAdd.quantity -= totalInSlots;
                bool stored = false;

                //try to store it in an already existing frame
                foreach (Transform child in transform)
                {
                    s = child.GetComponent<Slot>();
                    if (s != null)
                    {
                        //free slot
                        if (s.StoredItemFrame == null)
                        {
                            stored = s.TryCreateFrameFor(toAdd);
                            break;
                        }
                        //matching type
                        else if (s.StoredItemFrame.m_InventoryItem.systemName == toAdd.systemName)
                        {
                            s.StoredItemFrame.SetQuantity(s.StoredItemFrame.m_InventoryItem.quantity + toAdd.quantity);
                            stored = true;
                            break;
                        }
                    }
                }

                //create a new slot otherwise
                if (!stored)
                {
                    s = Instantiate(slotPrefab, transform).GetComponent<Slot>();
                    s.associatedInventory = associatedInventory;
                    s.TryCreateFrameFor(toAdd);
                }
            }

            //if we have to much then take it away
            else if (totalInSlots > pair.Value.quantity) {
                int diff = totalInSlots - pair.Value.quantity;
                //try to store it in an already existing frame
                foreach (Transform child in transform)
                {
                    s = child.GetComponent<Slot>();
                    if (s != null && s.StoredItemFrame != null)
                    {
                        if (s.StoredItemFrame.m_InventoryItem.systemName == pair.Value.systemName) {
                            int quantity = s.StoredItemFrame.m_InventoryItem.quantity;
                            quantity -= diff;
                            if (quantity > 0)
                            {
                                s.StoredItemFrame.SetQuantity(quantity);
                                break;
                            }
                            else if (quantity == 0)
                            {
                                GameObject item = s.StoredItemFrame.gameObject;
                                s.SilentRemoveItemFrame();
                                Destroy(item);
                                break;
                            }
                            else if (quantity < 0) {
                                diff = Mathf.Abs(quantity);
                                GameObject item = s.StoredItemFrame.gameObject;
                                s.SilentRemoveItemFrame();
                                Destroy(item);
                            }
                        }
                    }
                }
            }
        }
    }
}
