
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 5;
    public GameObject tilePrefab;
    public float tileSize = 1.5f;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
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
                Instantiate(tilePrefab, position, Quaternion.identity, transform);
                var tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                Tile tile = tileObj.GetComponent<Tile>();
                tile.Init(x, y);
            }
        }
    }
}
