using System;
using UnityEngine;

public class HealthResourceManager : Resource
{
    public float Health { get; private set; }
    public float MaxHealth = 100;

    public GameObject explosionPrefab = null;

    public event EventHandler<HealthChangedEventArgs> HealthValueChangedEvent;

    private bool exploded = false;

    public bool isPlayerHealth = false;

    public event EventHandler OnExploded;

    private void Start()
    {
        SetHealth(MaxHealth);
    }

    /// <summary>
    /// Sets health to the value of h (health is constrained between 0 and max)
    /// </summary>
    /// <param name="h">The value to set health to</param>
    public void SetHealth(float h)
    {
        Health = Mathf.Clamp(h, 0, MaxHealth);
        HealthValueChangedEvent?.Invoke(this, new HealthChangedEventArgs(Health, MaxHealth));
        if (Health <= 0) Explode();
    }

    /// <summary>
    /// Modifies health by the value of h (health is constrained between 0 and max)
    /// </summary>
    /// <param name="h">The value to modify h by</param>
    public void ModifyHealth(float h)
    {
        SetHealth(Health + h);
    }

    /// <summary>
    /// Increases health by the absolute value of h (health cannot go above max)
    /// </summary>
    /// <param name="h">The value to increase health by</param>
    public void AddHealth(float h)
    {
        ModifyHealth(Mathf.Abs(h));
    }

    /// <summary>
    /// Reduces health by the absolute value of h (health cannot go below 0)
    /// </summary>
    /// <param name="h">The value to reduce health by</param>
    public void ReduceHealth(float h)
    {
        ModifyHealth(-Mathf.Abs(h));
    }

    /// <summary>
    /// Reduces health by the absolute value of d
    /// </summary>
    /// <param name="d">The amount of damage to do</param>
    public virtual void DoDamage(float d)
    {
        ReduceHealth(d);
    }

    protected void Explode()
    {
        if (!exploded)
        {
            if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            LootDrops mLootDrops = GetComponent<LootDrops>();
            if (mLootDrops != null) mLootDrops.DropLoot();
            if (isPlayerHealth)
            {
                GameManager.Instance.OnPlayerDeath(PlayerDeathTypes.Exploded);
            }
            else
            {
                Destroy(gameObject);
            }
            exploded = true;
            OnExploded?.Invoke(this, null);
        }
    }

    public override void UpdateResource()
    {
        return;
    }
}

public class HealthChangedEventArgs : EventArgs
{
    public HealthChangedEventArgs(float _newHealth, float _maxHealth) { NewHealth = _newHealth; MaxHealth = _maxHealth; }
    public float NewHealth { get; } 
    public float MaxHealth { get; }
}
