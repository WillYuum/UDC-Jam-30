using UnityEngine;

[ExecuteAlways]
public class GridRenderer : MonoBehaviour
{
    public Color gridColor = Color.white;
    public float cellSize = 1f;
    public Vector2 gridSize = new Vector2(10, 10);
    public Vector2 offset = Vector2.zero;
    public Rect bounds = new Rect(-5, -5, 10, 10);

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        // Calculate the starting point
        float startX = Mathf.Floor(bounds.xMin / cellSize) * cellSize;
        float endX = Mathf.Ceil(bounds.xMax / cellSize) * cellSize;

        float startY = Mathf.Floor(bounds.yMin / cellSize) * cellSize;
        float endY = Mathf.Ceil(bounds.yMax / cellSize) * cellSize;

        // Draw vertical lines
        for (float x = startX; x <= endX; x += cellSize)
        {
            Gizmos.DrawLine(new Vector3(x, bounds.yMin, 0) + (Vector3)offset, new Vector3(x, bounds.yMax, 0) + (Vector3)offset);
        }

        // Draw horizontal lines
        for (float y = startY; y <= endY; y += cellSize)
        {
            Gizmos.DrawLine(new Vector3(bounds.xMin, y, 0) + (Vector3)offset, new Vector3(bounds.xMax, y, 0) + (Vector3)offset);
        }
    }
}
