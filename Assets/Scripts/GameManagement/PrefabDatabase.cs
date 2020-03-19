using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabDatabase : MonoBehaviour
{
    public static PrefabDatabase Instance { get; private set; } = null;
    public static Dictionary<string, GameObject> PrefabDictionary { get; private set; } = new Dictionary<string, GameObject>();

    [SerializeField]
    private GameObject [] allPrefabs;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        foreach (GameObject g in allPrefabs) {
            PrefabDictionary.Add(g.name, g);
        }
    }
}
