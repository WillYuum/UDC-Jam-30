using Unity.VisualScripting;
using UnityEngine;

public class RootController : MonoBehaviour
{
    [SerializeField] public GameObject _rootLineRenderer;
    private float _maxSegmentLength = 1.5f;


    public int RootCount { get; private set; } = 0;

    private Vector2 _startPosition;
    private bool _isDragging = false;
    private RootSegment _spawnedRoot;

    [SerializeField] private RootInteractable _rootInteractablePrefab;
    [SerializeField] private Transform _rootInteractableHolder;
    [SerializeField] private GameObject _renderLinesHolder;



    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);



        if (Input.GetMouseButtonDown(0))
        {
            if (CheckIfClickedOnRootInteractable(mousePosition, out RootInteractable rootInteractable))
            {
                StartNewSegment(rootInteractable.transform.position);
            }
        }
        else if (Input.GetMouseButton(0) && _isDragging)
        {
            UpdateCurrentSegment(mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            EndCreateRootSegment(mousePosition);
        }
    }

    public void UpdateRootInteractables()
    {
        //Destroy all root interactables
        foreach (Transform child in _rootInteractableHolder)
        {
            Destroy(child.gameObject);
        }

        //add new root interactables
        RootSegment[] rootSegments = _renderLinesHolder.GetComponentsInChildren<RootSegment>();

        foreach (RootSegment rootSegment in rootSegments)
        {
            Instantiate(_rootInteractablePrefab, rootSegment.GetEndPosition(), Quaternion.identity);
            // rootSegment.SetEndPosition(rootSegment.transform.position);
        }
    }


    private bool CheckIfClickedOnRootInteractable(Vector2 mousePosition, out RootInteractable rootInteractable)
    {
        RaycastHit2D raycast = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (raycast.collider != null)
        {
            rootInteractable = raycast.collider.GetComponent<RootInteractable>();
        }
        else
        {
            rootInteractable = null;
        }

        return rootInteractable != null;
    }

    private void StartNewSegment(Vector2 position)
    {
        _startPosition = position;

        _spawnedRoot = Instantiate(_rootLineRenderer).GetComponent<RootSegment>();
        _spawnedRoot.SetStartPosition(position);
        _spawnedRoot.UpdateEndPosition(position);

        _isDragging = true;

    }

    private void UpdateCurrentSegment(Vector2 currentMousePosition)
    {
        Vector2 direction = currentMousePosition - _startPosition;
        float distance = direction.magnitude;

        if (distance > _maxSegmentLength)
        {
            currentMousePosition = _startPosition + direction.normalized * _maxSegmentLength;
        }

        _spawnedRoot.UpdateEndPosition(currentMousePosition);
    }

    private void EndCreateRootSegment(Vector2 mousePosition)
    {
        bool CheckIfCollidingWithOtherNode()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, 0.1f);
            foreach (Collider2D collider in colliders)
            {

                if (collider.GetComponent<RootInteractable>() != null)
                {
                    return true;
                }
            }
            return false;
        }

        if (CheckIfCollidingWithOtherNode())
        {
            Destroy(_spawnedRoot.gameObject);
            _isDragging = false;
            return;
        }

        if (_spawnedRoot.IsWaterColliding(out WaterResource[] waterResources))
        {
            _spawnedRoot.AddResourceConnection(waterResources);
            CollectWaterObserver.instance.ObserveRoot(_spawnedRoot);
        }

        _spawnedRoot.SetEndPosition(mousePosition);
        _spawnedRoot.SetNodeConnection(Instantiate(_rootInteractablePrefab, mousePosition, Quaternion.identity).gameObject);
        _isDragging = false;
        _spawnedRoot = null;


        RootCount += 1;
    }


}
