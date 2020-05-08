using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldSpawner : MonoBehaviour
{
    public GameObject spawnPrefab;
    public int minObjectsPerCell = 3;
    public int maxObjectsPerCell = 6;
    private Transform playerShipTransform;

    [SerializeField]
    private int cellSize = 64;
    private float halfCellSize;

    private Dictionary<int, Dictionary<int, bool>> cells;

    private int prevX = int.MaxValue;
    private int prevY = int.MaxValue;

    public Text debugText;

    private void Awake()
    {
        halfCellSize = cellSize / 2;
        playerShipTransform = GameObject.FindGameObjectWithTag("PlayerShip").transform;
        cells = new Dictionary<int, Dictionary<int, bool>>
        {
            { 0, new Dictionary<int, bool>() }
        };
        cells[0].Add(0, true);
    }

    private void Update()
    {

        int x = Mathf.RoundToInt((playerShipTransform.position.x / halfCellSize) / 2);
        int y = Mathf.RoundToInt((playerShipTransform.position.y / halfCellSize) / 2);

        if (x != prevX || y != prevY)
        {
            SpawnObjectsInNeighbourhood(x, y);
            prevX = x;
            prevY = y;
        }
    }

    private void SpawnObjectsInCell(int x, int y) {
        int numAsteroids = Random.Range(minObjectsPerCell, maxObjectsPerCell + 1);
        for (int i = 0; i < numAsteroids; i++) {
            Vector2 pos = new Vector2(
                x * cellSize + Random.Range(-halfCellSize, halfCellSize), 
                y * cellSize + Random.Range(-halfCellSize, halfCellSize));
            Instantiate(spawnPrefab, pos, Quaternion.identity);
        }
    }

    private void SpawnObjectsInNeighbourhood(int x, int y) {
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                int xIndex = x + i;
                int yIndex = y + j;
                if (cells.ContainsKey(xIndex))
                {
                    if (!cells[xIndex].ContainsKey(yIndex))
                    {
                        cells[xIndex].Add(yIndex, true);
                        SpawnObjectsInCell(xIndex, yIndex);
                    }
                }
                
                else
                {
                    Dictionary<int, bool> row = new Dictionary<int, bool>
                    {
                        { yIndex, true }
                    };
                    cells.Add(xIndex, row);
                    SpawnObjectsInCell(xIndex, yIndex);
                }
            }
        }
    }

}
