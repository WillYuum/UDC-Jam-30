using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFit : MonoBehaviour
{

    void Start()
    {
        SceneryController sceneryController = FindObjectOfType<SceneryController>();
        Bounds backgroundBounds = sceneryController.GetBackgroundBounds();
        Camera.main.orthographicSize = backgroundBounds.size.x * Screen.height / Screen.width * 0.5f;
    }


}
