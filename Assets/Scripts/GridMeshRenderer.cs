using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider2D))]
public class GridMeshRenderer : MonoBehaviour
{
    private Grid _grid;
    public Color _gridColor = Color.white;
    public float _gridThickness = 0.1f;

    [SerializeField] public int cellsLeft = 5;
    [SerializeField] public int cellsRight = 5;
    [SerializeField] public int cellsTop = 5;
    [SerializeField] public int cellsBottom = 5;

    private BoxCollider2D _boxCollider;



    private MeshRenderer _meshRenderer;
    private Mesh _mesh;
    private Shader gridShader;

    private Vector3[] vertices;
    private int[] indices;

    private void Awake()
    {
        SetupRequiredVariables();
        DrawGridMesh();


        _boxCollider = GetComponent<BoxCollider2D>();
        float boxWidth = (cellsLeft + cellsRight) * _grid.cellSize.x;
        float boxHeight = (cellsTop + cellsBottom) * _grid.cellSize.y;
        _boxCollider.size = new Vector2(boxWidth, boxHeight);

        float xOffset = (cellsRight - cellsLeft) * _grid.cellSize.x / 2;
        float yOffset = (cellsTop - cellsBottom) * _grid.cellSize.y / 2;
        _boxCollider.offset = new Vector2(xOffset, yOffset);
    }

    public void DrawGridMesh()
    {
        float cellSize = _grid.cellSize.x;

        // Calculate grid bounds based on the number of cells
        float gridHeight = (cellsTop + cellsBottom) * cellSize;
        float gridWidth = (cellsLeft + cellsRight) * cellSize;

        int verticalGridLines = cellsLeft + cellsRight;   // Number of vertical lines based on cells
        int horizontalGridLines = cellsTop + cellsBottom; // Number of horizontal lines based on cells

        vertices = new Vector3[(verticalGridLines * 2 + horizontalGridLines * 2) * 2];
        indices = new int[vertices.Length];

        int vIndex = 0;

        // Create vertical lines
        for (int x = 0; x <= verticalGridLines; x++)
        {
            float xPos = (x - cellsLeft) * cellSize;
            vertices[vIndex++] = new Vector3(xPos, -cellsBottom * cellSize, 0);
            vertices[vIndex++] = new Vector3(xPos, cellsTop * cellSize, 0);
            indices[vIndex - 2] = vIndex - 2;
            indices[vIndex - 1] = vIndex - 1;
        }

        // Create horizontal lines
        for (int i = 0; i <= horizontalGridLines; i++)
        {
            float y = (i - cellsBottom) * cellSize;  // Offset from center
            vertices[vIndex++] = new Vector3(-cellsLeft * cellSize, y, 0);
            vertices[vIndex++] = new Vector3(cellsRight * cellSize, y, 0);
            indices[vIndex - 2] = vIndex - 2;
            indices[vIndex - 1] = vIndex - 1;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.SetIndices(indices, MeshTopology.Lines, 0);
        _mesh.RecalculateBounds();

        Material material = new(gridShader)
        {
            color = _gridColor
        };
        material.SetFloat("_LineThickness", _gridThickness);
        _meshRenderer.material = material;
    }

    private void SetupRequiredVariables()
    {
        _grid = GetComponent<Grid>();

        _meshRenderer = GetComponent<MeshRenderer>();

        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        gridShader = Shader.Find("Unlit/Color");
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        float cellSize = _grid.cellSize.x;
        int x = Mathf.FloorToInt(worldPosition.x / cellSize) + cellsLeft;
        int y = Mathf.FloorToInt(worldPosition.y / cellSize) + cellsBottom;
        return new Vector2Int(x, y);
    }

    public Vector3 CellToWorld(Vector2Int cellPosition)
    {
        float cellSize = _grid.cellSize.x;
        float x = (cellPosition.x - cellsLeft) * cellSize;
        float y = (cellPosition.y - cellsBottom) * cellSize;
        return new Vector3(x, y, 0);
    }

    public Vector3 GetCellSize()
    {
        return _grid.cellSize;
    }
}
