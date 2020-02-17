using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LootDrop
{
    [SerializeField]
    public GameObject dropPrefab;
    [SerializeField]
    public float dropProbability;
    [SerializeField]
    public int dropQuantityMax;
    [SerializeField]
    public int dropQuantityMin;
}
