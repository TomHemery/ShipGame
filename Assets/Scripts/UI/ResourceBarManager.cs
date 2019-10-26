using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBarManager : MonoBehaviour
{
    public RectTransform HealthBar;
    public RectTransform ShieldBar;

    Vector3 healthBarScale = new Vector3(1, 1, 1);
    Vector3 shieldBarScale = new Vector3(1, 1, 1);

    public void UpdateHealthBar(float h, float maxH) {
        healthBarScale.x = h.Map(0, maxH, 0, 1);
        HealthBar.localScale = healthBarScale;
    }

    public void UpdateShieldBar(float s, float maxS) {
        shieldBarScale.x = s.Map(0, maxS, 0, 1);
        ShieldBar.localScale = shieldBarScale;
    }
}