using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class CraftingSystem : MonoBehaviour
{
    public GameObject CentralPanel;

    public Slot blueprintSlot;
    public Text materialsText;
    public Dropdown blueprintDropdown;
    public Slot outputSlot;
    public Inventory associatedInventory;
    public Text craftButtonLabel;

    private List<Blueprint> unlockedBlueprints = new List<Blueprint>();
    private bool canCraft = false;

    private void OnEnable()
    {
        blueprintSlot.SlotContentsChanged += OnBlueprintSlotContentsChange;
        associatedInventory.InventoryChangedEvent += OnAssociatedInventoryChanged;
    }

    private void OnDisable()
    {
        blueprintSlot.SlotContentsChanged -= OnBlueprintSlotContentsChange;
        associatedInventory.InventoryChangedEvent -= OnAssociatedInventoryChanged;
    }

    private void Start()
    {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
        CentralPanel.SetActive(false);
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
        canCraft = true;

        Blueprint bp = unlockedBlueprints[blueprintDropdown.value];

        string requirementsDesc = "<b>Requirements</b>\n";

        for (int i = 0; i < bp.materials.Length; i++)
        {
            if (associatedInventory.Contents.ContainsKey(bp.materials[i]) && associatedInventory.Contents[bp.materials[i]].quantity >= bp.quantities[i])
            {
                requirementsDesc += "<color=green>";
            }
            else
            {
                requirementsDesc += "<color=red>";
                canCraft = false;
            }
            requirementsDesc += "<b>" + bp.materials[i] + ":</b> " + bp.quantities[i] + "</color>\n";
        }

        materialsText.text = requirementsDesc;

        craftButtonLabel.color = canCraft ? Color.green : Color.red;
    }

    public void Craft() {
        if (canCraft) {
            Blueprint bp = unlockedBlueprints[blueprintDropdown.value];
            for (int i = 0; i < bp.materials.Length; i++) {
                associatedInventory.TryRemoveItem(bp.materials[i], bp.quantities[i]);
            }

            GameObject output = PrefabDatabase.PrefabDictionary[bp.output];
            if (output.GetComponent<Weapon>() != null) {
                outputSlot.TryCreateFrameFor(output.GetComponent<Weapon>().m_inventoryItem);
            }
            else if (output.GetComponent<PickUpOnContact>() != null) {
                outputSlot.TryCreateFrameFor(output.GetComponent<PickUpOnContact>().m_inventoryItem);
            }
        }
    }

    private void OnBlueprintSlotContentsChange() {
        if (blueprintSlot.StoredItemFrame != null) {
            GameObject blueprintObject = blueprintSlot.StoredItemFrame.gameObject;

            Blueprint bp = 
                BlueprintDatabase.BlueprintDictionary[blueprintObject.GetComponent<ItemFrame>().m_InventoryItem.systemName];

            blueprintSlot.SilentRemoveItemFrame();
            Destroy(blueprintObject);

            unlockedBlueprints.Add(bp);

            List<OptionData> newOptions = new List<OptionData>
            {
                new OptionData(bp.output)
            };

            blueprintDropdown.AddOptions(newOptions);
        }
        blueprintDropdown.value = blueprintDropdown.options.Count - 1;
        OnBlueprintDropdownSelection();
    }
}
