using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabDatabase : MonoBehaviour
{
    public static PrefabDatabase Instance { get; private set; } = null;
    protected static Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    [SerializeField]
    private GameObject [] allPrefabs;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        foreach (GameObject g in allPrefabs) {
            prefabDictionary.Add(g.name, g);
        }
    }

    public GameObject this [string name]
    {
        get
        {
            if (prefabDictionary.ContainsKey(name)) {
                GameObject requested = prefabDictionary[name];
                return Instantiate(requested);
            }
            return null;
        }
    }
}
