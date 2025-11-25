
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    private void OnMouseDown()
    {
        Debug.Log($"Tile clicked at ({x}, {y})");  // 클릭 로그 추가
        // TODO: 이 타일이 클릭됐다는 걸 "전투 관리 스크립트"에 알려주기
        // 예: BattleManager.Instance.OnTileClicked(this);
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnTileClicked(this);
        }
        else
        {
            Debug.LogWarning("BattleManager.Instance is null!");
        }
    }
}
