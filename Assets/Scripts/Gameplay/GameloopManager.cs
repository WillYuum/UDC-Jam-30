using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameloopManager : MonoBehaviour
{

    private GameUI _gameUI;


    [SerializeField] public TreeStats TreeStats { get; private set; }
    [SerializeField] public EnergyCostOfLiving EnergyCostOfLiving { get; private set; }

    private RootController _rooController;

    private GameTicker _gameTicker;

    public float DurationTillDeath { get; } = 30f;
    private float _deathCountdown = 0f;
    private bool _isInDeathState = false;


    void Awake()
    {
        _gameUI = FindObjectOfType<GameUI>();
        _gameTicker = FindObjectOfType<GameTicker>();

        TreeStats = new TreeStats();
        EnergyCostOfLiving = new EnergyCostOfLiving();

        _rooController = FindObjectOfType<RootController>();


        void InvokeLifeCycleEvents()
        {
            ConvertWaterToEnergy();
            ConsumeEnergyCostOfLiving();

            CheckEnergyStatus();

            float energyLevelVal = TreeStats.EnergyLevel.Value;
            float waterLevelVal = TreeStats.WaterLevel.Value;

            _gameUI.LevelIndicators.UpdateEnergyLevel(energyLevelVal / TreeStats.MaxEnergyLevel.Value, energyLevelVal);
            _gameUI.LevelIndicators.UpdateWaterLevel(waterLevelVal / TreeStats.MaxWaterLevel.Value, waterLevelVal);
        }


        _gameTicker.OnTick += InvokeLifeCycleEvents;

        _gameTicker.ToggleTicker(false);

        ToggleLoop(false);
    }

    void Start()
    {
        _gameUI.GameTimeText.gameObject.SetActive(false);
    }

    void Update()
    {
        _gameUI.GameTimeText.text = _gameTicker.GameTime.ToString("F2");

        if (_isInDeathState)
        {
            // Debug.Log("Death Countdown: " + _deathCountdown);
            _deathCountdown -= Time.deltaTime;
            if (_deathCountdown > 0.0)
            {
                _gameUI.DeathCountDownController.UpdateDeathCountdown(_deathCountdown);
            }
            else if (_deathCountdown <= 0.0)
            {
                HandleLoseGame();
                ToggleLoop(false);
            }
        }
    }

    private void ToggleLoop(bool active)
    {
        enabled = active;
    }

    public void StartGame()
    {
        StartGameIntro();
    }


    private void StartGameIntro()
    {
        _gameUI.PlayGameIntro(StartGameLoop);
    }

    private void StartGameLoop()
    {
        TreeStats.EnergyLevel.Set(TreeStats.MaxEnergyLevel.Value * 0.07f);
        TreeStats.WaterLevel.Set(TreeStats.MaxWaterLevel.Value * 0.0f);

        _gameUI.DeathCountDownController.ToggleDeathCountdown(false);

        SeasonTimer seasonTimer = FindObjectOfType<SeasonTimer>();
        seasonTimer.StartSeasonTimer();

        _rooController.UpdateRootInteractables();

        _gameUI.GameTimeText.gameObject.SetActive(true);
        _gameUI.MainUI.SetActive(true);
        _gameTicker.ToggleTicker(true);

        seasonTimer.OnSeasonChange += (season) =>
        {
            int CurrentLayer = 0;
            switch (season)
            {
                case SeasonTimer.Season.Summer:
                    CurrentLayer = 1;
                    break;
                case SeasonTimer.Season.Autumn:
                    CurrentLayer = 2;
                    break;
                case SeasonTimer.Season.Winter:
                    CurrentLayer = 4;
                    break;
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Level", CurrentLayer);
        };
        string endGameEvent = "event:/Music";
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Level", 1);
        FMODUnity.RuntimeManager.PlayOneShot(endGameEvent);


        SetupListentingToEndGame();

        ToggleLoop(true);
    }


    private void SetupListentingToEndGame()
    {
        SeasonTimer seasonTimer = FindObjectOfType<SeasonTimer>();
        seasonTimer.OnSeasonChange += (season) =>
        {
            switch (season)
            {
                case SeasonTimer.Season.Summer:
                    WinGame();
                    break;
                default:
                    break;
            }
        };
    }


    private void WinGame()
    {
        // GameUI gameUI = FindObjectOfType<GameUI>();
        // _gameUI.WinScreen.gameObject.SetActive(true);
        _gameUI.WinScreen.PlayWinScreen();
        _gameTicker.ToggleTicker(false);
    }


    private void HandleLoseGame()
    {
        Debug.Log("|Game Loop| Lose Game Triggered");

        // GameUI gameUI = FindObjectOfType<GameUI>();
        _gameUI.LoseScreen.gameObject.SetActive(true);
        _gameTicker.ToggleTicker(false);
    }


    public bool CollectWater()
    {

        if (TreeStats.IsWaterFull())
        {
            return false;
        }

        float collectedWater = TreeStats.WaterAbsorbtionRate.Value;
        TreeStats.WaterLevel.Add(collectedWater);
        return true;
    }

    private void ConvertWaterToEnergy()
    {
        if (!TreeStats.SufficentWaterForEnergyConversion() || TreeStats.IsEnergyFull())
        {
            return;
        }

        TreeStats.WaterLevel.Consume(TreeStats.WaterAmountForEnergyConversion.Value);
        float waterToConvert = TreeStats.WaterAmountForEnergyConversion.Value;
        float energyConverted = TreeStats.WaterToEnergyLogic.ConvertWaterToEnergy(waterToConvert);
        TreeStats.EnergyLevel.Add(energyConverted);
    }

    private void ConsumeEnergyCostOfLiving()
    {
        TreeStats.EnergyLevel.Consume(EnergyCostOfLiving.JustLivingCost * _rooController.RootCount);
        TreeStats.EnergyLevel.Consume(EnergyCostOfLiving.RootCost);
    }


    private void CheckEnergyStatus()
    {
        switch (_isInDeathState)
        {
            case true:
                if (TreeStats.EnergyLevel.Value > 0)
                {
                    _isInDeathState = false;
                    _deathCountdown = DurationTillDeath;
                    _gameUI.DeathCountDownController.ToggleDeathCountdown(false);
                    _gameUI.DeathCountDownController.UpdateDeathCountdown(0.0f);
                }
                break;
            case false:
                if (TreeStats.EnergyLevel.Value <= 0)
                {
                    _isInDeathState = true;
                    _deathCountdown = DurationTillDeath;
                    _gameUI.DeathCountDownController.UpdateDeathCountdown(_deathCountdown);
                    _gameUI.DeathCountDownController.ToggleDeathCountdown(true);
                }
                break;
        }
    }

}


public class TreeStats
{
    public Consumable EnergyLevel { get; set; }
    public Consumable WaterLevel { get; set; }
    public UpgradableAbility MaxEnergyLevel { get; set; }
    public UpgradableAbility MaxWaterLevel { get; set; }

    public UpgradableAbility WaterAmountForEnergyConversion { get; set; }
    public WaterToEnergyLogic WaterToEnergyLogic { get; set; }

    public UpgradableAbility WaterAbsorbtionRate { get; set; }


    public TreeStats()
    {
        WaterToEnergyLogic = new WaterToEnergyLogic();

        EnergyLevel = new(0);
        WaterLevel = new(0);
        MaxEnergyLevel = new(AbilityType.MaxEnergyLevel, new(new float[] { 50, 100, 150 }));
        MaxWaterLevel = new(AbilityType.MaxWaterLevel, new(new float[] { 100, 150, 200 }));


        WaterToEnergyLogic.UpdateWaterToEnergyRate(0.5f); //By Percentage
        WaterAmountForEnergyConversion = new(AbilityType.WaterAmountForEnergyConversion, new(new float[] { 0.4f, 0.3f, 0.2f }));
        WaterAbsorbtionRate = new(AbilityType.IncreaseWaterAbsortionRate, new(new float[] { 0.3f, 0.5f, 0.7f }));
    }

    public bool SufficentWaterForEnergyConversion() => WaterLevel.Value > 0;

    public bool IsEnergyFull() => EnergyLevel.Value >= MaxEnergyLevel.Value;

    public bool IsWaterFull() => WaterLevel.Value >= MaxWaterLevel.Value;
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

public class Consumable
{
    public float Value { get; private set; }

    public Consumable(float value)
    {
        Value = value;
    }

    public void Consume(float amount)
    {
        Value -= amount;
        Value = Mathf.Max(0, Value);
    }

    public void Add(float amount)
    {
        Value += amount;
    }

    public void Set(float value)
    {
        Value = value;
        Value = Mathf.Max(0, Value);
    }

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