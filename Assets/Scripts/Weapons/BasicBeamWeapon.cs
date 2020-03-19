using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBeamWeapon : Weapon
{
    public GameObject beamPrefab;
    public Transform beamSource;

    private void Start()
    {
        beamPrefab = Instantiate(beamPrefab, beamSource);
        beamPrefab.SetActive(false);
    }

    public override void EnableAutoFire()
    {
        beamPrefab.SetActive(true);
    }

    public override void DisableAutoFire()
    {
        beamPrefab.SetActive(false);
    }

}
