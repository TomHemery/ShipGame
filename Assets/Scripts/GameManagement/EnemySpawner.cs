using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; } = null;

    [SerializeField]
    private GameObject[] allEnemyPrefabs; //assigned in inspector

    public static Dictionary<string, GameObject> EnemyPrefabs { get; private set; } = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        foreach (GameObject g in allEnemyPrefabs) EnemyPrefabs.Add(g.name, g);
    }
}