using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridMeshRenderer : MonoBehaviour
{
    public enum AnchorPoint
    {
        Center,
        LeftCenter,
        RightCenter
    }

    private Grid _grid;
    public Color _gridColor = Color.white;
    public float _gridThickness = 0.1f;

    [SerializeField] public int cellsLeft = 5;
    [SerializeField] public int cellsRight = 5;
    [SerializeField] public int cellsTop = 5;
    [SerializeField] public int cellsBottom = 5;

    private MeshRenderer _meshRenderer;
    private Mesh _mesh;
    private Shader gridShader;

    private Vector3[] vertices;
    private int[] indices;

    private void Awake()
    {
        SetupRequiredVariables();
        DrawGridMesh();
    }

    void Update()
    {

    }

    public void DrawGridMesh()
    {
        float cellSize = _grid.cellSize.x;

        // Calculate grid bounds based on the number of cells
        float gridHeight = (cellsTop + cellsBottom) * cellSize;
        float gridWidth = (cellsLeft + cellsRight) * cellSize;

        int verticalGridLines = cellsLeft + cellsRight;   // Number of vertical lines based on cells
        int horizontalGridLines = cellsTop + cellsBottom; // Number of horizontal lines based on cells

        // Initialize vertices and indices arrays
        vertices = new Vector3[(verticalGridLines * 2 + horizontalGridLines * 2) * 2];
        indices = new int[vertices.Length];

        int vIndex = 0;

        // Create vertical lines
        for (int i = 0; i <= verticalGridLines; i++)
        {
            float x = (i - cellsLeft) * cellSize;  // Offset from center
            vertices[vIndex++] = new Vector3(x, -cellsBottom * cellSize, 0);
            vertices[vIndex++] = new Vector3(x, cellsTop * cellSize, 0);
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

        // Set mesh properties
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.SetIndices(indices, MeshTopology.Lines, 0);
        _mesh.RecalculateBounds();

        // Apply grid color and thickness
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
}
