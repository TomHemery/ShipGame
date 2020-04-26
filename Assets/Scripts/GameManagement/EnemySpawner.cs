using System;
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

    private const float ENEMY_SPACING = 5.0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        foreach (GameObject g in allEnemyPrefabs) EnemyPrefabs.Add(g.name, g);

        GameManager.OnPlayerDeath.AddListener(OnPlayerDeath);
        GameManager.OnAreaLoaded.AddListener(OnAreaLoaded);
    }

    public void OnPlayerDeath() {
        DestroyAllEnemyGameObjects();
    }

    public void OnAreaLoaded() {
        DestroyAllEnemyGameObjects();
    }

    public static void SpawnAt(Vector2 pos, string name) {
        GameObject enemyShip = Instantiate(EnemyPrefabs[name], pos, Quaternion.identity);
        activeEnemies.Add(enemyShip);
        enemyShip.GetComponent<HealthResource>().OnExploded += OnEnemyShipDestroyed;

        if (activeEnemies.Count > 0) {
            GameObject.FindGameObjectWithTag("MiningStation").GetComponent<MiningStationUIToggle>().SetBehaviourEnabled(false);
        }
    }

    public static void SpawnAt(Vector2 pos, string name, int num) {
        Vector2 offset = new Vector2();
        for (int i = 0; i < num; i++) {
            SpawnAt(pos + offset, name);
            offset.Set(offset.x + ENEMY_SPACING, offset.y);
        }
    }

    public static void OnEnemyShipDestroyed(object sender, EventArgs e) {
        activeEnemies.Remove(((HealthResource)sender).gameObject);
        if (activeEnemies.Count <= 0) {
            OnAllEnemiesDefeated();
        }
    }

    private static void DestroyAllEnemyGameObjects() {
        foreach (GameObject g in activeEnemies)
        {
            Destroy(g);
        }
        activeEnemies.Clear();
        GameObject.FindGameObjectWithTag("MiningStation").GetComponent<MiningStationUIToggle>().SetBehaviourEnabled(true);
    }

    private static void OnAllEnemiesDefeated() {
        AllEnemiesDestroyed.Invoke();
        GameObject.FindGameObjectWithTag("MiningStation").GetComponent<MiningStationUIToggle>().SetBehaviourEnabled(true);
        if (MusicPlayer.Instance.PlayerState == MusicPlayer.MusicState.High || MusicPlayer.Instance.PlayerState == MusicPlayer.MusicState.Transition)
        {
            MusicPlayer.Instance.FadeToNewState(1.0f, MusicPlayer.MusicState.Mid);
        }
    }
}