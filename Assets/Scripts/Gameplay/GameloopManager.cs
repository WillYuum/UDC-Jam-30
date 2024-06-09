using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameloopManager : MonoBehaviour
{

    private GameUI _gameUI;


    [SerializeField] public TreeStats TreeStats { get; private set; }
    [SerializeField] public EnergyCostOfLiving EnergyCostOfLiving { get; private set; }

    private GameTicker _gameTicker;

    private float _durationTillDeath = 30f;
    private bool _isInDeathState = false;


    void Awake()
    {
        _gameUI = FindObjectOfType<GameUI>();
        _gameTicker = FindObjectOfType<GameTicker>();

        TreeStats = new TreeStats();
        EnergyCostOfLiving = new EnergyCostOfLiving();


        void InvokeLifeCycleEvents()
        {
            ConvertWaterToEnergy();
            ConusmeCostOfLiving();

            CheckEnergyStatus();


            _gameUI.LevelIndicators.UpdateEnergyLevel(TreeStats.EnergyLevel / TreeStats.MaxEnergyLevel, TreeStats.EnergyLevel);
            _gameUI.LevelIndicators.UpdateWaterLevel(TreeStats.WaterLevel / TreeStats.MaxWaterLevel, TreeStats.WaterLevel);
        }


        _gameTicker.OnTick += InvokeLifeCycleEvents;

    }

    void Start()
    {
        _gameTicker.ToggleTicker(false);

        TreeStats.EnergyLevel = 50;
        TreeStats.WaterLevel = 50;

        StartGameIntro();

        _gameUI.GameTimeText.gameObject.SetActive(false);
    }

    void Update()
    {
        _gameUI.GameTimeText.text = _gameTicker.GameTime.ToString("F2");
    }


    private void StartGameIntro()
    {
        _gameUI.PlayGameIntro(StartGameLoop);
    }

    private void StartGameLoop()
    {
        _gameUI.GameTimeText.gameObject.SetActive(true);
        _gameTicker.ToggleTicker(true);
    }



    public void CollectWater()
    {

        if (TreeStats.IsWaterFull())
        {
            return;
        }

        float collectedWater = TreeStats.WaterAbsorbtionRate;
        TreeStats.WaterLevel += collectedWater;

    }

    private void ConvertWaterToEnergy()
    {
        if (!TreeStats.SufficentWaterForEnergyConversion() || TreeStats.IsEnergyFull())
        {
            return;
        }

        TreeStats.WaterLevel -= TreeStats.WaterAmountForEnergyConversion;
        float waterToConvert = TreeStats.WaterAmountForEnergyConversion;
        float energyConverted = TreeStats.WaterToEnergyLogic.ConvertWaterToEnergy(waterToConvert);
        TreeStats.EnergyLevel += energyConverted;
    }

    private void ConusmeCostOfLiving()
    {
        TreeStats.EnergyLevel -= EnergyCostOfLiving.JustLivingCost;
        TreeStats.WaterLevel -= EnergyCostOfLiving.RootCost;
    }


    private void CheckEnergyStatus()
    {
        if (TreeStats.EnergyLevel <= 0)
        {
            //Enter Death State
            _isInDeathState = true;
        }
        else if (_isInDeathState)
        {
            if (TreeStats.EnergyLevel > 0)
            {
                _isInDeathState = false;
            }

            //Update countdown UI?
        }
    }
}


public class TreeStats
{
    public float EnergyLevel { get; set; }
    public float WaterLevel { get; set; }
    public float MaxEnergyLevel { get; set; }
    public float MaxWaterLevel { get; set; }

    public float WaterAmountForEnergyConversion { get; set; }
    public WaterToEnergyLogic WaterToEnergyLogic { get; set; }

    public float WaterAbsorbtionRate { get; set; }


    public TreeStats()
    {
        WaterToEnergyLogic = new WaterToEnergyLogic();

        EnergyLevel = 0;
        WaterLevel = 0;
        MaxEnergyLevel = 100;
        MaxWaterLevel = 100;


        WaterToEnergyLogic.UpdateWaterToEnergyRate(0.5f); //By Percentage
        WaterAmountForEnergyConversion = 0.4f;
        WaterAbsorbtionRate = 0.3f;
    }

    public bool SufficentWaterForEnergyConversion() => WaterLevel >= WaterAmountForEnergyConversion;

    public bool IsEnergyFull() => EnergyLevel >= MaxEnergyLevel;

    public bool IsWaterFull() => WaterLevel >= MaxWaterLevel;
}

public class WaterToEnergyLogic
{
    private float _waterToEnergyRate;
    public float ConvertWaterToEnergy(float waterAmount)
    {
        return waterAmount * _waterToEnergyRate;
    }

    public void UpdateWaterToEnergyRate(float newRate)
    {
        _waterToEnergyRate = newRate;
    }
}




[System.Serializable]
public class EnergyCostOfLiving
{
    [SerializeField] public float RootCost { get; private set; } = 0.2f;
    [SerializeField] public float JustLivingCost { get; private set; } = 0.3f;


}