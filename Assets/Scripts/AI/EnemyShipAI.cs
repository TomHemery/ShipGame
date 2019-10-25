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
    private float targetDist = 4;
    private float speed;

    void Awake()
    {
        controller = GetComponent<ShipController>();
    }

    void Update()
    {

        targetPosition = playerShipTransform.position;

        float dist = Vector2.Distance(transform.position, targetPosition);

        if (dist < targetDist) speed = 0;
        else if (dist > maxDist) speed = maxSpeed;
        else {
            speed = dist.Map(targetDist, maxDist, minSpeed, maxSpeed);
        }

        controller.SetTargetSpeed(speed, minSpeed, maxSpeed);
        transform.right = targetPosition - transform.position;

    }

}
