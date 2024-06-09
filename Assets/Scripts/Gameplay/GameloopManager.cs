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


            _gameUI.LevelIndicators.UpdateEnergyLevel(TreeStats.EnergyLevel / TreeStats.MaxEnergyLevel.Value, TreeStats.EnergyLevel);
            _gameUI.LevelIndicators.UpdateWaterLevel(TreeStats.WaterLevel / TreeStats.MaxWaterLevel.Value, TreeStats.WaterLevel);
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
        _gameUI.MainUI.SetActive(true);
        _gameTicker.ToggleTicker(true);
    }



    public void CollectWater()
    {

        if (TreeStats.IsWaterFull())
        {
            return;
        }

        float collectedWater = TreeStats.WaterAbsorbtionRate.Value;
        TreeStats.WaterLevel += collectedWater;

    }

    private void ConvertWaterToEnergy()
    {
        if (!TreeStats.SufficentWaterForEnergyConversion() || TreeStats.IsEnergyFull())
        {
            return;
        }

        TreeStats.WaterLevel -= TreeStats.WaterAmountForEnergyConversion.Value;
        float waterToConvert = TreeStats.WaterAmountForEnergyConversion.Value;
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
    public UpgradableAbility MaxEnergyLevel { get; set; }
    public UpgradableAbility MaxWaterLevel { get; set; }

    public UpgradableAbility WaterAmountForEnergyConversion { get; set; }
    public WaterToEnergyLogic WaterToEnergyLogic { get; set; }

    public UpgradableAbility WaterAbsorbtionRate { get; set; }


    public TreeStats()
    {
        WaterToEnergyLogic = new WaterToEnergyLogic();

        EnergyLevel = 0;
        WaterLevel = 0;
        MaxEnergyLevel = new(AbilityType.MaxEnergyLevel, new(new float[] { 50, 100, 150 }));
        MaxWaterLevel = new(AbilityType.MaxWaterLevel, new(new float[] { 100, 150, 200 }));


        WaterToEnergyLogic.UpdateWaterToEnergyRate(0.5f); //By Percentage
        WaterAmountForEnergyConversion = new(AbilityType.WaterAmountForEnergyConversion, new(new float[] { 0.4f, 0.3f, 0.2f }));
        WaterAbsorbtionRate = new(AbilityType.IncreaseWaterAbsortionRate, new(new float[] { 0.3f, 0.5f, 0.7f }));
    }

    public bool SufficentWaterForEnergyConversion() => WaterLevel >= WaterAmountForEnergyConversion.Value;

    public bool IsEnergyFull() => EnergyLevel >= MaxEnergyLevel.Value;

    public bool IsWaterFull() => WaterLevel >= MaxWaterLevel.Value;
}

public class WaterToEnergyLogic
{
    private float _waterToEnergyRate;

    public UpgradableAbility UpgradableAbility { get; set; }


    public WaterToEnergyLogic()
    {
        UpgradableAbility = new(AbilityType.IncreaseWaterConversionRate, new(new float[] { 0.5f, 0.6f, 0.7f }));
    }

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




public enum AbilityType
{
    MaxEnergyLevel,
    MaxWaterLevel,
    MaxRootLength,
    IncreaseWaterAbsortionRate,
    DecreaseLivingCost,
    DecreaseUpgradeCost,
    IncreaseWaterConversionRate,
    WaterAmountForEnergyConversion,
}

public class UpgradableAbility
{
    public AbilityType Type { get; private set; }

    private UpgradeData _data;

    private float _baseUpgradeCost = 5f;

    public UpgradableAbility(AbilityType type, UpgradeData data)
    {
        Type = type;
        _data = data;
    }

    public float GetUpgradeCost() => _baseUpgradeCost * (_data.CurrentLevel + 1);

    public float Value => _data.ValueLevels[_data.CurrentLevel];
    public string GetUpgradeInfo() => _data.GetUpgradeInfo();

    public void Upgrade()
    {
        _data.Upgrade();
    }
}



public class UpgradeData
{
    public int CurrentLevel { get; private set; }
    public float[] ValueLevels { get; private set; }


    public UpgradeData(float[] valueLevels)
    {
        CurrentLevel = 0;
        ValueLevels = valueLevels;
    }

    public void Upgrade()
    {
        if (IsMaxLevel())
        {
            Debug.Log("Max Level Reached");
            return;
        }

        CurrentLevel++;
    }

    public bool IsMaxLevel() => CurrentLevel >= ValueLevels.Length - 1;

    public string GetUpgradeInfo()
    {
        if (IsMaxLevel())
        {
            return "Max Level: " + ValueLevels[CurrentLevel].ToString();
        }

        return ValueLevels[CurrentLevel].ToString() + "->" + ValueLevels[CurrentLevel + 1].ToString();
    }
}