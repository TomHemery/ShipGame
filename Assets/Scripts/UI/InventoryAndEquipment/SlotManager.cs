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
        ResetSlots();
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

    private void ResetSlots()
    {
        foreach(Transform child in transform) {
            child.GetComponent<Slot>().SilentRemoveItemFrame();
            foreach (Transform grandChild in child) {
                Destroy(grandChild.gameObject);
            }
        }

    }

    void UpdateSlots()
    {
        Debug.Log("Updating slots for " + associatedInventory);
        //check every item in the inventory
        foreach (KeyValuePair<string, InventoryItem> pair in associatedInventory.Contents)
        {
            Debug.Log(pair.Key + ": " + pair.Value.quantity);
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

            Debug.Log("Total in slots: " + totalInSlots);

            //if we haven't stored it all then store the rest
            if (totalInSlots < pair.Value.quantity) {
                //create an item with the right quantity
                InventoryItem toAdd = pair.Value;
                toAdd.quantity -= totalInSlots;
                bool stored = false;

                Debug.Log("To add: " + toAdd.quantity);

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
                if (!stored) {
                    s = Instantiate(slotPrefab, transform).GetComponent<Slot>();
                    s.associatedInventory = associatedInventory;
                    s.TryCreateFrameFor(toAdd);
                }
            }
        }
    }
}
