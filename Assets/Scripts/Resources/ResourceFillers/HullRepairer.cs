using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullRepairer : MonoBehaviour
{
    public Slot ironSlot;

    private HealthAndShieldsResource playerhrm;

    //iron used per cooldown
    private int ironPerCooldown = 1;
    private float healthPerIron = 10.0f;

    private float elapsedSinceLastUse = 0.0f;
    private float cooldownTime = 0.5f;
    bool cooldown = false;

    private void Awake()
    {
        playerhrm = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<HealthAndShieldsResource>();
    }

    public void Update()
    {
        if (ironSlot != null && ironSlot.StoredItemFrame != null)
        {
            if (!cooldown && !playerhrm.IsFull())
            {
                playerhrm.AddHealth(healthPerIron * ironPerCooldown);
                ironSlot.StoredItemFrame.SetQuantity(ironSlot.StoredItemFrame.m_InventoryItem.quantity - ironPerCooldown);
                if (ironSlot.StoredItemFrame.m_InventoryItem.quantity <= 0)
                {
                    Destroy(ironSlot.StoredItemFrame.gameObject);
                    ironSlot.SilentRemoveItemFrame();
                }
                cooldown = true;
            }
            else if (cooldown)
            {
                elapsedSinceLastUse += Time.deltaTime;
                if (elapsedSinceLastUse > cooldownTime)
                {
                    elapsedSinceLastUse = 0.0f;
                    cooldown = false;
                }
            }
        }
    }
}
