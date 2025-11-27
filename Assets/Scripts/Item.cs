using UnityEngine;


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

    // === 장비용 추가 정보 ===
    public bool isEquipment;   // 장비인지 여부 (무기/방어구 등)
    public int bonusAttack;    // 공격력 보너스
    public int bonusDefense;   // 방어력 보너스
    public int maxDurability;  // 최대 내구도
    public int minAttackRange = 1;   // 기본은 1
    public int attackRange = 1;      // 이건 최대 사거리 역할로 사용
}

