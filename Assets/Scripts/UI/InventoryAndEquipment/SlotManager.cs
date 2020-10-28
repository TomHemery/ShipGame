using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public Inventory associatedInventory;
    public bool attachToPlayerInventory;
    public GameObject slotPrefab;
    public AutoMoveTarget autoMoveTarget;
    public int minSlots;

    private void Awake()
    {
        if (attachToPlayerInventory) associatedInventory = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>();

        foreach (Transform child in transform) { //set up slots associated inventories
            child.GetComponent<Slot>().associatedInventory = associatedInventory;
            child.GetComponent<Slot>().autoMoveTarget = autoMoveTarget;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Slot s = transform.GetChild(i).GetComponent<Slot>();
            s.index = i;
        }
    }

    private void OnEnable()
    {
        associatedInventory.InventoryChangedEvent += OnAssociatedInventoryChanged;
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
        SilentClearAllSlots();

        int numSlots = minSlots > associatedInventory.Contents.Count ? minSlots : associatedInventory.Contents.Count;

        // create missing slots
        if (transform.childCount < numSlots)
        {
            for(int i = transform.childCount; i < numSlots; i++)
            {
                GameObject slot = Instantiate(slotPrefab, transform);
                slot.GetComponent<Slot>().index = i;
                slot.GetComponent<Slot>().autoMoveTarget = autoMoveTarget;
                slot.GetComponent<Slot>().associatedInventory = associatedInventory;
            }
        }

        // populate the inventory
        for (int i = 0; i < associatedInventory.Contents.Count; i++)
        {
            Slot s = transform.GetChild(i).GetComponent<Slot>();
            if (associatedInventory.Contents[i] != null)
            {
                s.TryCreateFrameFor(associatedInventory.Contents[i]);
            }
        }
    }
}
