﻿using UnityEngine;

public class OxygenResourceManager : Resource
{
    /// <summary>
    /// Maximum oxygen capacity for this oxygen resource
    /// </summary>
    public float MaxOxygenCapacity { get; private set; } = 100.0f;
    /// <summary>
    /// The current amount of oxygen stored by this resource
    /// </summary>
    public float Oxygen { get; private set; }

    /// <summary>
    /// Is this resource full?
    /// </summary>
    public bool Full { get; private set; } = true;

    /// <summary>
    /// The base amount of oxygen used per second by this resource
    /// </summary>
    public float baseOxygenUsageRate = 1.0f;

    private void Awake()
    {
        InstantFillOxygen();
    }

    /// <summary>
    /// Instantly sets the stored quantity of oxygen to its maximum value
    /// </summary>
    void InstantFillOxygen() {
        SetOxygen(MaxOxygenCapacity);
    }

    /// <summary>
    /// Increases stored oxygen by the absolute value of o - clamped between 0 and max
    /// </summary>
    /// <param name="o"></param>
    public void AddOxygen(float o) {
        SetOxygen(Mathf.Clamp(Oxygen + Mathf.Abs(o), 0, MaxOxygenCapacity));
    }

    /// <summary>
    /// Reduces stored oxygen by the absolute value of o - clamped between 0 and max
    /// </summary>
    /// <param name="o"></param>
    public void ReduceOxygen(float o) {
        SetOxygen(Mathf.Clamp(Oxygen - Mathf.Abs(o), 0, MaxOxygenCapacity));
    }

    /// <summary>
    /// Sets the value of stored oxygen to o, never update oxygen variable directly in any other way
    /// </summary>
    /// <param name="o"></param>
    private void SetOxygen(float o) {
        Oxygen = o;
        Full = Oxygen == MaxOxygenCapacity;
        if (Oxygen == 0.0f) OutOfOxygen();
    }

    private void OutOfOxygen() {
        GameManager.Instance.OnPlayerDeath(PlayerDeathTypes.OutOfOxygen);
    }

    public override void UpdateResource()
    {
        if (Oxygen > 0.0f)
        {
            ReduceOxygen(baseOxygenUsageRate * Time.deltaTime);
        }
    }
}