using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTextTracker : MonoBehaviour
{

    public Inventory associatedInventory;
    public bool attachToPlayer;
    public Text textTarget;
    public string messagePrefix;

    public Color defaultColour;
    public Color fullColour;

    private void Awake()
    {
        if (attachToPlayer) associatedInventory = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>();
        associatedInventory.InventoryChangedEvent += OnAssociatedInventoryChanged;

        if (textTarget == null) {
            textTarget = GetComponent<Text>();
            if (textTarget == null) Debug.LogError("No text target set for text tracker", this);
        }
    }

    private void OnDestroy()
    {
        associatedInventory.InventoryChangedEvent -= OnAssociatedInventoryChanged;
        associatedInventory = null;
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void OnAssociatedInventoryChanged(object sender, EventArgs e)
    {
        Refresh();
    }

    public void Refresh()
    {
        textTarget.text = messagePrefix + associatedInventory.FilledCapacity.ToString() + "/" + associatedInventory.MaxCapacity.ToString();
        textTarget.color = associatedInventory.FilledCapacity >= associatedInventory.MaxCapacity ? fullColour : defaultColour;
    }
}
