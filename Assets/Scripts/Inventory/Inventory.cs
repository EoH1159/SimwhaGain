using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int capacity = 5;              // 최대 슬롯 수 (지금은 5칸)
    public List<InventoryItem> items = new();  // InventoryItem 리스트로 변경
    public bool AddItem(ItemData itemData)
    {
        // TODO: 여기서 인벤토리가 꽉 찼는지 확인하고,
        // 여유가 있으면 items에 추가한 뒤 true/false 리턴하기
        if (items.Count >= capacity)
        {
            return false; // 인벤토리가 꽉 찼음
        }
        InventoryItem newItem = new InventoryItem();
        newItem.data = itemData;
        newItem.currentDurability = itemData.maxDurability; // 설계도에서 최대 내구도 가져오기

        items.Add(newItem);
        return true; // 아이템 추가 성공
    }

    public void RemoveItem(InventoryItem item)
    {
        // TODO: items 리스트에서 해당 아이템 제거하기
        items.Remove(item);
    }
}
