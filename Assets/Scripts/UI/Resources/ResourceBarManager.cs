using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBarManager : MonoBehaviour
{
    public RectTransform HealthBar;
    public RectTransform ShieldBar;

    Vector3 healthBarScale = new Vector3(1, 1, 1);
    Vector3 shieldBarScale = new Vector3(1, 1, 1);

    public HealthAndShieldsResource healthAndShieldsResource;
    public HealthResource healthResource;

    public GameObject shieldsDisabledWarning;

    public bool attachToPlayer = false;

    private void Awake()
    {
        if (attachToPlayer)
        {
            healthAndShieldsResource =
                GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<HealthAndShieldsResource>();
        }
        //if (shieldsDisabledWarning != null) shieldsDisabledWarning.SetActive(false);
    }

    private void OnEnable()
    {
        if (healthAndShieldsResource != null)
        {
            healthAndShieldsResource.HealthValueChangedEvent += OnHealthChanged;
            healthAndShieldsResource.ShieldValueChangedEvent += OnShieldsChanged;
            //set initial values
            UpdateHealthBar(healthAndShieldsResource.HealthValue, healthAndShieldsResource.MaxHealthValue);
            UpdateShieldBar(healthAndShieldsResource.ShieldValue, healthAndShieldsResource.MaxShieldValue);
        }
        else if (healthResource != null) {
            healthResource.HealthValueChangedEvent += OnHealthChanged;
            //set initial values
            UpdateHealthBar(healthResource.HealthValue, healthResource.MaxHealthValue);
        }
    }

    private void OnDisable()
    {
        if (healthAndShieldsResource != null)
        {
            healthAndShieldsResource.HealthValueChangedEvent -= OnHealthChanged;
            healthAndShieldsResource.ShieldValueChangedEvent -= OnShieldsChanged;
        }
        else if (healthResource != null)
        {
            healthResource.HealthValueChangedEvent -= OnHealthChanged;
        }
    }

    public void OnHealthChanged(object sender, HealthChangedEventArgs e)
    {
        UpdateHealthBar(e.NewHealth, e.MaxHealth);
    }

    public void OnShieldsChanged(object sender, ShieldChangedEventArgs e)
    {
        UpdateShieldBar(e.NewShields, e.MaxShields);
    }

    public void UpdateHealthBar(float h, float maxH) {
        if (HealthBar != null)
        {
            healthBarScale.x = h.Map(0, maxH, 0, 1);
            HealthBar.localScale = healthBarScale;
        }
    }

    public void UpdateShieldBar(float s, float maxS) {
        if (ShieldBar != null)
        {
            shieldBarScale.x = s.Map(0, maxS, 0, 1);
            ShieldBar.localScale = shieldBarScale;
        }
        if (shieldsDisabledWarning != null) {
            shieldsDisabledWarning.SetActive(healthAndShieldsResource.ShieldsDisabled);
        }
    }
}