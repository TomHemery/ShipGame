using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateSlotsOnEnable : MonoBehaviour
{
    public Inventory associatedInventory;
    public bool attachToPlayerInventory;
    public GameObject slotPrefab;

    private void Awake()
    {
        if (attachToPlayerInventory) associatedInventory = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>();
    }

    private void OnEnable()
    {
        int index = 0;
        foreach (KeyValuePair<string, InventoryItem> pair in associatedInventory.Contents) {
            if (index < transform.childCount)
            {
                Slot s = transform.GetChild(index).GetComponent<Slot>();
                if (s.TryCreateFrameFor(pair.Value)) index++;
            }
            else {
                Slot s = Instantiate(slotPrefab, transform).GetComponent<Slot>();
                if (s.TryCreateFrameFor(pair.Value)) index++;
            }
        }
    }
}
