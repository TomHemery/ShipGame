using UnityEngine;

[System.Serializable]
public class AutoMoveTarget
{
    [SerializeField]
    public Slot associatedSlot;
    [SerializeField]
    public Inventory associatedInventory;
    [SerializeField]
    public AutomoveType type;

    public AutoMoveTarget()
    {
        if (type == AutomoveType.TargetSlot) associatedInventory = null;
        else if (type == AutomoveType.TargetInventory) associatedSlot = null;
    }

    public enum AutomoveType{ 
        TargetSlot,
        TargetInventory,
        None
    }
}
