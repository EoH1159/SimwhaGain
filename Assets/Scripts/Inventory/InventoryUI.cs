using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;              // 실제 인벤토리 데이터
    public InventorySlotUI slotPrefab;       // 슬롯 프리팹
    public Transform slotsParent;            // 슬롯들이 붙을 부모

    private List<InventorySlotUI> slots = new();

    private void Start()
    {
        // TODO: inventory.capacity 만큼 슬롯 생성해서 slots 리스트에 넣기
        // 그리고 처음 한 번 Refresh() 호출
        for (int i = 0; i < inventory.capacity; i++)
        {
            InventorySlotUI newSlot = Instantiate(slotPrefab, slotsParent);
            slots.Add(newSlot);
        }
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // TODO: inventory.items 내용을 읽어서
        // slots[i].SetItem(해당 InventoryItem 또는 null) 호출
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].SetItem(inventory.items[i]);
            }
            else
            {
                slots[i].SetItem(null);
            }
        }
    }
}
