﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; } = null;
    public GameObject CentralPanel;

    public Slot blueprintSlot;
    public Text materialsText;
    public Dropdown blueprintDropdown;
    public Slot outputSlot;
    public Inventory associatedInventory;
    public Text craftButtonLabel;
    public Text craftMaxButtonLabel;

    public List<Blueprint> UnlockedBlueprints { get; private set; } = new List<Blueprint>();
    private bool canCraft = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnEnable()
    {
        blueprintSlot.SlotContentsChanged += OnBlueprintSlotContentsChange;
        outputSlot.SlotContentsChanged += OnOutputSlotContentsChange;
        associatedInventory.InventoryChangedEvent += OnAssociatedInventoryChanged;
    }

    private void OnDisable()
    {
        blueprintSlot.SlotContentsChanged -= OnBlueprintSlotContentsChange;
        outputSlot.SlotContentsChanged -= OnOutputSlotContentsChange;
        associatedInventory.InventoryChangedEvent -= OnAssociatedInventoryChanged;
    }

    private void Start()
    {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        CentralPanel.SetActive(false);
        CheckMaterials();
    }

    public void Hide() {
        gameObject.SetActive(false);
        CentralPanel.SetActive(true);
    }

    public void Toggle() {
        if (gameObject.activeInHierarchy) Hide();
        else Show();
    }

    public void OnBlueprintDropdownSelection()
    {
        CheckMaterials();
    }

    private void OnAssociatedInventoryChanged(object sender, EventArgs e)
    {
        CheckMaterials();
    }

    public void CheckMaterials() {
        if (blueprintDropdown.value >= 0)
        {
            canCraft = true;

            Blueprint bp = UnlockedBlueprints[blueprintDropdown.value];

            string requirementsDesc = "<b>Requirements</b>\n";

            int totalCraftable = int.MaxValue;
            for (int i = 0; i < bp.materials.Length; i++)
            {
                int count = associatedInventory.CountOf(bp.materials[i]);
                if (count >= bp.quantities[i])
                {
                    requirementsDesc += "<color=green>";
                    int num = count / bp.quantities[i];
                    totalCraftable = totalCraftable > num ? num : totalCraftable;
                }
                else
                {
                    totalCraftable = 0;
                    requirementsDesc += "<color=red>";
                    canCraft = false;
                }
                requirementsDesc += "<b>" + bp.materials[i] + ":</b> " + bp.quantities[i] + "</color>\n";

            }

            if (canCraft) {
                canCraft = outputSlot.StoredItemFrame == null || outputSlot.StoredItemFrame.m_InventoryItem.systemName == bp.output;
            }

            materialsText.text = requirementsDesc;

            craftButtonLabel.color = canCraft ? Color.green : Color.red;
            craftMaxButtonLabel.color = canCraft ? Color.green : Color.red;
            craftMaxButtonLabel.text = "Craft Max [" + totalCraftable + "]";
        }
    }

    public void Craft() {
        if (canCraft) {
            Blueprint bp = UnlockedBlueprints[blueprintDropdown.value];
            for (int i = 0; i < bp.materials.Length; i++)
            {
                associatedInventory.TryRemove(bp.materials[i], bp.quantities[i]);
            }
            if (outputSlot.StoredItemFrame == null)
            {
                GameObject output = PrefabDatabase.Instance[bp.output];
                if (output.GetComponent<Weapon>() != null)
                {
                    outputSlot.TryCreateFrameFor(output.GetComponent<Weapon>().m_inventoryItem);
                }
                else if (output.GetComponent<PickUpOnContact>() != null)
                {
                    outputSlot.TryCreateFrameFor(output.GetComponent<PickUpOnContact>().m_inventoryItem);
                }
            }
            else
            {
                outputSlot.StoredItemFrame.SetQuantity(outputSlot.StoredItemFrame.m_InventoryItem.quantity + 1);
            }
        }

        CheckMaterials();
    }

    public void CraftMax() {
        StartCoroutine(CraftMaxCoroutine());
    }

    private IEnumerator CraftMaxCoroutine() {
        float autocraftDelay = 0.1f;
        while (canCraft) {
            Craft();
            yield return new WaitForSeconds(autocraftDelay);
        }
    }

    private void OnBlueprintSlotContentsChange() {
        if (blueprintSlot.StoredItemFrame != null) {
            GameObject blueprintObject = blueprintSlot.StoredItemFrame.gameObject;

            Blueprint bp = 
                BlueprintDatabase.BlueprintDictionary[blueprintObject.GetComponent<ItemFrame>().m_InventoryItem.systemName];

            blueprintSlot.SilentRemoveItemFrame();
            Destroy(blueprintObject);

            if (!HasUnlockedBlueprint(bp))
            {
                UnlockedBlueprints.Add(bp);

                List<OptionData> newOptions = new List<OptionData>
                {
                    new OptionData(bp.output)
                };

                blueprintDropdown.AddOptions(newOptions);
            }
        }
        blueprintDropdown.value = blueprintDropdown.options.Count - 1;
        OnBlueprintDropdownSelection();
    }

    private void OnOutputSlotContentsChange() {
        CheckMaterials();
    }

    public void SetUnlockedBlueprints(List<Blueprint> bps) {
        UnlockedBlueprints = bps;

        blueprintDropdown.ClearOptions();

        List<OptionData> newOptions = new List<OptionData>();
        foreach (Blueprint bp in bps) {
            newOptions.Add(new OptionData(bp.output));
        }

        blueprintDropdown.AddOptions(newOptions);
        blueprintDropdown.value = blueprintDropdown.options.Count - 1;
        OnBlueprintDropdownSelection();
    }

    private bool HasUnlockedBlueprint(Blueprint bp) {
        foreach(Blueprint curr in UnlockedBlueprints) {
            if (bp.systemName == curr.systemName) return true;
        }
        return false;
    }
}
