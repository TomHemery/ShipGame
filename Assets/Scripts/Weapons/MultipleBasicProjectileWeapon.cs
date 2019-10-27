using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleBasicProjectileWeapon : Weapon
{

    public Transform [] SpawnPoints;
    public GameObject ProjectilePrefab;
    private Rigidbody2D mRigidbody;
    private int index = 0;

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody2D>();
    }

    override protected void Fire()
    {
        Transform SpawnPoint = SpawnPoints[index];
        GameObject projectile = Instantiate(ProjectilePrefab, SpawnPoint.position, SpawnPoint.rotation);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>(), true);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.SetAcc(100, transform.rotation.z);
        //projectile.GetComponent<Rigidbody2D>().velocity = mRigidbody.velocity;
        index = index < SpawnPoints.Length - 1 ? index + 1 : 0;
    }
}
