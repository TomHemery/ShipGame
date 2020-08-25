using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//AI responsible for controlling enemy ships
public class EnemyShipAI : BasicAI
{
    //reference to player ship
    public Transform playerShipTransform;

    //ship controller we control
    private ShipController controller;
    //where we want to go to
    private Vector3 targetPosition;

    //parameters
    public float maxDist = 20;
    public float targetDist = 5;
    public float attackRange = 18;
    public float avoidSpeed = 10;

    private float targetSpeed;
    private float distToTarget = float.MaxValue;
    private Vector2 targetDirection = new Vector2(0, 0);
    private float rotationDeadZone = 5.0f;

    //layer mask for raycasts
    private LayerMask obstacleAvoidanceLayerMask;
    private readonly string[] OBSTACLE_AVOID_LAYERS = { "Obstacle" };

    //transform references for raycasts
    public Transform frontSensor;
    public Transform frontSensorEnd;
    public Transform leftSensor;
    public Transform leftSensorEnd;
    public Transform rightSensor;
    public Transform rightSensorEnd;
    private float frontSensorRange;
    private float leftSensorRange;
    private float rightSensorRange;

    //indexes for behaviours
    private const int SEEK_TARGET_INDEX = 0;
    private const int AVOID_OBSTACLES_INDEX = 1;

    //control struct for behaviours
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
        //get reference to controller
        controller = GetComponent<ShipController>();
        //create layer mask
        obstacleAvoidanceLayerMask = LayerMask.GetMask(OBSTACLE_AVOID_LAYERS);

        //work out sensor ranges
        frontSensorRange = (frontSensor.position - frontSensorEnd.position).magnitude;
        leftSensorRange = (leftSensor.position - leftSensorEnd.position).magnitude;
        rightSensorRange = (rightSensor.position - rightSensorEnd.position).magnitude;

        //get reference to player ship
        playerShipTransform = GameObject.FindGameObjectWithTag("PlayerShip").transform;
    }

    private void Start()
    {
        //add tracker to player radar
        Radar.Instance.AddTarget(transform);
    }


    void Update()
    {
        //perform behaviours
        SeekTarget();
        AvoidObstacles();

        //reset targets
        targetSpeed = 0;
        targetDirection.Set(0, 0);

        //assign outputs of behaviours to control based on weighting (essentially obstacle avoid subsumes seek target)
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

        //if within attack range, shoot player
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
        //remove tracker from radar
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

    //control structure, loosely based on Brooke's subsumption architecture, but uses behaviour fusion with a weighted contribution (thanks ARS)
    public struct ControlStruct {
        public Vector2 direction;
        public float speed;
        public float weight;
    }
}
