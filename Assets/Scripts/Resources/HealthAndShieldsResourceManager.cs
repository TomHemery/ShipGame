
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAndShieldsResourceManager : HealthResourceManager
{
    public float Shields { get; private set; }
    public float MaxShields = 50;
    public float ShieldRechargeRate { get; private set; } = 10; //shields per second
    public event EventHandler<ShieldChangedEventArgs> ShieldValueChangedEvent;

    void Start()
    {
        SetShields(MaxShields);
        SetHealth(MaxHealth);
    }

    private void Update()
    {
        AddShields(Time.deltaTime * ShieldRechargeRate);
    }

    /// <summary>
    /// Sets shields to the value of s (shields are constrained between 0 and max)
    /// </summary>
    /// <param name="s">The value to set shields to</param>
    public void SetShields(float s) {
        Shields = Mathf.Clamp(s, 0, MaxShields);
        ShieldValueChangedEvent?.Invoke(this, new ShieldChangedEventArgs(Shields, MaxShields));
    }

    /// <summary>
    /// Modifies shields by the value of s (shields are constrained between 0 amd max
    /// </summary>
    /// <param name="s">The value to modify shields by</param>
    public void ModifyShields(float s) {
        SetShields(Shields + s);
    }

    /// <summary>
    /// Increases shields by the absolute value of s (shields cannot go above max)
    /// </summary>
    /// <param name="s">The value to increase shields by</param>
    public void AddShields(float s) {
        ModifyShields(Mathf.Abs(s));
    }

    /// <summary>
    /// Reduces health by the absolute value of s (shields cannot go below 0)
    /// </summary>
    /// <param name="s">The value to reduce shields by</param>
    public void ReduceShields(float s) {
        ModifyShields(-Mathf.Abs(s));
    }

    /// <summary>
    /// Reduces shields, then health by the absolute value of d
    /// </summary>
    /// <param name="d">The amount of damage to do</param>
    public override void DoDamage(float d)
    {
        d = Mathf.Abs(d);
        float healthDamage = d - Shields;
        if (healthDamage < 0) healthDamage = 0;
        float shieldDamage = d - healthDamage;
        ReduceShields(shieldDamage);
        ReduceHealth(healthDamage);
    }
}

public class ShieldChangedEventArgs : EventArgs
{
    public ShieldChangedEventArgs(float _newShields, float _maxShields) { NewShields = _newShields; MaxShields = _maxShields; }
    public float NewShields { get; } 
    public float MaxShields { get; }
}
