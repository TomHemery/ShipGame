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
    private float speed;

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
        targetPosition = playerShipTransform.position;

        distToTarget = Vector2.Distance(transform.position, targetPosition);

        if (distToTarget < targetDist) speed = 0;
        else if (distToTarget > maxDist) speed = maxSpeed;
        else {
            speed = distToTarget.Map(targetDist, maxDist, minSpeed, maxSpeed);
        }

        controller.SetTargetSpeed(speed, minSpeed, maxSpeed);
        transform.right = targetPosition - transform.position;

        Weapon.DoAutoFire = distToTarget < attackRange;
    }

    private void OnDestroy()
    {
        Radar.Instance.RemoveTarget(transform);
    }
}
