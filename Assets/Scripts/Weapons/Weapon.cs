using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    public InventoryItem m_inventoryItem;
    protected ShipController parentShip_controller;

    public AudioSource mAudioSource = null;

    public void Awake()
    {
        parentShip_controller = transform.root.GetComponent<ShipController>();
        if(parentShip_controller != null) parentShip_controller.weapons.Add(this);
    }

    private void OnDestroy()
    {
        if (parentShip_controller != null) parentShip_controller.weapons.Remove(this);
    }

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
    public bool AutoFiring { get; private set; } = false;

    protected virtual void Update()
    {
        if (OnCooldown)
        {
            CooldownTimer += Time.deltaTime;
            if (CooldownTimer >= CooldownInterval) {
                OnCooldown = false;
                CooldownTimer = 0;
            }
        }
        else if (AutoFiring) {
            TryFire();
        }
    }

    /// <summary>
    /// Start auto firing this weapon
    /// </summary>
    public virtual void EnableAutoFire() {
        AutoFiring = true;
    }

    /// <summary>
    /// Stop auto firing this weapon
    /// </summary>
    public virtual void DisableAutoFire() {
        AutoFiring = false;
    }

    /// <summary>
    /// Attempts to fire this weapon based on cooldown
    /// </summary>
    /// <returns>True only if the weapon fires sucessfully</returns>
    public bool TryFire() {
        if (!OnCooldown && !GameManager.SimPaused) {
            Fire();
            OnCooldown = true;
            return true;
        }
        return false;
    }
    /// <summary>
    /// The fire method, called by the "Try Fire" method, override if this weapon spawns a projectile or similar 
    /// </summary>
    protected virtual void Fire() { }

}
