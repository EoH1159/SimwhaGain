
using UnityEngine;

public class CommandUI : MonoBehaviour
{
    public GameObject panel;          // CommandPanel
    public BattleManager battleManager;

    private UnitStatus currentUnit;   //  마지막으로 열린 유닛
    public bool IsOpen => panel != null && panel.activeSelf;  // 밖에서 열려 있는지 체크용
    public void Show(UnitStatus unit)
    {
        currentUnit = unit;                        //  기억해두기
        battleManager.SetSelectedUnit(unit);
        panel.SetActive(true);
    }

    public void ShowCurrent()
    {
        if (currentUnit != null)
        {
            Show(currentUnit);   // ★ 마지막 유닛 기준으로 다시 열기
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void OnClickMove()
    {
        Debug.Log("Move 버튼 눌림");

        if (!battleManager.canMoveThisTurn)
        {
            Debug.Log("이번 턴에는 이미 이동했습니다.");
            panel.SetActive(false);
            return;
        }

        battleManager.isMoveMode = true;  // 이동 모드 ON
        if (battleManager.gridManager != null && battleManager.selectedUnit != null)
        {
            battleManager.gridManager.HighlightMoveRange(battleManager.selectedUnit);
        }
        panel.SetActive(false);
    }

    public void OnClickAttack()
    {
        Debug.Log("Attack 버튼 눌림 (무기 선택 모드)");

        if (battleManager.hasAttackedThisTurn)
        {
            Debug.Log("이번 턴에는 이미 공격했습니다.");
            return;
        }

        battleManager.isMoveMode = false;
        battleManager.isAttackMode = false;      // 아직 공격 대상 선택 X
        battleManager.isWeaponSelectMode = true; // 무기 고르는 중

        // 인벤토리 열기
        UIManager.Instance.ShowInventory(true);

        panel.SetActive(false);
    }

    public void OnClickEndTurn()
    {
        if (battleManager.selectedUnit != null)
        {
            battleManager.selectedUnit.SetSelected(false);
        }

        battleManager.StartEnemyTurn();
        panel.SetActive(false);
    }
}
