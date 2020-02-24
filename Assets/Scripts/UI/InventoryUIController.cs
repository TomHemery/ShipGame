using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public GameObject contents;
    public GameObject itemFrame;

    public Inventory targetInventory;
    public bool attachToPlayerInventory;

    public Text capacityText;
    public Text nameText;
    public Color CapacityTextNotFilled;
    public Color CapacityTextFilled;

    private RectTransform contentsRect;

    private void Awake()
    {
        contentsRect = contents.GetComponent<RectTransform>();
        if(attachToPlayerInventory) targetInventory = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>();
        nameText.text = targetInventory.prettyName;
    }

    public void UpdateContents() {
        int x = 0;
        int y = 0;

        foreach (Transform child in contents.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<string, int> entry in targetInventory.Contents) {
            GameObject screenItem = Instantiate(itemFrame, contents.transform);

            RectTransform itemRect = screenItem.GetComponent<RectTransform>();
            itemRect.localPosition = new Vector2(x * itemRect.rect.width, y * itemRect.rect.height);

            GameObject itemSprite = screenItem.transform.Find("ItemSprite").gameObject;
            itemSprite.transform.Find("QuantityText").GetComponent<Text>().text = entry.Value.ToString();
            itemSprite.transform.Find("NameText").GetComponent<Text>().text = entry.Key;
            itemSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Collectables/" + entry.Key);

            ItemFrame itemFrameBehaviour = screenItem.GetComponent<ItemFrame>();
            itemFrameBehaviour.parentInventoryController = this;
            itemFrameBehaviour.itemName = entry.Key;
            itemFrameBehaviour.itemQuantity = entry.Value;

            x++;
            if (x * itemRect.rect.width >= contentsRect.rect.width) {
                x = 0;
                y--;
            }

        }

        capacityText.text = targetInventory.FilledCapacity.ToString() + "/" + targetInventory.MaxCapacity.ToString();
        capacityText.color = targetInventory.FilledCapacity == targetInventory.MaxCapacity ? CapacityTextFilled : CapacityTextNotFilled;
    }

    private void OnEnable()
    {
        targetInventory.uiControllers.Add(this);
        UpdateContents();
    }

    private void OnDisable()
    {
        targetInventory.uiControllers.Remove(this);
    }
}