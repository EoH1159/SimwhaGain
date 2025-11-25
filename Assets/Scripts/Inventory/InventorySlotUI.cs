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
}
