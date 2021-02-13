using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanGraphic : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public float lifeTime = 1.0f;
    private float timeAlive;

    public GameObject impactEffectPrefab;

    private float maxLength = -1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Instantiate(impactEffectPrefab, transform);
    }

    private void Update()
    {
        if (maxLength < 0) maxLength = spriteRenderer.size.y;

        timeAlive += Time.deltaTime;

        Color c = spriteRenderer.color;
        c.a = Mathf.Lerp(1, 0, timeAlive / lifeTime);
        spriteRenderer.color = c;

        spriteRenderer.size = new Vector2(spriteRenderer.size.x, maxLength * c.a);

        if (timeAlive > lifeTime) {
            Destroy(gameObject);
        }
    }
}
