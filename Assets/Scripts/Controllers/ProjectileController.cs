using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    public float MaxSpeed = 50;
    private Vector2 acc;

    private Rigidbody2D mRigidBody;
    public float lifeSpan = 2; //lifespan in s
    private float timeAlive;

    public float damage = 10;

    public GameObject explosionPrefab;

    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (mRigidBody.velocity.magnitude < MaxSpeed)
            mRigidBody.AddForce(acc);
        if (mRigidBody.velocity.magnitude > MaxSpeed)
            mRigidBody.velocity = VectorMethods.SetMagnitude(mRigidBody.velocity, MaxSpeed);
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > lifeSpan)
            Explode(mRigidBody.velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        ShipResourceManager resourceManager = collision.gameObject.GetComponent<ShipResourceManager>();
        if(resourceManager != null)
        {
            resourceManager.DoDamage(damage);
        }
        Rigidbody2D otherRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        Explode(otherRigidbody != null ? otherRigidbody.velocity : mRigidBody.velocity);
    }

    public void SetAcc(float _acc, Vector3 dir) {
        acc = dir * _acc;
    }

    private void Explode(Vector2 velocity) {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<Rigidbody2D>().velocity = velocity;
        Destroy(gameObject);
    }
}
