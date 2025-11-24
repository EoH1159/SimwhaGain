
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public void Attack(UnitStatus attacker, UnitStatus defender, InventoryItem weapon, Inventory attackerInventory)
    {
        // 1. 무기가 부서졌는지 체크
        if (weapon == null)
        {
            Debug.Log("무기가 없습니다.");
            return;
        }
        if (weapon.IsBroken)
        {
            Debug.Log("무기를 쓸 수 없습니다.");
            return;
        }
        // 2. 공격력 = attacker.attack + weapon.data.bonusAttack
        int totalAttack = attacker.attack + weapon.data.bonusAttack;

        // 3. 실제 데미지 = 공격력 - defender.defense (0 밑으로는 내려가지 않게)
        int damage = Mathf.Max(totalAttack - defender.defense, 0);

        // 4. defender.currentHP 깎기
        defender.currentHP -= Mathf.Max(defender.currentHP - damage, 0);
        if (defender.currentHP <= 0)
        {
            defender.Die();
        }

        // 5. weapon.currentDurability-- 하고, 0 되면 attackerInventory.RemoveItem(weapon)
        weapon.currentDurability--;
        if (weapon.currentDurability <= 0)
        {
            attackerInventory.RemoveItem(weapon);
            Debug.Log("내구도가 다하였습니다.");
        }
    }
}
