
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipResourceManager : MonoBehaviour
{

    public float Health { get; private set; }
    public float MaxHealth = 100;
    public float Shields { get; private set; }
    public float MaxShields = 50;
    public float ShieldRechargeRate { get; private set; } = 10; //shields per second


    public bool HasShields = true; //does this ship have shields?

    public ResourceBarManager ResourceBarManager;

    // Start is called before the first frame update
    void Start()
    {
        if (!HasShields) MaxShields = 0;
        SetHealth(MaxHealth);
        SetShields(MaxShields);
    }

    private void Update()
    {
        AddShields(Time.deltaTime * ShieldRechargeRate);
        if (Health <= 0) Explode();
    }

    /// <summary>
    /// Sets health to the value of h (health is constrained between 0 and max)
    /// </summary>
    /// <param name="h">The value to set health to</param>
    public void SetHealth(float h) {
        Health = h;
        if (Health < 0) Health = 0;
        else if (Health > MaxHealth) Health = MaxHealth;
        if (ResourceBarManager != null) ResourceBarManager.UpdateHealthBar(Health, MaxHealth);
    }

    /// <summary>
    /// Modifies health by the value of h (health is constrained between 0 and max)
    /// </summary>
    /// <param name="h">The value to modify h by</param>
    public void ModifyHealth(float h) {
        SetHealth(Health + h);
    }

    /// <summary>
    /// Increases health by the absolute value of h (health cannot go above max)
    /// </summary>
    /// <param name="h">The value to increase health by</param>
    public void AddHealth(float h) {
        ModifyHealth(Mathf.Abs(h));
    }

    /// <summary>
    /// Reduces health by the absolute value of h (health cannot go below 0)
    /// </summary>
    /// <param name="h">The value to reduce health by</param>
    public void ReduceHealth(float h) {
        ModifyHealth(-Mathf.Abs(h));
    }

    /// <summary>
    /// Sets shields to the value of s (shields are constrained between 0 and max)
    /// </summary>
    /// <param name="s">The value to set shields to</param>
    public void SetShields(float s) {
        Shields = s;
        if (Shields < 0) Shields = 0;
        else if (Shields > MaxShields) Shields = MaxShields;
        if (ResourceBarManager != null) ResourceBarManager.UpdateShieldBar(Shields, MaxShields);
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
    public void DoDamage(float d)
    {
        if (HasShields)
        {
            d = Mathf.Abs(d);
            float healthDamage = d - Shields;
            if (healthDamage < 0) healthDamage = 0;
            float shieldDamage = d - healthDamage;
            ReduceShields(shieldDamage);
            ReduceHealth(healthDamage);
        }
        else
            ReduceHealth(d);
    }

    private void Explode()
    {
        Destroy(gameObject);
    }
}
