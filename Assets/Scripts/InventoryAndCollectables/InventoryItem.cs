using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
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

    public InventoryItem(InventoryItem item)
    {
        inventorySprite = item.inventorySprite;
        quantity = item.quantity;
        systemName = item.systemName;
        prettyName = item.prettyName;
        equipable = item.equipable;
        equipType = item.equipType;
    }

    public override string ToString() {
        return "Inventory Item: " + systemName + ", " + prettyName + "; " + quantity + "\nEquipable? " + equipable + ", equip type: " + equipType;
    }
}

[System.Serializable]
public enum EquipType { 
    Weapon,
    SetItem,
    Blueprint,
    None
}