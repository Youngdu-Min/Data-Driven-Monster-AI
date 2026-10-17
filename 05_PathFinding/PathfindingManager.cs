using PathFind;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }
    public PathFind.Grid Grid { get; private set; }

    [Header("Grid Settings")]
    public Vector2Int gridSize = new Vector2Int(50, 50);
    public Vector3 gridOrigin = new Vector3(-25, -25, 0); // 왼쪽 하단 시작점
    public float cellSize = 1f;

    private void Awake()
    {
        Instance = this;

        // 임시로 모든 타일 통과 가능하도록 초기화
        float[,] costs = new float[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
                costs[x, y] = 1.0f;

        Grid = new PathFind.Grid(gridSize.x, gridSize.y, costs);
    }

    public Point WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.Clamp(Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize), 0, gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((worldPos.y - gridOrigin.y) / cellSize), 0, gridSize.y - 1);
        return new Point(x, y);
    }

    public Vector3 GridToWorld(PathFind.Point point)
    {
        return new Vector3(
            (point.x * cellSize) + gridOrigin.x + (cellSize * 0.5f), // 중심점 보정
            (point.y * cellSize) + gridOrigin.y + (cellSize * 0.5f),
            0
        );
    }
}