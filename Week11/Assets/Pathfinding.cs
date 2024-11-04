using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int next;
    private Vector2Int current;
    [SerializeField]
    private Vector2Int start = new Vector2Int(0, 1);
    [SerializeField]
    private Vector2Int goal = new Vector2Int(4, 4);
    public float obstacleProb = 30;
    [SerializeField]
    private Vector2Int obstacleLocation;
    private float change;

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 0, 1, 0 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0 }
    };

    private void Start()
    {
        GenerateRandomGrid(grid.GetLength(0), grid.GetLength(1), obstacleProb);
        path.Clear();
        FindPath(start, goal);
    }

    private void OnValidate()
    {
        if (change != obstacleProb)
        {
            int width = Random.Range(6, 30);
            int height = Random.Range(6, 30);
            grid = new int[height, width];

            change = obstacleProb;
            GenerateRandomGrid(grid.GetLength(0), grid.GetLength(1), obstacleProb);
        }

        AddObstacle(obstacleLocation);

        path.Clear();
        FindPath(start, goal);
    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }

    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 && point.y < grid.GetLength(0);
    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                next = current + direction;

                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }

        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }

    private void GenerateRandomGrid(int height, int width, float obstacleProb)
    {
        grid = new int[height, width];

        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {     
                grid[y, x] = obstacleProb > Random.Range(1, 100) ? 1 : 0;
            }
        }
        grid[start.y, start.x] = 0;
        grid[goal.y, goal.x] = 0;
    }

    private void AddObstacle(Vector2Int position)
    {
        if (position == start || position == goal)
        {
            grid[position.x, position.y] = 0;
        }
        else
        {
            grid[position.x, position.y] = 1;
        }        
    }
}