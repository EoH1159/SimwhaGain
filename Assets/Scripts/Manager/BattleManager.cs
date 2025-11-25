
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public bool canMoveThisTurn = true;
    public UnitStatus selectedUnit;   // 현재 선택된 유닛 (일단 플레이어 하나)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

        if (selectedUnit == null)
        {
            Debug.LogWarning("selectedUnit 이 null 입니다!");
            return;
        }
        if (!canMoveThisTurn)
        {
            Debug.Log("이번 턴에는 이미 이동했습니다.");
            return;
        }

        Vector3 unitPos = selectedUnit.transform.position;
        Vector3 tilePos = tile.transform.position;

        selectedUnit.transform.position = new Vector3(tilePos.x, tilePos.y, unitPos.z);

        canMoveThisTurn = false; // 이번 턴 이동 사용 완료
        Debug.Log("유닛 이동 완료");
    }

    public void StartNextTurn()
    {
        canMoveThisTurn = true;
        Debug.Log("다음 턴 시작! 다시 이동할 수 있습니다.");
    }
}
