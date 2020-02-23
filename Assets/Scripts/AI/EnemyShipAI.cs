using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAI : BasicAI
{
    
    public Transform playerShipTransform;

    private ShipController controller;
    private Vector3 targetPosition;

    public float maxDist = 20;
    public float targetDist = 5;
    public float attackRange = 18;
    public float avoidSpeed = 10;

    private float targetSpeed;
    private float distToTarget = float.MaxValue;
    private Vector2 targetDirection = new Vector2(0, 0);
    private float rotationDeadZone = 5.0f;

    private LayerMask obstacleAvoidanceLayerMask;
    private readonly string[] OBSTACLE_AVOID_LAYERS = { "Obstacle" };

    public Transform frontSensor;
    public Transform frontSensorEnd;
    public Transform leftSensor;
    public Transform leftSensorEnd;
    public Transform rightSensor;
    public Transform rightSensorEnd;
    private float frontSensorRange;
    private float leftSensorRange;
    private float rightSensorRange;

    private const int SEEK_TARGET_INDEX = 0;
    private const int AVOID_OBSTACLES_INDEX = 1;

    private ControlStruct[] controlStructs = {
        new ControlStruct{ //SEEK TARGET
            direction = new Vector2(),
            speed = 0,
            weight = 0.1f
        },
        new ControlStruct{ //AVOID OBSTACLES
            direction = new Vector2(),
            speed = 0,
            weight = 0.0f
        }
    };

    void Awake()
    {
        controller = GetComponent<ShipController>();
        obstacleAvoidanceLayerMask = LayerMask.GetMask(OBSTACLE_AVOID_LAYERS);

        frontSensorRange = (frontSensor.position - frontSensorEnd.position).magnitude;
        leftSensorRange = (leftSensor.position - leftSensorEnd.position).magnitude;
        rightSensorRange = (rightSensor.position - rightSensorEnd.position).magnitude;

        playerShipTransform = GameObject.FindGameObjectWithTag("PlayerShip").transform;

    }

    private void Start()
    {
        Radar.Instance.AddTarget(transform);
    }


    void Update()
    {
        SeekTarget();
        AvoidObstacles();

        targetSpeed = 0;
        targetDirection.Set(0, 0);

        float totalWeight = 0;
        foreach (ControlStruct c in controlStructs) totalWeight += c.weight;

        for (int i = controlStructs.Length - 1; i >= 0; i--) {
            ControlStruct c = controlStructs[i];
            targetSpeed = (targetSpeed * (totalWeight - c.weight) + c.speed * c.weight) / totalWeight;
            targetDirection += (targetDirection * (totalWeight - c.weight) + c.direction * c.weight) / totalWeight;
        }

        controller.thrustMode = controller.M_Rigidbody.velocity.magnitude < targetSpeed ?
                ShipController.ThrustMode.Forward : ShipController.ThrustMode.None;

        controller.desiredRotation = targetDirection;
        if (distToTarget < attackRange)
        {
            RaycastHit2D [] hits = Physics2D.RaycastAll(transform.position, transform.up, attackRange);
            if(hits.Length > 1 && hits[1].collider.gameObject == playerShipTransform.gameObject)
                foreach (Weapon w in controller.weapons) w.EnableAutoFire();
            else
                foreach (Weapon w in controller.weapons) w.DisableAutoFire();
        }
        else
        {
            foreach (Weapon w in controller.weapons) w.DisableAutoFire();
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
        else if (distToTarget > maxDist) desiredSpeed = controller.maxSpeed;
        else desiredSpeed = distToTarget.Map(targetDist, maxDist, controller.minSpeed, controller.maxSpeed);

        //controlStructs[SEEK_TARGET_INDEX].direction = (targetPosition - transform.position);
        Vector3 targetDir = targetPosition - transform.position;
        float angle = Vector2.SignedAngle(transform.up, targetDir);



        if (angle < rotationDeadZone && angle > -rotationDeadZone) controlStructs[SEEK_TARGET_INDEX].direction = targetDir;
        else if (angle > rotationDeadZone) controlStructs[SEEK_TARGET_INDEX].direction = Quaternion.Euler(0, 0, 45) * transform.up;
        else controlStructs[SEEK_TARGET_INDEX].direction = Quaternion.Euler(0, 0, -45) * transform.up;

        controlStructs[SEEK_TARGET_INDEX].speed = desiredSpeed;
    }

    private void AvoidObstacles() {
        //look for obstacles directly infront of us
        //0 = me.......... >:(
        RaycastHit2D [] fronts = Physics2D.RaycastAll(frontSensor.position, frontSensor.up, frontSensorRange, obstacleAvoidanceLayerMask.value);
        RaycastHit2D [] lefts = Physics2D.RaycastAll(leftSensor.position, leftSensor.up, leftSensorRange, obstacleAvoidanceLayerMask.value);
        RaycastHit2D [] rights = Physics2D.RaycastAll(rightSensor.position, rightSensor.up, rightSensorRange, obstacleAvoidanceLayerMask.value);

        if (fronts.Length > 1 || lefts.Length > 1 && rights.Length > 1) //obstacle straight ahead
        {
            float direction = lefts.Length <= 1 ? 1 : -1;
            controlStructs[AVOID_OBSTACLES_INDEX].direction = Quaternion.Euler(0, 0, direction * 45) * transform.up;
            controlStructs[AVOID_OBSTACLES_INDEX].weight = 99.9f;
        }
        else if (lefts.Length > 1)
        {
            controlStructs[AVOID_OBSTACLES_INDEX].direction = Quaternion.Euler(0, 0, -45) * transform.up;
            controlStructs[AVOID_OBSTACLES_INDEX].weight = 99.9f;
        }
        else if (rights.Length > 1)
        {
            controlStructs[AVOID_OBSTACLES_INDEX].direction = Quaternion.Euler(0, 0, 45) * transform.up;
            controlStructs[AVOID_OBSTACLES_INDEX].weight = 99.9f;
        }
        else 
        {
            controlStructs[AVOID_OBSTACLES_INDEX].weight = 0.0f;
        }

        if (controlStructs[AVOID_OBSTACLES_INDEX].weight != 0.0f) controlStructs[AVOID_OBSTACLES_INDEX].speed = avoidSpeed;
    }

    public struct ControlStruct {
        public Vector2 direction;
        public float speed;
        public float weight;
    }
}
