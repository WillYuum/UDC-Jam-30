using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelIndicators : MonoBehaviour
{
    [SerializeField] private Slider _energySlider;
    [SerializeField] private Slider _waterSlider;

    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TextMeshProUGUI _waterText;


    public void UpdateEnergyLevel(float energyLevelRatio, float energyLevel)
    {
        _energySlider.value = energyLevelRatio;
        _energyText.text = energyLevel.ToString("F2");
    }

    public void UpdateWaterLevel(float waterLevelRaio, float waterLevel)
    {
        _waterSlider.value = waterLevelRaio;
        _waterText.text = waterLevel.ToString("F2");
    }
}
