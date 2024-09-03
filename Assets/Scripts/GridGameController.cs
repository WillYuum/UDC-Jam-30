using UnityEngine;

public class GridGameController : MonoBehaviour
{
    [SerializeField] private GridMeshRenderer _gridMeshRenderer;
    [SerializeField] private GameObject _highlightObject; // The object used to highlight cells
    public Color highlightColor = Color.yellow;

    private Vector2Int? _lastHighlightedCell = null;

    void Awake()
    {
        if (_highlightObject == null)
        {
            Debug.LogError("Highlight object is not assigned.");
            return;
        }

        // Set the initial color of the highlight object
        Renderer renderer = _highlightObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = highlightColor;
        }
    }

    void Update()
    {
        // Perform a 2D raycast to check where the mouse is pointing
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);


        if (hit.collider != null)
        {
            Vector3 hitPosition = hit.point;
            Vector2Int cellPos = _gridMeshRenderer.WorldToCell(hitPosition);

            if (_lastHighlightedCell == null || _lastHighlightedCell != cellPos)
            {
                Vector3 highlightPosition = _gridMeshRenderer.CellToWorld(cellPos);
                Vector3 cellSize = _gridMeshRenderer.GetCellSize();

                highlightPosition.x += cellSize.x / 2;
                highlightPosition.y += cellSize.y / 2;

                _highlightObject.transform.position = highlightPosition;
                _lastHighlightedCell = cellPos;

                // Ensure the highlight object is visible
                if (!_highlightObject.activeSelf)
                {
                    _highlightObject.SetActive(true);
                }
            }
        }
        else if (_lastHighlightedCell != null)
        {
            _highlightObject.SetActive(false);
            _lastHighlightedCell = null;
        }
    }
}
