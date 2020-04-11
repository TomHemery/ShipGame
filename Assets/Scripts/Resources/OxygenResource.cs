using UnityEngine;

public class OxygenResource : Resource
{

    /// <summary>
    /// The base amount of oxygen used per second by this resource
    /// </summary>
    public float baseOxygenUsageRate = 1.0f;

    protected override void Awake()
    {
        FillResource();
    }

    /// <summary>
    /// Sets the value of stored oxygen to o, never update oxygen variable directly in any other way
    /// </summary>
    /// <param name="o"></param>
    public override void SetResource(float v) {
        Value = Mathf.Clamp(v, 0, MaxValue);
        if (Value == 0.0f) OutOfOxygen();
    }

    private void OutOfOxygen() {
        GameManager.Instance.OnPlayerDeath(PlayerDeathTypes.OutOfOxygen);
    }

    public override void UpdateResource()
    {
        if (Value > 0.0f)
        {
            SubtractResource(baseOxygenUsageRate * Time.deltaTime);
        }
    }
}
