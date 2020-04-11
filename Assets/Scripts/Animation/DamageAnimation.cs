using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAnimation : MonoBehaviour
{
    public HealthResource healthResource;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private void Update()
    {
        if (spriteRenderer != null && healthResource != null) {
            float index = healthResource.HealthValue;
            index = index.Map(0, healthResource.MaxHealthValue, sprites.Length - 1, 0);
            spriteRenderer.sprite = sprites[Mathf.RoundToInt(index)];
        }
    }
}
