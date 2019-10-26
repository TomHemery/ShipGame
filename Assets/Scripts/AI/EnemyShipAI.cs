using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAI : BasicAI
{
    private ShipController controller;
    public Transform playerShipTransform;
    private Vector3 targetPosition;

    private float minSpeed = 1;
    private float maxSpeed = 20;
    private float maxDist = 20;
    private float targetDist = 5;
    private float speed;

    private float distToTarget = float.MaxValue;

    private float attackRange = 18;
    private Transform projectileSpawnLeft;
    private Transform projectileSpawnRight;
    private bool shootRight = true;
    public GameObject projectilePrefab;

    void Awake()
    {
        controller = GetComponent<ShipController>();
        projectileSpawnLeft = transform.Find("ProjectileSpawnLeft");
        projectileSpawnRight = transform.Find("ProjectileSpawnRight");
    }

    private void Start()
    {
        InvokeRepeating("FireProjectile", 0.0f, 0.8f);
    }

    void Update()
    {

        targetPosition = playerShipTransform.position;

        distToTarget = Vector2.Distance(transform.position, targetPosition);

        if (distToTarget < targetDist) speed = 0;
        else if (distToTarget > maxDist) speed = maxSpeed;
        else {
            speed = distToTarget.Map(targetDist, maxDist, minSpeed, maxSpeed);
        }

        controller.SetTargetSpeed(speed, minSpeed, maxSpeed);
        transform.right = targetPosition - transform.position;

    }

    void FireProjectile()
    {
        if (distToTarget < attackRange)
        {
            Transform spawnPoint = shootRight ? projectileSpawnRight : projectileSpawnLeft;
            
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            projectileController.SetAcc(100, transform.rotation.z);
            projectile.GetComponent<Rigidbody2D>().velocity = controller.mRigidbody.velocity;

            shootRight = !shootRight;
        }        
    }

}
