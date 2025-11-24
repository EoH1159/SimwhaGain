
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemData data;       // 어떤 아이템 설계도인지
    public int currentDurability;

    public bool IsBroken => data != null && data.isEquipment && currentDurability <= 0;
}
