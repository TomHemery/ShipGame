using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    Vector2 pos = new Vector2();
    Vector2 vel = new Vector2();
    Vector2 acc = new Vector2();

    float rotation = 0;

    float dampening = 0.005f;
    float baseAcc = 50.0f;

    float maxSpeed = 32;
    float minSpeed = 1;
    float targetSpeed;

    bool firstUpdate = false;

    private void Awake()
    {
        targetSpeed = minSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (firstUpdate) firstUpdate = false;
        else
        {
            transform.Rotate(0, 0, rotation);
            ApplyForce();
            transform.position = pos;
        }
    }
    private void ApplyForce() {
        vel += (vel * -1 * dampening) * Time.deltaTime;
        acc = VectorMethods.FromDegrees(transform.eulerAngles.z) * baseAcc;
        vel += acc * Time.deltaTime;
        if (vel.magnitude > targetSpeed)
        { 
            vel = VectorMethods.SetMagnitude(vel, targetSpeed);
        }
        pos += vel * Time.deltaTime;
        acc.Set(0, 0);
    }

    public void SetRotation(float r) {
        rotation = r;
    }

    public void SetTargetSpeed(float s, float min, float max) {
        targetSpeed = s.Map(min, max, minSpeed, maxSpeed);
    }
}
