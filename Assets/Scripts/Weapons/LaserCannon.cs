using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCannon : BasicHitscanWeapon
{
    public Light muzzleFlash;
    private float muzzleFlashMaxIntensity;

    public override void Awake()
    {
        base.Awake();
        muzzleFlashMaxIntensity = muzzleFlash.intensity;
        muzzleFlash.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        if (OnCooldown)
        {
            muzzleFlash.enabled = true;
            muzzleFlash.intensity = Mathf.Lerp(muzzleFlashMaxIntensity, 0, CooldownTimer / CooldownInterval);
        }
        else {
            muzzleFlash.enabled = false;
        }
    }
}
