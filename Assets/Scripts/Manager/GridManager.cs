using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 5;
    public GameObject tilePrefab;
    public float tileSize = 1.5f;

    public Tile[,] tiles;   // (x,y) 위치의 타일을 기억하는 2차원 배열

    void Awake()
    {
        Generate();
    }

    void Generate()
    {
        tiles = new Tile[width, height];   // 배열 생성

        // 격자의 왼쪽 아래 시작점 구하기
        float startX = -(width - 1) * tileSize / 2f;
        float startY = -(height - 1) * tileSize / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float posX = startX + x * tileSize;
                float posY = startY + y * tileSize;

                Vector2 position = new Vector2(posX, posY);
                var tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                Tile tile = tileObj.GetComponent<Tile>();
                tile.Init(x, y);

                tiles[x, y] = tile;   // 배열에 저장
            }
        }
    }

    // 전 타일 하이라이트 끄기
    public void ClearAllHighlights()
    {
        if (tiles == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] != null)
                    tiles[x, y].SetHighlight(false);
            }
        }
    }

    // 특정 유닛의 이동 범위 표시
    public void HighlightMoveRange(UnitStatus unit)
    {
        ClearAllHighlights();
        if (unit == null || unit.currentTile == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                if (tile == null) continue;

                int distance = Mathf.Abs(unit.currentTile.x - tile.x)
                             + Mathf.Abs(unit.currentTile.y - tile.y);

                if (distance <= unit.moveRange)
                {
                    tile.SetHighlight(true);
                }
            }
        }
    }

    public Tile GetClosestTile(Vector3 worldPos)
    {
        Tile closestTile = null;
        float bestDistance = Mathf.Infinity;
        foreach (var tile in tiles)
        {
            if (tile == null) continue;  // 혹시 모를 null 방지

            // Distance 대신 sqrMagnitude 쓰면 살짝 더 효율적
            float distance = (tile.transform.position - worldPos).sqrMagnitude;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                closestTile = tile;
            }
        }
        return closestTile;
    }
}
