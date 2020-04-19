using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Blueprint {
    [SerializeField]
    public string systemName;
    [SerializeField]
    public string prettyName;
    [SerializeField]
    public string[] materials;
    [SerializeField]
    public int[] quantities;
    [SerializeField]
    public string output;
} 