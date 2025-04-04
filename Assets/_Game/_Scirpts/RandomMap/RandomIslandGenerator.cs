using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomIslandGenerator : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public float fillPercent = 0.15f; // Tỷ lệ ô đất ban đầu (thấp hơn sẽ có ít đất hơn)
    public int smoothSteps = 5; // Số lần làm mịn địa hình

    public Tilemap tilemap;
    public TileBase groundTile;
    public TileBase waterTile;

    private int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width, height];

        // Bước 1: Random ô đất/nước ban đầu
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (Random.value < fillPercent) ? 1 : 0;
            }
        }

        // Bước 2: Làm mịn bản đồ
        for (int i = 0; i < smoothSteps; i++)
        {
            SmoothMap();
        }

        // Bước 3: Vẽ lên tilemap
        DrawMap();
    }

    void SmoothMap()
    {
        int[,] newMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWalls = GetSurroundingWallCount(x, y);

                if (neighborWalls > 4)
                    newMap[x, y] = 1; // Nếu có nhiều đất xung quanh thì giữ nguyên đất
                else if (neighborWalls < 4)
                    newMap[x, y] = 0; // Nếu ít đất xung quanh thì biến thành nước
                else
                    newMap[x, y] = map[x, y];
            }
        }

        map = newMap;
    }

    int GetSurroundingWallCount(int x, int y)
    {
        int count = 0;
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                int nx = x + offsetX;
                int ny = y + offsetY;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    count += map[nx, ny];
                }
                else
                {
                    count++; // Nếu ngoài map, coi như có đất
                }
            }
        }
        return count;
    }

    void DrawMap()
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase tile = (map[x, y] == 1) ? groundTile : waterTile;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
