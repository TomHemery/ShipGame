﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryController : MonoBehaviour
{
    public static UIInventoryController Instance { get; private set; } = null;
    public GameObject contents;
    public GameObject itemFrame;
    public Inventory playerInventory;

    private RectTransform contentsRect;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        contentsRect = contents.GetComponent<RectTransform>();
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

            screenItem.GetComponentInChildren<Text>().text = "" + entry.Key + ": " + entry.Value;

            x++;
            if (x * itemRect.rect.width > contentsRect.rect.width) {
                x = 0;
                y++;
            }
        }
    }
}
