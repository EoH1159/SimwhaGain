using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text durabilityText;

    private InventoryItem currentItem;

    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        if (item == null || item.data == null)
        {
            itemNameText.text = " ";
            durabilityText.text = " ";
            return;
        }

        itemNameText.text = item.data.itemName;

        if (item.data.isEquipment)
        {
            durabilityText.text = $"내구도: {item.currentDurability}/{item.data.maxDurability}";
        }
        else
        {
            durabilityText.text = " ";
        }
    }

    public void OnClickSlot()
    {
        // 무기를 선택하는 모드일 때만 동작하게
        if (BattleManager.Instance != null && currentItem != null)
        {
            BattleManager.Instance.OnWeaponSelected(currentItem);
        }
    }
}
