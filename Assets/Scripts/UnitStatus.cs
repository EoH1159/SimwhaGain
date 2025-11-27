
using UnityEngine;

public class UnitStatus : MonoBehaviour
{
    public enum Faction
    {
        Player,
        Enemy
    }

    public Faction faction = Faction.Player;  // 기본은 플레이어

    public int maxHP;
    public int currentHP;

    public int attack;
    public int defense;

    public int moveRange; // 이동 칸 수
    public int luck;      // 명중/회피에 관여
                          // 더 이상 여기서 무기를 들고 있지 않아도 됨 (equippedWeapon 없이)
                          // 공격력은 "기본 공격력"만 들고 있고,
                          // 실제 공격 시에 선택한 무기 정보(InventoryItem)를 추가로 넘겨서 계산
    public Tile currentTile;

    public int level = 1;
    public int currentExp = 0;
    public int expToNext = 100;  // 다음 레벨까지 필요한 경험치

    SpriteRenderer sr;
    Color normalColor;
    public Color selectedColor = Color.yellow;  // 선택 시 색 (인스펙터에서 바꿔도 됨)
    public Color hitColor = Color.red;   // 피격 시 번쩍일 색 (Inspector에서 조절 가능)

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            normalColor = sr.color;
        }
    }

    public void SetSelected(bool selected)
    {
        if (sr == null) return;
        sr.color = selected ? selectedColor : normalColor;
    }

    public System.Collections.IEnumerator HitFlash()
    {
        if (sr == null) yield break;

        // 원래 색 저장 (선택 색이 켜져 있을 수도 있으니까, 현재색 기준)
        Color before = sr.color;

        // 피격 색으로 변경
        sr.color = hitColor;

        // 잠깐 대기
        yield return new WaitForSeconds(0.1f);
        if (sr != null)
        {
            sr.color = before;
        }

        // 다시 원래 색으로 되돌리기
        sr.color = before;
    }

    public void GainExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNext)
        {
            currentExp -= expToNext;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        maxHP += 2; 
        attack += 1; 
        defense += 1;
        moveRange += 0; // 이동력은 레벨업 시 증가하지 않음
        luck += 1;
        expToNext = Mathf.RoundToInt(expToNext * 1.2f); // 다음 레벨업 필요 경험치 증가
        currentHP = maxHP;
        

        Debug.Log($"{gameObject.name} 레벨 업! 현재 레벨: {level}");
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} 이(가) 사망했습니다.");
        // TODO: 애니메이션, 이펙트, 대사 등 나중에 여기서 처리
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
