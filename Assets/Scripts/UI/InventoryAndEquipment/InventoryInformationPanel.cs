﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInformationPanel : MonoBehaviour
{
    public Inventory associatedInventory;
    public bool attachToPlayer;
    public Text nameText;
    public Text capacityText;

    private void Awake()
    {
        if (attachToPlayer) associatedInventory = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh() {
        nameText.text = associatedInventory.prettyName;
        capacityText.text = associatedInventory.FilledCapacity.ToString() + "/" + associatedInventory.MaxCapacity.ToString();
    }
}
