using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHitscanWeapon : Weapon
{
    public Transform SpawnPoint;

    public float damage;
    public float range;
    public AnimationCurve graphicFadeCurve;

    public SpriteRenderer m_graphic;

    public override void Awake()
    {
        base.Awake();
        m_graphic.gameObject.SetActive(false);
    }

    protected override void Fire()
    {
        if (mAudioSource != null)
        {
            mAudioSource.Play();
        }
        RaycastHit2D hit = Physics2D.Raycast(SpawnPoint.position, SpawnPoint.up, range);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<HealthResource>() != null)
            {
                hit.collider.gameObject.GetComponent<HealthResource>().DoDamage(damage);
            }
            StartCoroutine(ShowGraphic(((Vector2)SpawnPoint.position - (Vector2)hit.collider.transform.position).magnitude));
        }
        else
            StartCoroutine(ShowGraphic(range));
    }

    private IEnumerator ShowGraphic(float length) {
        m_graphic.gameObject.SetActive(true);
        m_graphic.transform.localScale = new Vector3(m_graphic.transform.localScale.x, length, m_graphic.transform.localScale.z);
        float time = 0.0f;
        float alpha;
        do {
            time += Time.deltaTime;
            alpha = graphicFadeCurve.Evaluate(time);
            m_graphic.color = new Color(m_graphic.color.r, m_graphic.color.g, m_graphic.color.b, alpha);
            yield return null;
        }
        while (alpha > 0);
        m_graphic.gameObject.SetActive(false);
    }
}
