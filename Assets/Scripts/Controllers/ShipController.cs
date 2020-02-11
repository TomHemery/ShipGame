using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

    public Vector2 Acc { private set; get; } = new Vector2();
    public Rigidbody2D M_Rigidbody { private set; get; }

    float rotation = 0;

    public ThrustMode thrustMode = ThrustMode.None;
    float forwardThrustForce = 50.0f;
    float activeDampening = 0.95f;
    float passiveDampening = 0.99f;

    protected float maxSpeed = 32;
    protected float minSpeed = 1;

    private void Awake()
    {
        M_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        M_Rigidbody.velocity = VectorMethods.FromDegrees(transform.eulerAngles.z);
    }

    void FixedUpdate()
    {
        transform.Rotate(0, 0, rotation);

        switch (thrustMode) {
            case ThrustMode.Forward:
                ApplyForwardThrust(forwardThrustForce);
                break;
            case ThrustMode.Backward:
                M_Rigidbody.velocity = M_Rigidbody.velocity * activeDampening;
                break;
            case ThrustMode.None:
                M_Rigidbody.velocity = M_Rigidbody.velocity * passiveDampening;
                break;
        }

        LimitVelocity();
    }

    private void ApplyForwardThrust(float thrust)
    {
        Acc = VectorMethods.FromDegrees(transform.eulerAngles.z) * thrust;
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

    public enum ThrustMode {
        Forward,
        Backward,
        None
    }
}
