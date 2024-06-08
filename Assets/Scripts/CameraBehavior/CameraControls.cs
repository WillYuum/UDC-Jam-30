using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private Transform _maxBounds;
    [SerializeField] private Transform _minBounds;

    private float _moveSpeed = 0.1f;

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDirection = new Vector2(0.0f, Input.GetAxis("Vertical"));
        Debug.Log(moveDirection);
        if (moveDirection.y > 0)
        {
            Vector3 newCameraPosition = transform.position + new Vector3(0, moveDirection.y, 0) * _moveSpeed;
            if (transform.position.y < _maxBounds.position.y)
            {
                transform.position = newCameraPosition;
            }
        }
        else if (moveDirection.y < 0)
        {
            Vector3 newCameraPosition = transform.position + new Vector3(0, moveDirection.y, 0) * _moveSpeed;
            if (transform.position.y > _minBounds.position.y)
            {
                transform.position = newCameraPosition;
            }

        }

    }
}
