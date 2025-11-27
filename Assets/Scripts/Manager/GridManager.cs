using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 12;
    public int height = 8;
    public GameObject tilePrefab;
    public float tileSize = 1.0f;

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

    public void HighlightAttackRange(UnitStatus unit, int minRange, int maxRange)
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

                if (distance >= minRange && distance <= maxRange)
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

    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        int x = tile.x;
        int y = tile.y;

        if (x + 1 < width) neighbors.Add(tiles[x + 1, y]); // 오른쪽
        if (x - 1 >= 0) neighbors.Add(tiles[x - 1, y]); // 왼쪽
        if (y + 1 < height) neighbors.Add(tiles[x, y + 1]); // 위
        if (y - 1 >= 0) neighbors.Add(tiles[x, y - 1]); // 아래

        return neighbors;
    }
    public List<Tile> FindPathBFS(Tile start, Tile goal)
    {
        // start에서 goal까지 가는 "최단 칸 수" 경로를 반환
        List<Tile> path = new List<Tile>();

        if (start == null || goal == null)
            return path;

        Queue<Tile> queue = new Queue<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

        // 1. 시작 타일 셋업
        queue.Enqueue(start);
        visited.Add(start);

        // 2. BFS 루프
        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            if (current == goal)
            {
                break; // 목표 도달!
            }

            foreach (Tile neighbor in GetNeighbors(current))
            {
                // TODO: 아직 방문 안 했으면 queue에 넣고, cameFrom 기록
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        // 3. goal에서 start까지 역추적해서 path 만들기
        if (!visited.Contains(goal))
        {
            // goal에 도달 못했다면 빈 경로 반환
            return path;
        }

        Tile temp = goal;
        while (temp != null && temp != start)
        {
            path.Add(temp);
            if (!cameFrom.ContainsKey(temp)) break;
            temp = cameFrom[temp];
        }
        path.Add(start);
        path.Reverse();

        return path;
    }
}
