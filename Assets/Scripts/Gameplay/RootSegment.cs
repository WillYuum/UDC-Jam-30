using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSegment : MonoBehaviour
{

    private LineRenderer _lineRenderer;
    // [SerializeField] private Transform _rootEndNode;

    // [field: SerializeField] public RootInteractable RootInteractable { get; private set; }


    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
    }

    public void SetStartPosition(Vector2 startPosition)
    {
        _lineRenderer.SetPosition(0, startPosition);
        // RootInteractable.gameObject.SetActive(false);
    }


    public void UpdateEndPosition(Vector2 endPosition)
    {
        _lineRenderer.SetPosition(1, endPosition);
    }

    public void SetEndPosition(Vector2 endPosition)
    {
        // _lineRenderer.SetPosition(1, endPosition);
        // RootInteractable.gameObject.SetActive(true);
    }



    public void SetNodeConnection(GameObject node)
    {
        node.transform.position = _lineRenderer.GetPosition(1);
    }

    public Vector2 GetEndPosition()
    {
        return _lineRenderer.GetPosition(0);
    }
}
