using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectileWeapon : Weapon
{

    public Transform SpawnPoint;
    public GameObject ProjectilePrefab;

    /// <summary>
    /// Fires the weapon
    /// </summary>
    override protected void Fire() {
        if (mAudioSource != null) {
            mAudioSource.Play();
        }
        //instantiate the projectile at the spawn point
        GameObject projectile = Instantiate(ProjectilePrefab, SpawnPoint.position, SpawnPoint.rotation);
        //prevent collisions between this bullet and the spawning ship
        Physics2D.IgnoreCollision(parentShip_controller.GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>(), true);
        //get a reference to the projectile controller
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        //set the acceleration of the controller in the "up" direction
        projectileController.SetAcc(100, transform.up);
        //set the initial velocity of the projectile to be the velocity of the spawning ship
        projectile.GetComponent<Rigidbody2D>().velocity = parentShip_controller.M_Rigidbody.velocity;
    }
}