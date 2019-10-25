using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayerShip : MonoBehaviour
{

    public Vector2 acc { private set; get; } = new Vector2();
    public Rigidbody2D rigidBody { private set; get; }

    float rotation = 0;

    float baseAcc = 50.0f;

    float maxSpeed = 32;
    float minSpeed = 1;
    float targetSpeed;

    bool firstUpdate = false;

    private void Awake()
    { 
        rigidBody = GetComponent<Rigidbody2D>();
        targetSpeed = minSpeed; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (firstUpdate) firstUpdate = false;
        else
        {
            transform.Rotate(0, 0, rotation);
            ApplyForce();
        }
    }
    private void ApplyForce() {
        acc = VectorMethods.FromDegrees(transform.eulerAngles.z) * baseAcc;
        rigidBody.AddForce(acc);
        //rigidBody.velocity += acc;
        if (rigidBody.velocity.magnitude > targetSpeed)
        {
            rigidBody.velocity = VectorMethods.SetMagnitude(rigidBody.velocity, targetSpeed);
        }
        acc.Set(0, 0);
    }

    public void SetRotation(float r) {
        rotation = r;
    }

    public void SetTargetSpeed(float s, float min, float max) {
        targetSpeed = s.Map(min, max, minSpeed, maxSpeed);
    }
}
