using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreenh : MonoBehaviour
{
    [SerializeField] private Button _closeButton;


    void OnEnable()
    {
        _closeButton.onClick.AddListener(OnCloseScreen);
    }

    void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnCloseScreen);
    }


    private void OnCloseScreen()
    {
        GameTicker gameTicker = FindObjectOfType<GameTicker>();
        gameTicker.ToggleTicker(true);

        gameObject.SetActive(false);
    }
}
