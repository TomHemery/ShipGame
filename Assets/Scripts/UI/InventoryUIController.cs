using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : ItemFrameLayoutController
{
    public Inventory m_attachedInventory;
    public bool attachToPlayerInventory;

    public Text capacityText;
    public Text nameText;
    public Color CapacityTextNotFilled;
    public Color CapacityTextFilled;
    
    protected virtual void Awake()
    {
        if(attachToPlayerInventory) m_attachedInventory = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>();
        nameText.text = m_attachedInventory.prettyName;
    }

    public override void UpdateContents() {
        int x = 0;
        int y = 0;

        foreach (Transform child in contents.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<string, InventoryItem> entry in m_attachedInventory.Contents) {
            GameObject screenItem = Instantiate(itemFramePrefab, contents.transform);

            RectTransform itemRect = screenItem.GetComponent<RectTransform>();
            itemRect.localPosition = new Vector2(x * itemRect.rect.width, y * itemRect.rect.height);

            GameObject itemSprite = screenItem.transform.Find("ItemSprite").gameObject;
            itemSprite.transform.Find("QuantityText").GetComponent<Text>().text = entry.Value.quantity.ToString();
            itemSprite.transform.Find("NameText").GetComponent<Text>().text = entry.Value.prettyName;
            itemSprite.GetComponent<Image>().sprite = entry.Value.inventorySprite;

            ItemFrame frame = screenItem.GetComponent<ItemFrame>();
            frame.m_inventoryItem = entry.Value;
            frame.parentController = this;

            x++;
            if (x * itemRect.rect.width >= contents.rect.width) {
                x = 0;
                y--;
            }
        }

        capacityText.text = m_attachedInventory.FilledCapacity.ToString() + "/" + m_attachedInventory.MaxCapacity.ToString();
        capacityText.color = m_attachedInventory.FilledCapacity == m_attachedInventory.MaxCapacity ? CapacityTextFilled : CapacityTextNotFilled;
    }

    public override bool TryAddItemFrameFor(InventoryItem item)
    {
        return m_attachedInventory.TryAddItem(item);
    }

    public override bool TryRemoveItemFrameFor(InventoryItem item)
    {
        return m_attachedInventory.TryRemoveItem(item);
    }

    protected virtual void OnEnable()
    {
        m_attachedInventory.uiControllers.Add(this);
        UpdateContents();
    }

    protected virtual void OnDisable()
    {
        m_attachedInventory.uiControllers.Remove(this);
    }
}