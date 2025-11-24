
using UnityEngine;

public class UnitStatus : MonoBehaviour
{
    public int maxHP;
    public int currentHP;

    public int attack;
    public int defense;

    public int moveRange; // 이동 칸 수
    public int luck;      // 명중/회피에 관여
    // 더 이상 여기서 무기를 들고 있지 않아도 됨 (equippedWeapon 없이)
    // 공격력은 "기본 공격력"만 들고 있고,
    // 실제 공격 시에 선택한 무기 정보(InventoryItem)를 추가로 넘겨서 계산

    public void Die()
    {
        Debug.Log($"{gameObject.name} 이(가) 사망했습니다.");
        // TODO: 애니메이션, 이펙트, 대사 등 나중에 여기서 처리
        Destroy(gameObject);
    }
}
