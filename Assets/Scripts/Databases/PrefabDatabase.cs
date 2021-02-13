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

    /// <summary>
    /// Gets a gameobject from the database if one exists
    /// </summary>
    /// <param name="name">The name of the prefab object to look for</param>
    /// <returns></returns>
    public static GameObject Get(string name)
    {
        if (prefabDictionary.ContainsKey(name)) {
            return prefabDictionary[name];
        }
        return null;
    }
}
