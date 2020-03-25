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
        base.EnableAutoFire();
        if (!GameManager.SimPaused)
        {
            beamPrefab.SetActive(true);
            if(mAudioSource != null) mAudioSource.Play();
        }
    }

    public override void DisableAutoFire()
    {
        base.DisableAutoFire();
        beamPrefab.SetActive(false);
        if (mAudioSource != null) mAudioSource.Stop();
    }

}
