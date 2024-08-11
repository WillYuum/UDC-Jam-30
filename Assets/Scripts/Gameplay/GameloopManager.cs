using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameloopManager : MonoBehaviour
{

    private GameUI _gameUI;

    [HideInInspector] public List<GameObject> DiscoveredTrees { get; private set; }

    [SerializeField] public TreeStats TreeStats { get; private set; }
    [SerializeField] public EnergyCostOfLiving EnergyCostOfLiving { get; private set; }

    private RootController _rooController;

    private GameTicker _gameTicker;

    public float DurationTillDeath { get; } = 30f;
    private float _deathCountdown = 0f;
    private bool _isInDeathState = false;

    private TimeManager _timeManager;


    void Awake()
    {
        _timeManager = FindObjectOfType<TimeManager>();
        _timeManager.enabled = false;

        _gameUI = FindObjectOfType<GameUI>();
        _gameTicker = FindObjectOfType<GameTicker>();

        TreeStats = new TreeStats();
        EnergyCostOfLiving = new EnergyCostOfLiving();

        _rooController = FindObjectOfType<RootController>();

        DiscoveredTrees = new List<GameObject>();
        DiscoveredTrees.AddRange(GameObject.FindGameObjectsWithTag("Tree"));


        void InvokeLifeCycleEvents()
        {
            float energyCollected = ConvertWaterToEnergy();
            ConsumeEnergyCostOfLiving();

            CheckEnergyStatus();

            float energyLevelVal = TreeStats.EnergyLevel.Value;
            float waterLevelVal = TreeStats.WaterLevel.Value;

            _gameUI.LevelIndicators.UpdateEnergyLevel(energyLevelVal / TreeStats.MaxEnergyLevel.Value, energyLevelVal);
            _gameUI.LevelIndicators.UpdateWaterLevel(waterLevelVal / TreeStats.MaxWaterLevel.Value, waterLevelVal);


            float waterGain = TreeStats.WaterAbsorbtionRate.Value;
            float costOfLiving = EnergyCostOfLiving.GetTotalCost(_rooController.RootCount);

            _gameUI.TreeStatsUI.UpdateAllText(energyCollected, waterGain, costOfLiving);

            // _gameUI.TreeStatsUI.UpdateAllText(TreeStats.
        }


        _gameTicker.OnTick += InvokeLifeCycleEvents;

        _gameTicker.ToggleTicker(false);

        ToggleLoop(false);
    }

    void Start()
    {
        // _gameUI.GameTimeText.gameObject.SetActive(false);

    }

    void Update()
    {
        _gameUI.GameTimeText.text = _timeManager.GetCurrentTimeIn24HourFormat();

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

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            GameManager.instance.RestartGame(true);
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

    public void StartGameLoop()
    {
        TreeStats.EnergyLevel.Set(TreeStats.MaxEnergyLevel.Value * 0.82f);
        TreeStats.WaterLevel.Set(TreeStats.MaxWaterLevel.Value * 0.0f);

        _gameUI.DeathCountDownController.ToggleDeathCountdown(false);

        SeasonTimer seasonTimer = FindObjectOfType<SeasonTimer>();
        seasonTimer.StartSeasonTimer();

        _timeManager.enabled = true;

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


    public void EnterBuildMode()
    {
        _rooController.ToggleRootInteractables(true);
    }


    public void ExitBuildMode()
    {
        _rooController.ToggleRootInteractables(false);
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

    private float ConvertWaterToEnergy()
    {
        if (!TreeStats.SufficentWaterForEnergyConversion() || TreeStats.IsEnergyFull())
        {
            if (TreeStats.IsEnergyFull())
            {
                Debug.Log("Energy Full");
            }
            else if (!TreeStats.SufficentWaterForEnergyConversion())
            {
                // Debug.Log("Not Enough Water");
            }

            return 0.0f;
        }

        float waterToUse = TreeStats.WaterAmountForEnergyConversion.Value;
        float energyCollected = TreeStats.WaterToEnergyLogic.ConvertWaterToEnergy(waterToUse);

        TreeStats.WaterLevel.Consume(waterToUse);
        TreeStats.EnergyLevel.Add(energyCollected);

        return energyCollected;
    }

    private void ConsumeEnergyCostOfLiving()
    {
        float cost = EnergyCostOfLiving.GetTotalCost(_rooController.RootCount);
        TreeStats.EnergyLevel.Consume(cost);
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

    public float RootGrowthDuration { get; set; }


    public TreeStats()
    {
        WaterToEnergyLogic = new WaterToEnergyLogic();

        EnergyLevel = new(0);
        WaterLevel = new(0);
        MaxEnergyLevel = new(AbilityType.MaxEnergyLevel, new(new float[] { 50, 100, 150 }));
        MaxWaterLevel = new(AbilityType.MaxWaterLevel, new(new float[] { 100, 150, 200 }));

        RootGrowthDuration = 5.0f;

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
        UpgradableAbility = new(AbilityType.IncreaseWaterConversionRate, new(new float[] { 1.5f, 2.0f, 3.5f })); //Percentage
        _waterToEnergyRate = UpgradableAbility.Value;
    }

    public float ConvertWaterToEnergy(float waterAmount)
    {
        return waterAmount * _waterToEnergyRate;
    }

    public void UpdateWaterToEnergyRate()
    {
        UpgradableAbility.Upgrade();
        _waterToEnergyRate = UpgradableAbility.Value;
    }
}




[System.Serializable]
public class EnergyCostOfLiving
{
    [SerializeField] public float RootCost { get; private set; } = 0.05f;
    [SerializeField] public float JustLivingCost { get; private set; } = 0.25f;


    public float GetTotalCost(int rootCount)
    {
        return JustLivingCost + (rootCount * RootCost);
    }

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
    public float Value => _data.ValueLevels[_data.CurrentLevel];

    public UpgradableAbility(AbilityType type, UpgradeData data)
    {
        Type = type;
        _data = data;
    }

    public float GetUpgradeCost() => _baseUpgradeCost * (_data.CurrentLevel + 1);

    public string GetUpgradeInfo() => _data.GetUpgradeInfo();

    public bool IsMaxLevel() => _data.IsMaxLevel();

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

