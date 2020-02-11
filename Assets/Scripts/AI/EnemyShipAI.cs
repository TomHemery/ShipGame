using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAI : BasicAI
{
    
    public Transform playerShipTransform;

    private ShipController controller;
    private Vector3 targetPosition;

    public float minSpeed = 1;
    public float maxSpeed = 28;
    public float maxDist = 20;
    public float targetDist = 5;
    private float targetSpeed;

    private float distToTarget = float.MaxValue;

    public float attackRange = 18;

    void Awake()
    {
        controller = GetComponent<ShipController>();
        controller.maxSpeed = maxSpeed;
        controller.minSpeed = minSpeed;
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
        controller.thrustMode = controller.M_Rigidbody.velocity.magnitude < targetSpeed ? ShipController.ThrustMode.Forward : ShipController.ThrustMode.None;

        //point at the player
        transform.up = targetPosition - transform.position;

        //shoot if we're close
        foreach (Weapon w in controller.weapons)
        {
            w.DoAutoFire = distToTarget < attackRange;
        }
    }

    private void OnDestroy()
    {
        Radar.Instance.RemoveTarget(transform);
    }
}
