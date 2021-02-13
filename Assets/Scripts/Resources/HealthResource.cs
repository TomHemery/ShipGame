using System;
using UnityEngine;

public class HealthResource : MonoBehaviour
{
    public const float DEFAULT_MAX_HEALTH = 100;

    public float MaxHealthValue = DEFAULT_MAX_HEALTH;
    public float HealthValue;

    public GameObject explosionPrefab = null;
    public event EventHandler<HealthChangedEventArgs> HealthValueChangedEvent;
    public bool exploded = false;
    public bool isPlayerHealth = false;
    public event EventHandler OnExploded;

    protected virtual void Awake()
    {
        FillResource();
    }

    protected virtual void Update() {}

    /// <summary>
    /// Sets health value to the value of h (health is constrained between 0 and max)
    /// </summary>
    /// <param name="h">The value to set health to</param>
    public void SetHealth(float h)
    {
        HealthValue = Mathf.Clamp(h, 0, MaxHealthValue);
        HealthValueChangedEvent?.Invoke(this, new HealthChangedEventArgs(HealthValue, MaxHealthValue));
        if (HealthValue <= 0) Explode();
    }

    public void SetMaxHealth(float mh) {
        MaxHealthValue = Mathf.Clamp(mh, 0, float.MaxValue);
        SetHealth(HealthValue);
    }

    public void AddHealth(float h) {
        SetHealth(HealthValue + Mathf.Abs(h));
    }

    public void SubtractHealth(float h) {
        SetHealth(HealthValue - Mathf.Abs(h));
    }

    public void ModifyHealth(float h) {
        SetHealth(HealthValue + h);
    }

    public virtual void FillResource() {
        SetHealth(MaxHealthValue);
    }

    public bool IsFull() {
        return HealthValue >= MaxHealthValue;
    }

    public bool IsEmpty(){
        return HealthValue <= 0;
    }

    /// <summary>
    /// Reduces health value by the absolute value of d
    /// </summary>
    /// <param name="d">The amount of damage to do</param>
    public virtual void DoDamage(float d)
    {
        SubtractHealth(d);
    }

    protected void Explode()
    {
        if (!exploded)
        {
            if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            LootDropTable mLootDrops = GetComponent<LootDropTable>();
            if (mLootDrops != null) mLootDrops.DropLoot();
            if (isPlayerHealth)
            {
                GameManager.Instance.KillPlayerBy(PlayerDeathTypes.Exploded);
            }
            else
            {
                Destroy(gameObject);
            }
            exploded = true;
            OnExploded?.Invoke(this, null);
        }
    }
}

public class HealthChangedEventArgs : EventArgs
{
    public HealthChangedEventArgs(float _newHealth, float _maxHealth) { NewHealth = _newHealth; MaxHealth = _maxHealth; }
    public float NewHealth { get; } 
    public float MaxHealth { get; }
}
