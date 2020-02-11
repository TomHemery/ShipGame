using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleBasicProjectileWeapon : Weapon
{

    public Transform [] SpawnPoints;
    public GameObject ProjectilePrefab;
    private int index = 0;

    override protected void Fire()
    {
        Transform SpawnPoint = SpawnPoints[index];
        GameObject projectile = Instantiate(ProjectilePrefab, SpawnPoint.position, SpawnPoint.rotation);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>(), true);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.SetAcc(100, transform.up);
        projectile.GetComponent<Rigidbody2D>().velocity = transform.up * projectileController.MaxSpeed;
        index = index < SpawnPoints.Length - 1 ? index + 1 : 0;
    }
}
