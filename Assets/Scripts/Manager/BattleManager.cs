
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public bool canMoveThisTurn = true;
    public bool isMoveMode = false;
    public UnitStatus selectedUnit;   // 현재 선택된 유닛 (일단 플레이어 하나)
    public GridManager gridManager;

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
        if (selectedUnit != null && gridManager != null)
        {
            Tile startTile = gridManager.GetClosestTile(selectedUnit.transform.position);
            selectedUnit.currentTile = startTile;
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
        Debug.Log("유닛 이동 완료");
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
}
