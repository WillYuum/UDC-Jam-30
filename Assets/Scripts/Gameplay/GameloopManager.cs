using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameloopManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI GameTimeText;

    private GameTicker _gameTicker;

    void Awake()
    {
        _gameTicker = FindObjectOfType<GameTicker>();
    }

    void Update()
    {
        GameTimeText.text = _gameTicker.GameTime.ToString("F2");
    }
}
