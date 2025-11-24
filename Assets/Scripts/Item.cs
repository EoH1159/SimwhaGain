using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemCategory
    {
        Weapon,
        Armor,
        Consumable, // 포션 같은 소모품
        Quest,
        Etc
    }

    public enum ItemEffectType
    {
        None,
        HealHP,
        Attack,
        Defense,
        Speed
    }

    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public ItemCategory category;
        public ItemEffectType effectType;

        public int power;        // 효과의 세기 (예: 50 회복)
        public string description;
    }
}
