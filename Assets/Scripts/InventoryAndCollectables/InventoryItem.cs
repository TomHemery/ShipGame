using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InventoryItem
{
    [SerializeField]
    public GameObject itemPrefab;
    [SerializeField]
    public int quantity;
}
