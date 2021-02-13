using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintDatabase : MonoBehaviour
{
    public static BlueprintDatabase Instance { get; private set; } = null;
    public static Dictionary<string, Blueprint> BlueprintDictionary { get; private set; } = new Dictionary<string, Blueprint>();

    [SerializeField]
    private Blueprint[] allBlueprints;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;   
        }
        foreach (Blueprint b in allBlueprints)
        {
            BlueprintDictionary.Add(b.systemName, b);
        }
    }
}
