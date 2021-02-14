using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBeamWeapon : Weapon
{
    public GameObject beamPrefab;

    private GameObject beamInstance;
    public Transform beamSource;


    public override void Awake()
    {
        base.Awake();
        beamInstance = Instantiate(beamPrefab, beamSource);
        beamInstance.SetActive(false);
    }

    public override void EnableAutoFire()
    {
        base.EnableAutoFire();
        if (!GameManager.SimPaused)
        {
            beamInstance.SetActive(true);
            if(mAudioSource != null) mAudioSource.Play();
        }
    }

    public override void DisableAutoFire()
    {
        base.DisableAutoFire();
        beamInstance.SetActive(false);
        if (mAudioSource != null) mAudioSource.Stop();
    }

}
