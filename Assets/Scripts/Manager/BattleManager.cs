using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum TurnPhase
    {
        Player,
        Enemy
    }

    public TurnPhase currentPhase = TurnPhase.Player;

    public static BattleManager Instance { get; private set; }

    public bool canMoveThisTurn = true;
    public bool isMoveMode = false;
    public bool isAttackMode = false;
    public bool isWeaponSelectMode = false;
    public bool hasAttackedThisTurn = false;

    public UnitStatus selectedUnit;   // 현재 선택된 유닛 (일단 플레이어 하나)
    public InventoryItem pendingWeapon;     //  이번 공격에 쓸 무기
    public GridManager gridManager;

    public TMP_Text turnText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (gridManager == null) return;

        // 1) 플레이어 유닛 시작 타일 세팅
        if (selectedUnit != null)
        {
            Tile startTile = gridManager.GetClosestTile(selectedUnit.transform.position);
            if (startTile != null)
            {
                selectedUnit.currentTile = startTile;
            }

            // 적 유닛들도 타일 세팅
            UnitStatus[] allUnits = FindObjectsOfType<UnitStatus>();
            foreach (var unit in allUnits)
            {
                if (unit.faction == UnitStatus.Faction.Enemy)
                {
                    Tile t = gridManager.GetClosestTile(unit.transform.position);
                    if (t != null)
                        unit.currentTile = t;
                    t.occupant = unit;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNextTurn();
        }
    }

    public void OnTileClicked(Tile tile)
    {
        // 플레이어 턴이 아니면 바로 리턴
        if (currentPhase != TurnPhase.Player)
        {
            Debug.Log("지금은 플레이어 턴이 아닙니다.");
            return;
        }

        Debug.Log($"OnTileClicked! tile=({tile.x}, {tile.y})");

        if (!isMoveMode)
        {
            Debug.Log("지금은 이동 모드가 아닙니다.");
            return;
        }
        if (selectedUnit.currentTile == null)
        {
            // 아직 시작 타일이 안 세팅되어 있으면,
            // 첫 클릭한 타일을 현재 타일로 간주하고 그대로 이동 허용
            Debug.Log("currentTile 이 비어 있어서, 처음 클릭한 타일을 시작 위치로 설정합니다.");
        }
        else
        {
            int distance = Mathf.Abs(selectedUnit.currentTile.x - tile.x)
                         + Mathf.Abs(selectedUnit.currentTile.y - tile.y);
            if (distance > selectedUnit.moveRange)
            {
                Debug.Log("이 타일은 이동 범위를 벗어났습니다.");
                return;
            }
        }

        if (!canMoveThisTurn)
        {
            Debug.Log("이번 턴에는 이미 이동했습니다.");
            return;
        }
        // 1) 타겟 타일에 다른 유닛이 서 있는지 체크
        if (tile.occupant != null && tile.occupant != selectedUnit)
        {
            Debug.Log("이 타일에는 이미 다른 유닛이 서 있습니다.");
            return;
        }

        Vector3 unitPos = selectedUnit.transform.position;
        Vector3 tilePos = tile.transform.position;
        // 2) 이전 타일 occupant 비우기
        if (selectedUnit.currentTile != null)
        {
            selectedUnit.currentTile.occupant = null;
        }

        selectedUnit.transform.position = new Vector3(tilePos.x, tilePos.y, unitPos.z);
        selectedUnit.currentTile = tile;
        tile.occupant = selectedUnit;

        canMoveThisTurn = false; // 이번 턴 이동 사용 완료
        isMoveMode = false;      // 이동 모드 종료
        if (gridManager != null)
        {
            gridManager.ClearAllHighlights();
        }
        if (selectedUnit != null)
        {
            selectedUnit.SetSelected(false);
        }
        Debug.Log("유닛 이동 완료");
    }

    public void StartPlayerTurn()
    {
        currentPhase = TurnPhase.Player;
        canMoveThisTurn = true;
        isMoveMode = false;
        isAttackMode = false;
        isWeaponSelectMode = false;
        hasAttackedThisTurn = false; 
        pendingWeapon = null;

        if (gridManager != null)
            gridManager.ClearAllHighlights();

        if (turnText != null)
            turnText.text = "플레이어 턴";

        Debug.Log("플레이어 턴 시작");
    }

    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }
    private System.Collections.IEnumerator EnemyTurnRoutine()
    {
        currentPhase = TurnPhase.Enemy;
        canMoveThisTurn = false;
        isMoveMode = false;
        isAttackMode = false;
        isWeaponSelectMode = false;

        if (gridManager != null)
            gridManager.ClearAllHighlights();

        if (turnText != null)
            turnText.text = "적 턴";

        Debug.Log("적 턴 시작");

        // 0.5초 정도 적 턴 화면 보여주기
        yield return new WaitForSeconds(0.5f);

        // 여기서 적 AI 행동 실행
        UnitStatus[] allUnits = FindObjectsOfType<UnitStatus>();
        foreach (var unit in allUnits)
        {
            if (unit.faction == UnitStatus.Faction.Enemy)
            {
                Tile enemyTile = unit.currentTile;
                Tile playerTile = selectedUnit.currentTile;
                if (enemyTile == null || playerTile == null) continue;

                List<Tile> path = gridManager.FindPathBFS(enemyTile, playerTile);

                if (path.Count > 1)
                {
                    Tile nextTile = path[1];

                    //  1) 다음 타일에 누가 서 있는지 확인
                    if (nextTile.occupant != null && nextTile.occupant != unit)
                    {
                        // 플레이어나 다른 적이 이미 서 있음 → 이 적은 이번 턴 이동 안 함
                        Debug.Log($"{unit.name} 이동 불가: {nextTile.occupant.name} 이(가) 자리 차지 중");
                        continue;
                    }

                    //  2) 이전 타일 occupant 비우기
                    if (enemyTile.occupant == unit)
                    {
                        enemyTile.occupant = null;
                    }

                    //  3) 실제 위치 이동
                    Vector3 pos = unit.transform.position;
                    Vector3 targetPos = nextTile.transform.position;
                    unit.transform.position = new Vector3(targetPos.x, targetPos.y, pos.z);

                    //  4) currentTile + occupant 갱신
                    unit.currentTile = nextTile;
                    nextTile.occupant = unit;
                }
            }
        }

        // (선택) 적 이동이 보이도록 조금 더 기다릴 수도 있음
        yield return new WaitForSeconds(0.2f);

        // 플레이어 턴 시작
        StartPlayerTurn();
    }

    public void StartNextTurn()
    {
        if (gridManager != null)
        {
            gridManager.ClearAllHighlights();
        }
        canMoveThisTurn = true;
        isMoveMode = false;
        Debug.Log("다음 턴 시작! 다시 이동할 수 있습니다.");
    }

    public void SetSelectedUnit(UnitStatus unit)
    {
        // 이전 선택 유닛 해제
        if (selectedUnit != null && selectedUnit != unit)
        {
            selectedUnit.SetSelected(false);
        }

        selectedUnit = unit;

        // 새 유닛 선택
        if (selectedUnit != null)
        {
            selectedUnit.SetSelected(true);
        }
    }

    public void Attack(UnitStatus attacker, UnitStatus target)
    {
        if (!CanAttack(attacker, target))
            return;

        if (!IsInWeaponRange(attacker, target))
            return;

        int damage = CalculateDamage(attacker, target, pendingWeapon);
        ApplyDamageAndExp(attacker, target, damage);

        // 6. 무기 내구도 처리
        if (pendingWeapon != null && pendingWeapon.data != null && pendingWeapon.data.isEquipment)
            {
                pendingWeapon.currentDurability--;
                Debug.Log($"무기 내구도 감소: {pendingWeapon.currentDurability}/{pendingWeapon.data.maxDurability}");

                // 내구도 0 이하 → 부서짐
                if (pendingWeapon.currentDurability <= 0)
                {
                    var inv = attacker.GetComponent<Inventory>();
                    if (inv != null)
                    {
                        inv.RemoveItem(pendingWeapon); // Inventory가 InventoryItem Remove하도록 되어 있어야 함
                    }
                    Debug.Log($"무기 {pendingWeapon.data.itemName} 이(가) 부서졌습니다.");
                }
            }

            // 7. 공격 종료 후 상태 정리
            hasAttackedThisTurn = true;
            isAttackMode = false;
            canMoveThisTurn = false;
            pendingWeapon = null;

            if (gridManager != null)
            {
                gridManager.ClearAllHighlights();   // 공격 범위 표시 지우기
            }
            if (attacker != null)
            {
                attacker.SetSelected(false);
            }

            var ui = FindObjectOfType<InventoryUI>();
            if (ui != null)
            {
                ui.Refresh();
            }
        }
    private bool CanAttack(UnitStatus attacker, UnitStatus target)
    {
        if (currentPhase != TurnPhase.Player)
        {
            Debug.Log("지금은 플레이어 턴이 아닙니다.");
            return false;
        }
        if (!isAttackMode)
        {
            Debug.Log("공격 모드가 아닙니다.");
            return false;
        }
        if (attacker == null || target == null)
        {
            Debug.LogWarning("공격자 또는 대상이 null입니다.");
            return false;
        }
        if (attacker.currentTile == null || target.currentTile == null)
        {
            Debug.LogWarning("타일 정보가 없습니다.");
            return false;
        }

        return true;
    }
    private bool IsInWeaponRange(UnitStatus attacker, UnitStatus target)
    {
        int minRange = 1;
        int maxRange = 1;
        if (pendingWeapon != null && pendingWeapon.data != null)
        {
            minRange = pendingWeapon.data.minAttackRange;
            maxRange = pendingWeapon.data.attackRange;
        }

        int dist = Mathf.Abs(attacker.currentTile.x - target.currentTile.x)
                 + Mathf.Abs(attacker.currentTile.y - target.currentTile.y);

        // 최소/최대 범위 체크
        if (dist < minRange || dist > maxRange)
        {
            Debug.Log($"대상이 사거리({minRange}~{maxRange}) 밖에 있습니다.");
            return false;   // 사거리 바깥이니까 공격 불가
        }
        return true;        // 여기까지 왔다는 건 사거리 안이라 OK
    }
    private int CalculateDamage(UnitStatus attacker, UnitStatus target, InventoryItem weapon)
    {
        // 3. 데미지 계산 (무기 포함)
        int weaponAttack = 0;
        if (weapon != null && weapon.data != null)
        {
            weaponAttack = weapon.data.bonusAttack;
        }

        int totalAttack = attacker.attack + weaponAttack;
        int damage = Mathf.Max(totalAttack - target.defense, 0);

       return damage;
    }
    private void ApplyDamageAndExp(UnitStatus attacker, UnitStatus target, int damage)
    {
        // 4. HP 감소
        target.currentHP -= damage;
        Debug.Log($"{attacker.name} 이(가) {target.name} 을(를) 공격! 데미지: {damage}, 남은 HP: {target.currentHP}");

        // 공격 EXP (플레이어만)
        if (attacker.faction == UnitStatus.Faction.Player)
        {
            attacker.GainExp(25);
        }

        // 5. 죽었는지 체크 먼저
        if (target.currentHP <= 0)
        {
            // 처치 EXP (플레이어만)
            if (attacker.faction == UnitStatus.Faction.Player)
            {
                attacker.GainExp(50);
            }

            target.Die();
        }
        else
        {
            // 살아있는 경우에만 피격 연출 실행
            StartCoroutine(target.HitFlash());
        }
    }

    public void OnWeaponSelected(InventoryItem weapon)
    {
        if (!isWeaponSelectMode)
            return;

        if (selectedUnit == null)
            return;

        if (weapon == null || weapon.data == null || !weapon.data.isEquipment || weapon.IsBroken)
        {
            Debug.Log("사용할 수 없는 무기입니다.");
            return;
        }

        pendingWeapon = weapon;
        isWeaponSelectMode = false;
        isAttackMode = true;

        if (gridManager != null)
        {
            int min = weapon.data.minAttackRange;
            int max = weapon.data.attackRange;
            gridManager.HighlightAttackRange(selectedUnit, min, max);
        }

        UIManager.Instance.ShowInventory(false);

        // 인벤토리 닫기
        UIManager.Instance.ShowInventory(false);

        Debug.Log($"무기 선택: {weapon.data.itemName} 이제 적을 클릭하면 공격합니다.");
    }
}
