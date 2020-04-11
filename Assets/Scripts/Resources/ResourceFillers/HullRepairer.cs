using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullRepairer : MonoBehaviour
{
    public Slot slot;

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
        if (slot != null && slot.StoredItemFrame != null)
        {
            if (!cooldown && !playerhrm.IsFull())
            {
                playerhrm.AddHealth(healthPerIron * ironPerCooldown);
                slot.StoredItemFrame.SetQuantity(slot.StoredItemFrame.m_InventoryItem.quantity - ironPerCooldown);
                if (slot.StoredItemFrame.m_InventoryItem.quantity <= 0)
                {
                    Destroy(slot.StoredItemFrame.gameObject);
                    slot.SilentRemoveItemFrame();
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
