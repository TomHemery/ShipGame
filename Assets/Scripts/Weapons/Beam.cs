using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Collider2D otherCollider = null;

    private float length = 0;
    private readonly float lengthChangeRate = 50f;
    private float distanceToCollision = -1;

    public float maxRange = 10;
    public float dps = 50;

    public GameObject impactEffectPrefab;
    private GameObject impactEffect;

    private Material material;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(transform.root.GetComponent<Collider2D>(), boxCollider, true);
        impactEffect = Instantiate(impactEffectPrefab, transform);
        impactEffect.SetActive(false);
        material = spriteRenderer.material;
    }

    private void OnDisable()
    {
        length = 0;
        boxCollider.offset = new Vector2(boxCollider.offset.x, length / 2);
        boxCollider.size = new Vector2(boxCollider.size.x, length);
        material.SetFloat("_Length", length * 1.2f);
        distanceToCollision = -1;
        impactEffect.SetActive(false);
    }

    private void Update()
    {
        float targetLength;
        if (distanceToCollision < 0 || otherCollider == null)
        {
            impactEffect.SetActive(false);
            targetLength = maxRange;
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

        boxCollider.size = new Vector2(boxCollider.size.x, length);
        boxCollider.offset = new Vector2(boxCollider.offset.x, length / 2);
        material.SetFloat("_Length", length * 1.2f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Collectable"))
        {
            HealthResource hrm = other.gameObject.GetComponent<HealthResource>();
            if (hrm != null) hrm.DoDamage(dps * Time.deltaTime);
            distanceToCollision = Vector2.Distance(transform.position, other.ClosestPoint(transform.position));
            otherCollider = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        distanceToCollision = -1;
        otherCollider = null;
    }

}
