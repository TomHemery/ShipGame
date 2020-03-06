using UnityEngine;

public class OxygenResourceManager : MonoBehaviour
{
    /// <summary>
    /// Maximum oxygen capacity for this oxygen resource
    /// </summary>
    public float MaxOxygenCapacity { get; private set; } = 100.0f;
    /// <summary>
    /// The current amount of oxygen stored by this resource
    /// </summary>
    public float Oxygen { get; private set; } = 100.0f;

    public bool Full { get; private set; } = true;

    /// <summary>
    /// The base amount of oxygen used per second from this resource
    /// </summary>
    public float baseOxygenUsageRate = 1.0f;

    private void Awake()
    {
        InstantFillOxygen();
    }

    // Update is called once per frame
    void Update()
    {
        if (Oxygen > 0.0f) {
            ReduceOxygen(baseOxygenUsageRate * Time.deltaTime);
        }
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
    }
}
