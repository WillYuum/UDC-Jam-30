using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFit : MonoBehaviour
{

    private SpriteRenderer _boundsToFit;

    void Start()
    {
        SceneryController sceneryController = FindObjectOfType<SceneryController>();
        _boundsToFit = sceneryController.GetBackgroundBounds();
    }

    void Update()
    {
        Camera.main.orthographicSize = _boundsToFit.bounds.size.x * Screen.height / Screen.width * 0.5f;
    }


}
