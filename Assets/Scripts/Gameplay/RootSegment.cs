using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSegment : MonoBehaviour
{

    private LineRenderer _lineRenderer;

    [HideInInspector] public List<WaterResource> LinkedResources = new List<WaterResource>();


    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }


    public void SetStartPosition(Vector2 startPosition)
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.SetPosition(0, startPosition);
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

    public void AddResourceConnection(WaterResource[] waterResource)
    {
        LinkedResources.AddRange(waterResource);
    }



    public void SetNodeConnection(GameObject node)
    {
        node.transform.position = _lineRenderer.GetPosition(1);
    }

    public Vector2 GetEndPosition()
    {
        return _lineRenderer.GetPosition(0);
    }

    public bool IsWaterColliding(out WaterResource[] waterResource)
    {
        var boxCast = CreateBoxCastCollider();

        int waterResourceCount = boxCast.Length;
        if (waterResourceCount > 0)
        {
            waterResource = new WaterResource[waterResourceCount];
            for (int i = 0; i < waterResourceCount; i++)
            {
                waterResource[i] = boxCast[i].collider.GetComponent<WaterResource>();
            }
            return true;
        }

        waterResource = null;
        return false;
    }


    public bool IsWaterColliding()
    {
        var boxCast = CreateBoxCastCollider();

        return boxCast.Length > 0;
    }



    private RaycastHit2D[] CreateBoxCastCollider()
    {
        Vector2 endPosition = _lineRenderer.GetPosition(1);
        Vector2 startPosition = _lineRenderer.GetPosition(0);
        Vector2 direction = (endPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, endPosition);

        int waterLayerMask = 1 << 4;
        return Physics2D.BoxCastAll(startPosition, new Vector2(0.1f, distance), 0, direction, distance, waterLayerMask);
    }
}
