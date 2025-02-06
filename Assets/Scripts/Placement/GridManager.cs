using System.Collections.Generic;
using UnityEngine;

namespace Placement
{
    public class GridManager: MonoBehaviour 
    {
        [Range(0, 10)] public int gridWidth = 5;
        [Range(0, 3)] public int gridHeight = 3;
        public float rowSpacing = 1.0f;
        public float cellSize = 1.5f;
        public float laneSeparation = 2f;

        public bool debug;

        public Vector3[,] gridPositions
        {
            get; 
            private set;
        }
        
        private readonly Dictionary<Vector3, GameObject> _occupiedGridSpots = new();
        
        [Header("Debug ONLY")]
        public List<Vector3> spawnPositions = new();
        
        public void Start() 
        {
            spawnPositions.Clear();
            gridPositions = new Vector3[gridWidth, gridHeight];
            GenerateGrid();
        }

        private void GenerateGrid() 
        {
            gridPositions = new Vector3[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++) 
            {
                for (int y = 0; y < gridHeight; y++) 
                {
                    Vector3 worldPos = transform.position + new Vector3(x * cellSize, 0, y * (cellSize + rowSpacing));
                    gridPositions[x, y] = worldPos;
                    
                    // If it's the last column, mark it as a spawn point
                    if (x == gridWidth - 1)
                    {
                        Vector3 spawnPos = worldPos + new Vector3(laneSeparation, 0, 0);
                        spawnPositions.Add(spawnPos);
                    }

                    if (Application.isPlaying)
                    {
                        if (debug)
                        {
                            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            marker.transform.position = worldPos;
                            marker.transform.localScale = Vector3.one * 0.2f;
                            marker.GetComponent<Collider>().enabled = false;
                        }
                    }
                }
            }
        }

        public Vector3 GetClosestGridPosition(Vector3 targetPos) 
        {
            Vector3 closest = gridPositions[0, 0];
            float minDist = Mathf.Infinity;

            foreach (Vector3 pos in gridPositions) 
            {
                float dist = Vector3.Distance(targetPos, pos);
                
                if (dist < minDist && !IsPositionOccupied(pos)) 
                {
                    minDist = dist;
                    closest = pos;
                }
            }

            return closest;
        }
        
        public bool IsPositionOccupied(Vector3 position) 
        {
            return _occupiedGridSpots.ContainsKey(position);
        }

        public void SetOccupied(Vector3 position, GameObject turret)
        {
            _occupiedGridSpots.TryAdd(position, turret);
        }

        public void ClearOccupied(Vector3 position) 
        {
            if (_occupiedGridSpots.ContainsKey(position)) 
            {
                _occupiedGridSpots.Remove(position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            spawnPositions.Clear();
            GenerateGrid();
            
            if (gridPositions == null)
            {
                return;
            }

            //grid positions
            Gizmos.color = Color.green;
            foreach (Vector3 pos in gridPositions)
            {
                Gizmos.DrawCube(pos, Vector3.one * 0.3f);
            }

            //spawn points
            Gizmos.color = Color.red;
            foreach (Vector3 spawnPos in spawnPositions)
            {
                Gizmos.DrawCube(spawnPos, Vector3.one * 0.3f);
            }

            //spacing view
            Gizmos.color = Color.red;
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 lastColumnPos = gridPositions[gridWidth - 1, y];
                Vector3 spawnPos = spawnPositions[y];
                Gizmos.DrawLine(lastColumnPos, spawnPos);
            }
        }
    }
}
