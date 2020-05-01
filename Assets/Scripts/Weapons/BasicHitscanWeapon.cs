using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHitscanWeapon : Weapon
{
    public Transform SpawnPoint;

    public float damage;
    public float range;

    public GameObject graphicPrefab;

    [SerializeField]
    protected LayerMask hitscanLayerMask;

    public override void Awake()
    {
        base.Awake();
    }

    protected override void Fire()
    {
        if (mAudioSource != null)
        {
            mAudioSource.Play();
        }

        int currentLayer = transform.root.gameObject.layer;
        transform.root.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        RaycastHit2D hit = Physics2D.Raycast(SpawnPoint.position, SpawnPoint.up, range, hitscanLayerMask);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<HealthResource>() != null)
            {
                hit.collider.gameObject.GetComponent<HealthResource>().DoDamage(damage);
            }
            ShowGraphic(((Vector2)SpawnPoint.position - (Vector2)hit.collider.transform.position).magnitude);
        }
        else
            ShowGraphic(range);

        transform.root.gameObject.layer = currentLayer;
    }

    private void ShowGraphic(float length) {
        GameObject graphicObject = Instantiate(graphicPrefab, SpawnPoint.position + SpawnPoint.up * length, SpawnPoint.rotation);
        SpriteRenderer graphicSprite = graphicObject.GetComponent<SpriteRenderer>();
        graphicSprite.size = new Vector2(graphicSprite.size.x, length);
    }
}
