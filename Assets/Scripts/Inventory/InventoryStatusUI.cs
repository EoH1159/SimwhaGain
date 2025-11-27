
using TMPro;
using UnityEngine;

public class InventoryStatusUI : MonoBehaviour
{
    public UnitStatus unitStatus;      // 플레이어의 스탯
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text expText;
    public TMP_Text hpText;
    public TMP_Text attackText;
    public TMP_Text defenseText;
    public TMP_Text moveText;
    public TMP_Text luckText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (unitStatus == null) return;

        // 이름은 나중에 다른 스크립트에서 가져오고 싶으면 따로 빼도 됨
        if (nameText != null)
            nameText.text = unitStatus.gameObject.name;

        if (levelText != null)
            levelText.text = $"레벨: {unitStatus.level}";

        if (expText != null)
            expText.text = $"EXP: {unitStatus.currentExp} / {unitStatus.expToNext}";

        if (hpText != null)
            hpText.text = $"HP: {unitStatus.currentHP} / {unitStatus.maxHP}";

        if (attackText != null)
            attackText.text = $"공격: {unitStatus.attack}";

        if (defenseText != null)
            defenseText.text = $"방어: {unitStatus.defense}";

        if (moveText != null)
            moveText.text = $"이동: {unitStatus.moveRange}";

        if (luckText != null)
            luckText.text = $"행운: {unitStatus.luck}";
    }
}
