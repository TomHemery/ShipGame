using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBeamWeapon : Weapon
{
    public float range;
    private GameObject beam; 

    public override void EnableAutoFire()
    {
        //instantiate the beam object 
    }

    public override void DisableAutoFire()
    {
        if (beam != null) {
            Destroy(beam);
            beam = null;
        }
    }

}
