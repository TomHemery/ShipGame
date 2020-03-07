using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public int numAsteroidsPerCell = 6;
    private Transform playerShipTransform;

    private int cellSize = 64;

    private Dictionary<int, Dictionary<int, bool>> cells;

    private void Awake()
    {
        playerShipTransform = GameObject.FindGameObjectWithTag("PlayerShip").transform;
        cells = new Dictionary<int, Dictionary<int, bool>>();
        SpawnAsteroidsInNeighbourhood(0, 0);
    }

    private void Update()
    {
        int x = (int)playerShipTransform.position.x / cellSize;
        int y = (int)playerShipTransform.position.y / cellSize;
        SpawnAsteroidsInNeighbourhood(x, y);
    }

    private void SpawnAteroidsInCell(int x, int y) {
        for (int i = 0; i < numAsteroidsPerCell; i++) {
            Vector2 pos = new Vector2(x * cellSize + Random.Range(0f, cellSize), y * cellSize + Random.Range(0f, cellSize));
            Instantiate(asteroidPrefab, pos, Quaternion.identity);
        }
    }

    private void SpawnAsteroidsInNeighbourhood(int x, int y) {
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                int xIndex = x + i;
                int yIndex = y + j;
                if (cells.ContainsKey(xIndex))
                {
                    if (!cells[xIndex].ContainsKey(yIndex))
                    {
                        cells[xIndex].Add(yIndex, true);
                        SpawnAteroidsInCell(xIndex, yIndex);
                    }
                }
                else
                {
                    Dictionary<int, bool> row = new Dictionary<int, bool>
                    {
                        { yIndex, true }
                    };
                    cells.Add(xIndex, row);
                    SpawnAteroidsInCell(xIndex, yIndex);
                }
            }
        }
    }

}
