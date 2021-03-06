﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAndShieldsResource : HealthResource
{
    public const float DEFAULT_MAX_SHIELDS = 50;
    
    public float MaxShieldValue = DEFAULT_MAX_SHIELDS;
    public float ShieldValue { get; private set; }

    public float ShieldRechargeRate { get; private set; } = 10; //shield value added per second
    public event EventHandler<ShieldChangedEventArgs> ShieldValueChangedEvent;

    public bool ShieldsDisabled { get; private set; } = false;
    private float DisableTimer = 0.0f;

    protected override void Update()
    {
        base.Update();
        //recharge shields if the game isn't paused
        if (!GameManager.SimPaused)
        {
            //only recharge if shields aren't disabled
            if (!ShieldsDisabled)
            {
                AddShields(Time.deltaTime * ShieldRechargeRate);
            }
            //countdown until we can recharge again
            else
            {
                DisableTimer -= Time.deltaTime;
                if (DisableTimer <= 0) {
                    DisableTimer = 0.0f;
                    ShieldsDisabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Completely fills shields and health
    /// </summary>
    public override void FillResource()
    {
        base.FillResource();
        SetShieldValue(MaxHealthValue);
    }

    /// <summary>
    /// Sets shields to the value of s (shields are constrained between 0 and max)
    /// </summary>
    /// <param name="s">The value to set shields to</param>
    public void SetShieldValue(float s) {
        ShieldValue = Mathf.Clamp(s, 0, MaxShieldValue);
        ShieldValueChangedEvent?.Invoke(this, new ShieldChangedEventArgs(ShieldValue, MaxShieldValue));
    }


    /// <summary>
    /// Sets max shields to the value of ms, constrains shields between 0 and ms
    /// </summary>
    /// <param name="ms">The value to set max shields to</param>
    public void SetMaxShieldValue(float ms) {
        MaxShieldValue = ms;
        SetShieldValue(ShieldValue);
    }

    /// <summary>
    /// Modifies shields by the value of s (shields are constrained between 0 amd max
    /// </summary>
    /// <param name="s">The value to modify shields by</param>
    public void ModifyShieldValue(float s) {
        SetShieldValue(ShieldValue + s);
    }

    /// <summary>
    /// Increases shields by the absolute value of s (shields cannot go above max)
    /// </summary>
    /// <param name="s">The value to increase shields by</param>
    public void AddShields(float s) {
        ModifyShieldValue(Mathf.Abs(s));
    }

    /// <summary>
    /// Reduces health by the absolute value of s (shields cannot go below 0)
    /// </summary>
    /// <param name="s">The value to reduce shields by</param>
    public void SubtractShields(float s) {
        ModifyShieldValue(-Mathf.Abs(s));
    }

    /// <summary>
    /// Reduces shields, then health by the absolute value of d
    /// </summary>
    /// <param name="d">The amount of damage to do</param>
    public override void DoDamage(float d)
    {
        d = Mathf.Abs(d);
        float healthDamage = d - ShieldValue;
        if (healthDamage < 0) healthDamage = 0;
        float shieldDamage = d - healthDamage;
        SubtractShields(shieldDamage);
        SubtractHealth(healthDamage);
    }

    /// <summary>
    /// Disables (sets to zero) the shields for the specified duration
    /// </summary>
    /// <param name="time">The time to disable shields for</param>
    public void DisableShieldsForDuration(float time) {
        ShieldsDisabled = true;
        DisableTimer = time;
        SetShieldValue(0);
    }
}

/// <summary>
/// Argumnents for the shield charged event
/// </summary>
public class ShieldChangedEventArgs : EventArgs
{
    public ShieldChangedEventArgs(float _newShields, float _maxShields) { NewShields = _newShields; MaxShields = _maxShields; }
    public float NewShields { get; } 
    public float MaxShields { get; }
}
