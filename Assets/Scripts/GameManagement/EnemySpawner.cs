using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; } = null;

    [SerializeField]
    private GameObject[] allEnemyPrefabs; //assigned in inspector

    private static Dictionary<string, GameObject> EnemyPrefabs = new Dictionary<string, GameObject>();

    private static List<GameObject> activeEnemies = new List<GameObject>();

    public static UnityEvent AllEnemiesDestroyed { get; private set; } = new UnityEvent();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        foreach (GameObject g in allEnemyPrefabs) EnemyPrefabs.Add(g.name, g);
    }

    public static void SpawnAt(Vector2 pos, string name, int num = 1) {
        for (int i = 0; i < num; i++)
        {
            GameObject enemyShip = Instantiate(EnemyPrefabs[name], pos, Quaternion.identity);
            activeEnemies.Add(enemyShip);
            enemyShip.GetComponent<HealthResource>().OnExploded += OnEnemyShipDestroyed;
        }
    }

    public static void SpawnOnRadiusAt(Vector2 pos, float radius, string name, int num = 1) {
        for (int i = 0; i < num; i++) SpawnAt(pos + UnityEngine.Random.insideUnitCircle.normalized * radius, name);
    }

    public static void OnEnemyShipDestroyed(object sender, EventArgs e) {
        activeEnemies.Remove(((HealthResource)sender).gameObject);
        if (activeEnemies.Count <= 0) {
            AllEnemiesDestroyed.Invoke();
            if (MusicPlayer.Instance.PlayerState == MusicPlayer.MusicState.High) {
                MusicPlayer.Instance.FadeToNewState(1.0f, MusicPlayer.MusicState.Mid);
            }
        }
    }
}