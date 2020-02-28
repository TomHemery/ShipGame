using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : ItemFrameLayoutController
{
    public Text slotText;

    private void Awake()
    {
        contents = GetComponent<RectTransform>();
    }

    public override bool TryAddItemFrameFor(InventoryItem item)
    {
        if (transform.childCount == 1) {
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
        return false;
    }

    public override bool TryRemoveItemFrameFor(InventoryItem item)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateContents()
    {
        return;
    }
}
