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
    public UnitStatus selectedUnit;   // 현재 선택된 유닛 (일단 플레이어 하나)
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

        Vector3 unitPos = selectedUnit.transform.position;
        Vector3 tilePos = tile.transform.position;

        selectedUnit.transform.position = new Vector3(tilePos.x, tilePos.y, unitPos.z);
        selectedUnit.currentTile = tile;

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
                    Vector3 pos = unit.transform.position;
                    Vector3 targetPos = nextTile.transform.position;
                    unit.transform.position = new Vector3(targetPos.x, targetPos.y, pos.z);
                    unit.currentTile = nextTile;
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
}
