using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAnimation : MonoBehaviour
{
    public HealthResourceManager healthResourceManager;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private void Update()
    {
        if (spriteRenderer != null && healthResourceManager != null) {
            float index = healthResourceManager.Health;
            index = index.Map(0, healthResourceManager.MaxHealth, sprites.Length - 1, 0);
            spriteRenderer.sprite = sprites[Mathf.RoundToInt(index)];
        }
    }
}
