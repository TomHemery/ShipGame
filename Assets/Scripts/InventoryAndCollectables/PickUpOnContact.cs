using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpOnContact : MonoBehaviour
{
    public InventoryItem m_inventoryItem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Inventory targetInv = collision.gameObject.GetComponent<Inventory>();
        if (targetInv != null && targetInv.TryAddItem(m_inventoryItem))
        {
            Destroy(gameObject);
        }
    }
}