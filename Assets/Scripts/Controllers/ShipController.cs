using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    public Vector2 Acc { private set; get; } = new Vector2();
    public Rigidbody2D M_Rigidbody { private set; get; }

    float rotation = 0;

    public bool enableThrust = false;
    float thrustForce = 50.0f;
    float dampening = 0.98f;

    protected float maxSpeed = 32;
    protected float minSpeed = 1;

    private void Awake()
    {
        M_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        transform.Rotate(0, 0, rotation);
        if (enableThrust) ApplyForce();
        else M_Rigidbody.velocity = M_Rigidbody.velocity * dampening;
        LimitVelocity();
    }

    private void ApplyForce()
    {
        Acc = VectorMethods.FromDegrees(transform.eulerAngles.z) * thrustForce;
        M_Rigidbody.AddForce(Acc);
    }

    private void LimitVelocity() {
        if (M_Rigidbody.velocity.magnitude > maxSpeed)
        {
            M_Rigidbody.velocity = VectorMethods.SetMagnitude(M_Rigidbody.velocity, maxSpeed);
        }
        else if (M_Rigidbody.velocity.magnitude < minSpeed)
        {
            M_Rigidbody.velocity = VectorMethods.SetMagnitude(M_Rigidbody.velocity, minSpeed);
        }
    }

    public void SetRotation(float r)
    {
        rotation = r;
    }
}
