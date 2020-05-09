using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Controller
{

    public float MaxSpeed = 50;
    private Vector2 acc;

    private Rigidbody2D mRigidBody;
    public float lifeSpan = 2; //lifespan in s
    private float timeAlive; //time this projectile has been alive

    public float damage = 10; //damage on hit, also used as duration for emp 

    public GameObject explosionPrefab; //prefab spawned on explode

    public ProjectileType projectileType = ProjectileType.standard; //for special projectiles
    
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
        if (!GameManager.SimPaused)
        {
            timeAlive += Time.deltaTime;
            if (timeAlive > lifeSpan)
                Explode(mRigidBody.velocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthResource resourceManager = collision.gameObject.GetComponent<HealthResource>();
        if(resourceManager != null)
        {
            if (projectileType == ProjectileType.emp && resourceManager.GetType() == typeof(HealthAndShieldsResource)) {
                ((HealthAndShieldsResource)resourceManager).DisableShieldsForDuration(damage);
            }
            else if (projectileType == ProjectileType.standard) {
                resourceManager.DoDamage(damage);
            }
        }
        Rigidbody2D otherRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
        Explode(otherRigidbody != null ? otherRigidbody.velocity : mRigidBody.velocity);
    }

    public void SetAcc(float _acc, Vector3 dir) {
        acc = dir * _acc;
    }

    /// <summary>
    /// Explode this projectile
    /// </summary>
    /// <param name="velocity">Velocity to give explosion</param>
    private void Explode(Vector2 velocity) {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<Rigidbody2D>().velocity = velocity;
        Destroy(gameObject);
    }

    public enum ProjectileType { 
        standard,
        emp
    }
}
