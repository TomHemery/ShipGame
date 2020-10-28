using System.Collections;
using System.Collections.Generic;

public class InventoryCollection : IEnumerable<InventoryItem>
{
    protected List<InventoryItem> contents = new List<InventoryItem>();

    public int Count { get { return contents.Count; } }

    public InventoryItem this[int i]
    {
        get
        {
            if (i < Count) return contents[i];
            return null;
        }
        set
        {
            if (i >= Count)
            {
                for (int j = Count; j < i; j++) contents.Add(null);
                contents.Add(value);
            }
            else
            {
                contents[i] = value;
            }
        }
    }

    public InventoryItem this[string name]
    {
        get
        {
            foreach(InventoryItem item in contents)
            {
                if (item?.systemName == name)
                    return item;
            }
            return null;
        }
        set
        {
            for(int i = 0; i < contents.Count; i++)
            {
                if(contents[i]?.systemName == name)
                {
                    contents[i] = value;
                    return;
                }
            }
            AddToFirstEmptySlot(value);
        }
    }

    public InventoryItem this[InventoryItem item]
    {
        get { return this[item.systemName]; }
        set { this[item.systemName] = value; }
    }

    public bool Contains(InventoryItem item)
    {
        return contents.Contains(item);
    }

    public bool Contains(string itemName)
    {
        foreach(InventoryItem item in contents)
        {
            if (item?.systemName == itemName) return true;
        }
        return false;
    }

    public void AddToFirstEmptySlot(InventoryItem item)
    {
        for(int i = 0; i < contents.Count; i++)
        {
            if(contents[i] == null)
            {
                contents[i] = item;
                return;
            }
        }
        AddToEnd(item);
    }

    public void AddToEnd(InventoryItem item)
    {
        contents.Add(item);
    }

    public void RemoveAt(int index)
    {
        contents[index] = null;
    }

    public void Clear()
    {
        contents.Clear();
    }

    public IEnumerator<InventoryItem> GetEnumerator()
    {
        return contents.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
