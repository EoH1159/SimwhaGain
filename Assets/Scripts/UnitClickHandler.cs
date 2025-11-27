
using UnityEngine;

public class UnitClickHandler : MonoBehaviour
{
    public UnitStatus unitStatus;
    public CommandUI commandUI;

    private void OnMouseDown()
    {
        Debug.Log("Player Unit clicked!");

        var bm = BattleManager.Instance;
        if (bm == null) return;

        Debug.Log(
      $"Unit clicked: {unitStatus.gameObject.name}, " +
      $"faction={unitStatus.faction}, " +
      $"phase={bm.currentPhase}, " +
      $"isAttackMode={bm.isAttackMode}, " +
      $"selectedUnit={(bm.selectedUnit != null ? bm.selectedUnit.gameObject.name : "null")}"
  );

        // 1) 플레이어 유닛을 클릭한 경우 → 커맨드 열기
        if (unitStatus.faction == UnitStatus.Faction.Player)
        {
            commandUI.Show(unitStatus);
            return;
        }

        // 2) 적 유닛을 클릭한 경우
        if (unitStatus.faction == UnitStatus.Faction.Enemy)
        {
            // 공격 모드 + 플레이어 턴 + 공격할 유닛이 선택되어 있을 때만
            if (bm.currentPhase == BattleManager.TurnPhase.Player &&
                bm.isAttackMode &&
                bm.selectedUnit != null)
            {
                Debug.Log("적 유닛 클릭: 공격 시도");

                // 다음 단계에서 만들 Attack 로직 자리
                bm.Attack(bm.selectedUnit, unitStatus);
            }
        }
    }
}
