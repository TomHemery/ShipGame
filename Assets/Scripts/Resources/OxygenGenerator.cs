using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenGenerator : MonoBehaviour
{
    public Slot iceSlot;

    private OxygenResourceManager oxygenResource;

    //ice used per cooldown
    private int icePerCooldown = 1;
    private float oxygenPerIce = 10.0f;

    private float elapsedSinceLastUse = 0.0f;
    private float cooldownTime = 1.0f;
    bool cooldown = false;

    private void Awake()
    {
        oxygenResource = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<OxygenResourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (iceSlot != null && iceSlot.StoredItemFrame != null) {
            if (!cooldown && !oxygenResource.Full) 
            {
                oxygenResource.AddOxygen(oxygenPerIce * icePerCooldown);
                iceSlot.StoredItemFrame.SetQuantity(iceSlot.StoredItemFrame.m_InventoryItem.quantity - icePerCooldown);
                if (iceSlot.StoredItemFrame.m_InventoryItem.quantity <= 0) {
                    Destroy(iceSlot.StoredItemFrame);
                    iceSlot.SilentRemoveItemFrame();
                }
                cooldown = true;
            }
            else if(cooldown)
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
