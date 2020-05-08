using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicEnemySpawner : MonoBehaviour
{

    public string[] enemyTypes;

    public int maxEnemiesPerSpawn;
    public int minEnemiesPerSpawn;

    public float minSpawnCooldown;
    public float maxSpawnCooldown;

    public bool waitForEnemiesDefeated;
    private bool countToSpawn = true;

    private float waitPeriod;
    private float counter = 0;

    private Transform playerShipTransform;

    public float spawnRadius = 100.0f;

    private void Awake()
    {
        waitPeriod = Random.Range(minSpawnCooldown, maxSpawnCooldown);
        counter = 0;
        playerShipTransform = GameObject.FindGameObjectWithTag("PlayerShip").transform;
        Debug.Log("Preiodic enemy spawner awake, enemies spawning in: " + waitPeriod);

        if(waitForEnemiesDefeated)EnemySpawner.AllEnemiesDestroyed.AddListener(OnAllEnemiesDestroyed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.SimPaused)
        {
            if (counter > waitPeriod)
            {
                countToSpawn = !waitForEnemiesDefeated;
                counter = 0.0f;
                waitPeriod = Random.Range(minSpawnCooldown, maxSpawnCooldown);
                string enemyType = enemyTypes[Random.Range(0, enemyTypes.Length)];
                EnemySpawner.SpawnAt(
                    playerShipTransform.position + Random.insideUnitSphere.normalized * spawnRadius,
                    enemyType,
                    Random.Range(minEnemiesPerSpawn, maxEnemiesPerSpawn));
            }
            else if (countToSpawn)
            {
                counter += Time.deltaTime;
            }
        } 
    }

    void OnAllEnemiesDestroyed()
    {
        countToSpawn = true;
    }
}
