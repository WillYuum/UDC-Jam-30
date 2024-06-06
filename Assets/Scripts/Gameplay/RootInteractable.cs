using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootInteractable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visual;



    void OnMouseOver()
    {
        //highlight sprite render color
        _visual.color = Color.red;
    }

    void OnMouseExit()
    {
        _visual.color = Color.white;
    }

}
