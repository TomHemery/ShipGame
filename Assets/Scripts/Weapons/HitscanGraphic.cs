using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanGraphic : MonoBehaviour
{
    private SpriteRenderer m_renderer;

    public float lifeTime = 1.0f;
    private float timeAlive;

    public GameObject impactEffectPrefab;

    private float maxLength = -1;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        Instantiate(impactEffectPrefab, transform);
    }

    private void Update()
    {
        if (maxLength < 0) maxLength = m_renderer.size.y;

        timeAlive += Time.deltaTime;

        Color c = m_renderer.color;
        c.a = Mathf.Lerp(1, 0, timeAlive / lifeTime);
        m_renderer.color = c;

        m_renderer.size = new Vector2(m_renderer.size.x, maxLength * c.a);

        if (timeAlive > lifeTime) {
            Destroy(gameObject);
        }
    }
}
