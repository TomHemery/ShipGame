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

    // Update is called once per frame
    void Update()
    {
        if (mRigidBody.velocity.magnitude < MaxSpeed)
            mRigidBody.AddForce(acc);
        if (mRigidBody.velocity.magnitude > MaxSpeed)
            mRigidBody.velocity = VectorMethods.SetMagnitude(mRigidBody.velocity, MaxSpeed);

        timeAlive += Time.deltaTime;
        if (timeAlive > lifeSpan)
            Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        if(collision.gameObject.tag != "Projectile")
        {
            ShipResourceManager resourceManager = collision.gameObject.GetComponent<ShipResourceManager>();
            if(resourceManager != null)
            {
                resourceManager.DoDamage(damage);
            }
            Explode();
        }
    }

    public void SetAcc(float _acc, float h) {
        transform.Rotate(0, 0, h);
        acc = VectorMethods.FromDegrees(transform.eulerAngles.z) * _acc;
    }

    private void Explode() {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
