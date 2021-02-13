using System.Collections.Generic;
using UnityEngine;

public class InventoryItemDatabase : MonoBehaviour
{
    public static InventoryItemDatabase Instance { get; private set; }
    protected static Dictionary<string, InventoryItem> contents;

    [SerializeField]
    protected InventoryItem[] allItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            contents = new Dictionary<string, InventoryItem>();

            foreach (InventoryItem item in allItems)
            {
                contents.Add(item.systemName, item);
            }
        }
    }

    public static InventoryItem Get(string name)
    {
        if (contents.ContainsKey(name))
        {
            return contents[name];
        }
        return null;
    }
}
