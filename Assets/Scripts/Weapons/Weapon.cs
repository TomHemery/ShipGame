using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    /// <summary>
    /// The interval with which this weapon will cooldown in seconds
    /// </summary>
    public float CooldownInterval = 1.0f;
    /// <summary>
    /// The total time this weapon has spent on cooldown in seconds
    /// </summary>
    public float CooldownTimer {get; private set;} = 0.0f;
    /// <summary>
    /// Whether this weapon is on cool down
    /// </summary>
    public bool OnCooldown {get; private set;} = false;
    /// <summary>
    /// Wether this weapon should auto fire using cooldown interval
    /// </summary>
    public bool DoAutoFire = false;

    private void Update()
    {
        if (OnCooldown)
        {
            CooldownTimer += Time.deltaTime;
            if (CooldownTimer >= CooldownInterval) {
                OnCooldown = false;
                CooldownTimer = 0;
            }
        }
        else if (DoAutoFire) {
            TryFire();
        }
    }

    /// <summary>
    /// Syntactic sugar - do auto fire true
    /// </summary>
    public void EnableAutoFire() {
        DoAutoFire = true;
    }

    /// <summary>
    /// Syntactic sugar - do auto fire false
    /// </summary>
    public void DisableAutoFire() {
        DoAutoFire = false;
    }

    /// <summary>
    /// Attempts to fire this weapon based on cooldown
    /// </summary>
    /// <returns>True only if the weapon fires sucessfully</returns>
    public bool TryFire() {
        if (!OnCooldown) {
            Fire();
            OnCooldown = true;
            return true;
        }
        return false;
    }
    /// <summary>
    /// The fire method, should be provided by implementations of the weapon interface 
    /// </summary>
    protected abstract void Fire();

}
