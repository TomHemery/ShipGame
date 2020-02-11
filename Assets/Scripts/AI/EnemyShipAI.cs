using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAI : BasicAI
{
    
    public Transform playerShipTransform;
    public Weapon Weapon;

    private ShipController controller;
    private Vector3 targetPosition;

    private float minSpeed = 1;
    private float maxSpeed = 20;
    private float maxDist = 20;
    private float targetDist = 5;
    private float targetSpeed;

    private float distToTarget = float.MaxValue;

    private float attackRange = 18;

    void Awake()
    {
        controller = GetComponent<ShipController>();
    }

    private void Start()
    {
        Radar.Instance.AddTarget(transform);
    }


    void Update()
    {
        //find the target
        targetPosition = playerShipTransform.position;

        //work out how fast we want to go
        distToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distToTarget < targetDist) targetSpeed = 0;
        else if (distToTarget > maxDist) targetSpeed = maxSpeed;
        else {
            targetSpeed = distToTarget.Map(targetDist, maxDist, minSpeed, maxSpeed);
        }

        //move toward player (aiming for target speed)
        controller.enableThrust = controller.M_Rigidbody.velocity.magnitude < targetSpeed;

        //point at the player
        transform.right = targetPosition - transform.position;

        //shoot if we're close
        Weapon.DoAutoFire = distToTarget < attackRange;
    }

    private void OnDestroy()
    {
        Radar.Instance.RemoveTarget(transform);
    }
}
