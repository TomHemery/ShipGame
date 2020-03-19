using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidFieldSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    private int minAsteroidsPerCell = 3;
    private int maxAsteroidsPerCell = 6;
    private Transform playerShipTransform;

    private readonly int cellSize = 64;
    private readonly float halfCellSize = 32f;

    private Dictionary<int, Dictionary<int, bool>> cells;

    private int prevX = int.MaxValue;
    private int prevY = int.MaxValue;

    public Text debugText;

    private void Awake()
    {
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
            SpawnAsteroidsInNeighbourhood(x, y);
            prevX = x;
            prevY = y;
        }

        if (debugText != null) debugText.text = x + ", " + y + "\n" + playerShipTransform.position;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(prevX * cellSize, prevY * cellSize, 0), new Vector3(cellSize, cellSize, 0));
    }

    private void SpawnAteroidsInCell(int x, int y) {
        int numAsteroids = Random.Range(minAsteroidsPerCell, maxAsteroidsPerCell + 1);
        for (int i = 0; i < numAsteroids; i++) {
            Vector2 pos = new Vector2(
                x * cellSize + Random.Range(-halfCellSize, halfCellSize), 
                y * cellSize + Random.Range(-halfCellSize, halfCellSize));
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
