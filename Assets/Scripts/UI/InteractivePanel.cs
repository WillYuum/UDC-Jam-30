using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractivePanel : MonoBehaviour
{
    [SerializeField] private Button _buildButton;
    [SerializeField] private Button _closeBuildButton;


    void Awake()
    {
        GameloopManager gameloopManager = FindObjectOfType<GameloopManager>();

        _buildButton.onClick.AddListener(() =>
        {
            gameloopManager.EnterBuildMode();
            ToggleBuildMode(true);
        });

        _closeBuildButton.onClick.AddListener(() =>
        {
            gameloopManager.ExitBuildMode();
            ToggleBuildMode(false);
        });
    }


    private void ToggleBuildMode(bool active)
    {
        _buildButton.gameObject.SetActive(!active);
        _closeBuildButton.gameObject.SetActive(active);
    }
}
