using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemFrameLayoutController : MonoBehaviour
{
    public RectTransform contents;
    public GameObject itemFramePrefab;

    public abstract bool TryAddItem(InventoryItem item);

    public abstract bool TryRemoveItem(InventoryItem item);

    public abstract void UpdateContents();
}
