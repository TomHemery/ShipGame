using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : Controller
{

    public Vector2 Acc { private set; get; } = new Vector2();
    public Rigidbody2D M_Rigidbody { private set; get; }

    public Vector2 desiredRotation = new Vector2();
    public float rotationSpeed = 10;

    [HideInInspector]
    public ThrustMode thrustMode = ThrustMode.None;
    float forwardThrustForce = 50.0f;
    float activeDampening = 0.95f;
    float passiveDampening = 0.99f;

    public float maxSpeed = 32;
    public float minSpeed = 1;

    [HideInInspector]
    public List<Weapon> weapons = new List<Weapon>();

    public AudioSource engineAudioSource;

    private void Awake()
    {
        M_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        M_Rigidbody.velocity = transform.up;
    }

    void FixedUpdate()
    {
        if (!GameManager.SimPaused)
        {
            float angleDelta = Vector2.SignedAngle(transform.up, desiredRotation);

            transform.Rotate(0, 0, angleDelta * Time.fixedDeltaTime * rotationSpeed);

            switch (thrustMode)
            {
                case ThrustMode.Forward:
                    ApplyForwardThrust(forwardThrustForce);
                    if (!engineAudioSource.isPlaying)
                    {
                        engineAudioSource.Play();
                    }
                    break;
                case ThrustMode.Backward:
                    M_Rigidbody.velocity = M_Rigidbody.velocity * activeDampening;
                    if (engineAudioSource.isPlaying) engineAudioSource.Stop();
                    break;
                case ThrustMode.None:
                    M_Rigidbody.velocity = M_Rigidbody.velocity * passiveDampening;
                    if (engineAudioSource.isPlaying) engineAudioSource.Stop();
                    break;
            }

            LimitVelocity();
        }
        else
        {
            if (engineAudioSource.isPlaying) engineAudioSource.Stop();
        }
    }

    private void ApplyForwardThrust(float thrust)
    {
        Acc = transform.up * thrust;
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

    public enum ThrustMode {
        Forward,
        Backward,
        None
    }
}
