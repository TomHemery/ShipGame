﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectileWeapon : Weapon
{

    public Transform SpawnPoint;
    public GameObject ProjectilePrefab;

    override protected void Fire() {
        GameObject projectile = Instantiate(ProjectilePrefab, SpawnPoint.position, SpawnPoint.rotation);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>(), true);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.SetAcc(100, transform.rotation.z);
        projectile.GetComponent<Rigidbody2D>().velocity = VectorMethods.FromDegrees(transform.eulerAngles.z) * projectileController.MaxSpeed;
    }
}
