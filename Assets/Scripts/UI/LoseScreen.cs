using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private GameObject bg;
    [SerializeField] private Button _restartButton;


    void Awake()
    {
        gameObject.SetActive(false);
    }


    void OnEnable()
    {
        _restartButton.onClick.AddListener(() =>
        {
            GameManager.instance.RestartGame();
        });
    }

    void OnDisable()
    {
        _restartButton.onClick.RemoveAllListeners();
    }
}
