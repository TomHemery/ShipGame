using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryController : MonoBehaviour
{
    public static UIInventoryController Instance { get; private set; } = null;
    public GameObject contents;
    public GameObject itemFrame;
    public Inventory playerInventory;

    public Text CapacityText;
    public Color CapacityTextNotFilled;
    public Color CapacityTextFilled;

    private RectTransform contentsRect;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        contentsRect = contents.GetComponent<RectTransform>();
        Debug.Log("Contents rect width: " + contentsRect.rect.width);
    }

    private void Start()
    {
        UpdateContents();
    }

    public void UpdateContents() {
        int x = 0;
        int y = 0;

        foreach (Transform child in contents.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<string, int> entry in playerInventory.Contents) {
            GameObject screenItem = Instantiate(itemFrame, contents.transform);

            RectTransform itemRect = screenItem.GetComponent<RectTransform>();
            itemRect.localPosition = new Vector2(x * itemRect.rect.width, y * itemRect.rect.height);

            Debug.Log("Item rect width: " + itemRect.rect.width);

            GameObject itemSprite = screenItem.transform.Find("ItemSprite").gameObject;
            itemSprite.transform.Find("QuantityText").GetComponent<Text>().text = entry.Value.ToString();
            itemSprite.transform.Find("NameText").GetComponent<Text>().text = entry.Key;
            itemSprite.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Collectables/" + entry.Key);

            x++;
            if (x * itemRect.rect.width >= contentsRect.rect.width) {
                x = 0;
                y--;
            }

        }

        CapacityText.text = playerInventory.FilledCapacity.ToString() + "/" + playerInventory.MaxCapacity.ToString();
        CapacityText.color = playerInventory.FilledCapacity == playerInventory.MaxCapacity ? CapacityTextFilled : CapacityTextNotFilled;
    }
}
