using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceBarManager : ResourceBarManager
{
    HealthResourceManager playerHrm;
    private void Awake()
    {
        playerHrm = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<HealthResourceManager>();
        playerHrm.ResourceBarManager = this;
    }

    private void OnDestroy()
    {
        playerHrm.ResourceBarManager = null;
    }
}
