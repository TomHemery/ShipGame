using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private SpriteRenderer m_renderer;
    private BoxCollider2D m_collider;
    private Collider2D otherCollider = null;

    private float length = 0;
    private readonly float lengthChangeRate = 20f;
    private float distanceToCollision = -1;

    public float maxRange = 10;
    public float dps = 50;

    public GameObject impactEffectPrefab;
    private GameObject impactEffect;

    private void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(transform.root.GetComponent<Collider2D>(), m_collider, true);
        impactEffect = Instantiate(impactEffectPrefab, transform);
        impactEffect.SetActive(false);
    }

    private void OnDisable()
    {
        length = 0;
        m_renderer.size = new Vector2(m_renderer.size.x, length);
        m_collider.size = new Vector2(m_collider.size.x, length);
        distanceToCollision = -1;
        impactEffect.SetActive(false);
    }

    private void Update()
    {
        float targetLength;
        if (distanceToCollision < 0 || otherCollider == null)
        {
            impactEffect.SetActive(false);
            targetLength = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
            targetLength = distanceToCollision + 1.0f;
            impactEffect.SetActive(true);
            impactEffect.transform.position = transform.position + transform.up * targetLength;
        }
        
        targetLength = targetLength < maxRange ? targetLength : maxRange;

        if (length < targetLength) length += Time.deltaTime * lengthChangeRate;
        if (length > targetLength) length = targetLength;

        m_renderer.size = new Vector2(m_renderer.size.x, length);
        m_collider.size = new Vector2(m_collider.size.x, length);
        m_collider.offset = new Vector2(m_collider.offset.x, length / 2);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HealthResourceManager hrm = other.gameObject.GetComponent<HealthResourceManager>();
        if(hrm != null)hrm.DoDamage(dps * Time.deltaTime);
        distanceToCollision = Vector2.Distance(transform.position, other.ClosestPoint(transform.position));
        otherCollider = other;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        distanceToCollision = -1;
        otherCollider = null;
    }

}
