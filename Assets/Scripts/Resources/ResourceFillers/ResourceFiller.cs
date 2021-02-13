using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFiller : MonoBehaviour
{
    public Slot slot;
    public Resource resource;

    //ice used per cooldown
    public int itemUsePerCooldown = 1;
    public float resourcePerItem = 10.0f;

    public float cooldownTime = 0.5f;
    private float cooldownTimer = 0.0f;

    bool cooldown = false;

    private void Update()
    {
        if (slot != null && slot.StoredItemFrame != null)
        {
            if (!cooldown && !resource.IsFull())
            {
                resource.AddResource(resourcePerItem * itemUsePerCooldown);
                slot.StoredItemFrame.SetQuantity(slot.StoredItemFrame.inventoryItem.quantity - itemUsePerCooldown);
                if (slot.StoredItemFrame.inventoryItem.quantity <= 0)
                {
                    Destroy(slot.StoredItemFrame.gameObject);
                    slot.SilentRemoveItemFrame();
                }
                cooldown = true;
            }
            else if (cooldown)
            {
                cooldownTimer += Time.deltaTime;
                if (cooldownTimer > cooldownTime)
                {
                    cooldownTimer = 0.0f;
                    cooldown = false;
                }
            }
        }
    }
}
