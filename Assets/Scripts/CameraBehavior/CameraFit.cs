using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFit : MonoBehaviour
{

    [SerializeField] private float _padding = 1.0f;


    // private SpriteRenderer _boundsToFit;
    float _orthoSize = 5.0f;

    void Start()
    {
        UpdateCameraBoundsRelativeToDiscoveredTrees();
    }

    void Update()
    {
        // Camera.main.orthographicSize = _boundsToFit.bounds.size.x * Screen.height / Screen.width * 0.5f;
        Camera.main.orthographicSize = _orthoSize + _padding;
    }



    private void UpdateCameraBoundsRelativeToDiscoveredTrees()
    {
        GameloopManager gameloopManager = FindObjectOfType<GameloopManager>();

        if (gameloopManager == null)
        {
            return;
        }

        List<GameObject> discoveredTrees = gameloopManager.DiscoveredTrees;

        if (discoveredTrees.Count == 0)
        {
            return;
        }

        //min and max distance from most negative x to most positive x tree
        float minX = discoveredTrees[0].transform.position.x;
        float maxX = discoveredTrees[0].transform.position.x;


        for (int i = 0; i < discoveredTrees.Count; i++)
        {
            if (i == 0)
            {
                minX = discoveredTrees[i].transform.position.x;
                maxX = discoveredTrees[i].transform.position.x;
            }
            else
            {
                if (discoveredTrees[i].transform.position.x < minX)
                {
                    minX = discoveredTrees[i].transform.position.x;
                }
                if (discoveredTrees[i].transform.position.x > maxX)
                {
                    maxX = discoveredTrees[i].transform.position.x;
                }
            }
        }

        float distance = maxX - minX;

        _orthoSize = distance * Screen.height / Screen.width * 0.5f;

    }


}
