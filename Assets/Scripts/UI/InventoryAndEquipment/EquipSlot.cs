using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : ItemFrameLayoutController
{
    public Transform physicalAttachmentPoint;
    public Text slotText;

    private void Awake()
    {
        contents = GetComponent<RectTransform>();
    }

    public override bool TryAddItem(InventoryItem item)
    {
        if (item.equipable && item.equipType == EquipType.Weapon && transform.childCount == 1) {
            SpawnItemFrame(item);
            Instantiate(PrefabDatabase.PrefabDictionary[item.systemName], physicalAttachmentPoint);
            return true;
        }
        return false;
    }

    public void SpawnItemFrame(InventoryItem item) {
        GameObject screenItem = Instantiate(itemFramePrefab, contents);
        screenItem.transform.SetAsFirstSibling();

        GameObject itemSprite = screenItem.transform.Find("ItemSprite").gameObject;
        itemSprite.transform.Find("QuantityText").GetComponent<Text>().text = "";
        itemSprite.transform.Find("NameText").GetComponent<Text>().text = item.prettyName;
        itemSprite.GetComponent<Image>().sprite = item.inventorySprite;

        ItemFrame frame = screenItem.GetComponent<ItemFrame>();
        frame.m_inventoryItem = item;
        frame.parentController = this;
    }

    public override bool TryRemoveItem(InventoryItem item)
    {
        if (physicalAttachmentPoint.childCount > 0) {
            foreach (Transform child in physicalAttachmentPoint) {
                child.SetParent(null);
                Destroy(child.gameObject);
            }
            return true;
        }
        return false;
    }

    public override void UpdateContents() {
        return;
    }
}
