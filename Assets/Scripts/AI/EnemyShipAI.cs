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
    public float attackRange = 18;

    private float targetSpeed;
    private float distToTarget = float.MaxValue;

    private float avoidRange = 15;
    private ContactFilter2D obstacleAvoidanceFilter;
    private readonly string[] OBSTACLE_AVOID_LAYERS = { "Asteroid", "Ship" };

    private const int SEEK_TARGET_INDEX = 0;
    private const int AVOID_OBSTACLES_INDEX = 1;

    private ControlStruct[] controlStructs = {
        new ControlStruct{ //SEEK TARGET
            direction = new Vector2(),
            speed = 0,
            subsume = true
        },
        new ControlStruct{ //AVOID OBSTACLES
            direction = new Vector2(),
            speed = 0,
            subsume = false
        }
    };

    void Awake()
    {
        controller = GetComponent<ShipController>();
        controller.maxSpeed = maxSpeed;
        controller.minSpeed = minSpeed;
        obstacleAvoidanceFilter = new ContactFilter2D()
        {
            layerMask = LayerMask.GetMask(OBSTACLE_AVOID_LAYERS)
        };
    }

    private void Start()
    {
        Radar.Instance.AddTarget(transform);
    }


    void Update()
    {
        SeekTarget();
        AvoidObstacles();

        for (int i = controlStructs.Length - 1; i >= 0; i--) {
            if (controlStructs[i].subsume) {
                targetSpeed = controlStructs[i].speed;
                transform.up = controlStructs[i].direction;
                controller.thrustMode = controller.M_Rigidbody.velocity.magnitude < targetSpeed ? 
                    ShipController.ThrustMode.Forward : ShipController.ThrustMode.None;
                break;
            }
        }

        //shoot if we're close
        foreach (Weapon w in controller.weapons)
        {
            if (distToTarget < attackRange) w.EnableAutoFire();
            else w.DisableAutoFire();
        }
    }

    private void OnDestroy()
    {
        Radar.Instance.RemoveTarget(transform);
    }

    private void SeekTarget() {
        //find the target
        targetPosition = playerShipTransform.position;

        //work out how fast we want to go
        distToTarget = Vector2.Distance(transform.position, targetPosition);
        float desiredSpeed;
        if (distToTarget < targetDist) desiredSpeed = 0;
        else if (distToTarget > maxDist) desiredSpeed = maxSpeed;
        else desiredSpeed = distToTarget.Map(targetDist, maxDist, minSpeed, maxSpeed);

        controlStructs[SEEK_TARGET_INDEX].direction = targetPosition - transform.position;
        controlStructs[SEEK_TARGET_INDEX].speed = desiredSpeed;
    }

    private void AvoidObstacles() {
        //look for obstacles directly infront of us
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, avoidRange);
    }

    public struct ControlStruct {
        public Vector2 direction;
        public float speed;
        public bool subsume;
    }
}
