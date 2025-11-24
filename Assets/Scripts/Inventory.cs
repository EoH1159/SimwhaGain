using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int capacity = 5;              // 최대 슬롯 수 (지금은 5칸)
    public List<ItemData> items = new();  // 실제로 아이템을 들고 있는 리스트

    public bool AddItem(ItemData item)
    {
        // TODO: 여기서 인벤토리가 꽉 찼는지 확인하고,
        // 여유가 있으면 items에 추가한 뒤 true/false 리턴하기
        if (items.Count >= capacity)
        {
            return false; // 인벤토리가 꽉 찼음
        }
        items.Add(item);
        return true; // 아이템 추가 성공
    }

    public void RemoveItem(ItemData item)
    {
        // TODO: items 리스트에서 해당 아이템 제거하기
        items.Remove(item);
    }
}
