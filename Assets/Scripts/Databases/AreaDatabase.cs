using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDatabase : MonoBehaviour
{
    public static AreaDatabase Instance { get; private set; } = null;
    public static Dictionary<string, Area> AreaDictionary { get; private set; } = new Dictionary<string, Area>();

    [SerializeField]
    private Area[] allAreas;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        foreach (Area a in allAreas)
        {
            AreaDictionary.Add(a.systemName, a);
        }
    }
}
