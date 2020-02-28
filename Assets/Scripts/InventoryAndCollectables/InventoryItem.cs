using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InventoryItem
{
    [SerializeField]
    public GameObject itemPrefab;
    [SerializeField]
    public Sprite inventorySprite;
    [SerializeField]
    public int quantity;
    [SerializeField]
    public string systemName;
    [SerializeField]
    public string prettyName;
    [SerializeField]
    public bool equipable;
    [SerializeField]
    public EquipType equipType;
}

public enum EquipType { 
    Weapon,
    None
}