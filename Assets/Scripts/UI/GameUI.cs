using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] public GameObject MainUI;
    [SerializeField] public TextMeshProUGUI GameTimeText;
    [SerializeField] public LevelIndicators LevelIndicators;
    [SerializeField] public IntroScreen IntroScreen;

    [SerializeField] public LoseScreen LoseScreen;
    [SerializeField] public WinScreen WinScreen;

    [SerializeField] private GameObject _upgradeScreen;
    [SerializeField] private Button _buttonToUpgradeScreen;



    void Awake()
    {
        _upgradeScreen.SetActive(false);
        _buttonToUpgradeScreen.onClick.AddListener(() =>
        {
            _upgradeScreen.SetActive(true);

            GameTicker gameTicker = FindObjectOfType<GameTicker>();
            gameTicker.ToggleTicker(false);

            GetDataAndRenderUpgrades();
        });
    }


    private void GetDataAndRenderUpgrades()
    {
        GameloopManager gameloopManager = FindObjectOfType<GameloopManager>();


        bool CanUpgrade(float cost) => gameloopManager.TreeStats.EnergyLevel >= cost;

        void UpdateLevelIndicators()
        {
            TreeStats treeStats = gameloopManager.TreeStats;
            LevelIndicators.UpdateEnergyLevel(treeStats.EnergyLevel / treeStats.MaxEnergyLevel.Value, treeStats.EnergyLevel);
            LevelIndicators.UpdateWaterLevel(treeStats.WaterLevel / treeStats.MaxWaterLevel.Value, treeStats.WaterLevel);
        }

        TreeStats treeStats = gameloopManager.TreeStats;
        DisplayUpgradeUICards(new UpgradeUI.UpgradeInfo[] {
                new() {
                    Title = "Max Energy Level",
                    Description = treeStats.MaxEnergyLevel.GetUpgradeInfo(),
                    Cost = treeStats.MaxEnergyLevel.GetUpgradeCost(),
                    OnClicked = () =>
                    {
                        if(CanUpgrade(treeStats.MaxEnergyLevel.GetUpgradeCost())){
                        treeStats.EnergyLevel -= treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost();
                            treeStats.MaxEnergyLevel.Upgrade();
                            UpdateLevelIndicators();
                        }
                    },

                },
                new()
                {
                    Title = "Max Water Level",
                    Description = treeStats.MaxWaterLevel.GetUpgradeInfo(),
                    Cost = treeStats.MaxWaterLevel.GetUpgradeCost(),
                    OnClicked = () =>
                    {
                        if(CanUpgrade(treeStats.MaxWaterLevel.GetUpgradeCost()))
                        treeStats.EnergyLevel -= treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost();
                            treeStats.MaxWaterLevel.Upgrade();
                            UpdateLevelIndicators();
                    }
                },
                new()
                {
                    Title = "Water to energy",
                    Description = treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeInfo(),
                    Cost = treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost(),
                    OnClicked = () =>
                    {
                        if(CanUpgrade(treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost()))
                        treeStats.EnergyLevel -= treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost();
                            treeStats.WaterToEnergyLogic.UpgradableAbility.Upgrade();
                            UpdateLevelIndicators();
                    }
                }

            });
    }


    public void PlayGameIntro(Action onEnd)
    {
        MainUI.SetActive(false);
        IntroScreen.gameObject.SetActive(true);
        IntroScreen.FadeInScreen(onEnd);
    }


    [SerializeField] private GameObject _upgradeUIPrefab;
    [SerializeField] private GameObject _upgradeUIHolder;
    public void DisplayUpgradeUICards(UpgradeUI.UpgradeInfo[] upgradeInfos)
    {
        //destroy children
        foreach (Transform child in _upgradeUIHolder.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (UpgradeUI.UpgradeInfo upgradeInfo in upgradeInfos)
        {
            GameObject upgradeUI = Instantiate(_upgradeUIPrefab, _upgradeUIHolder.transform);
            UpgradeUI upgradeUIComponent = upgradeUI.GetComponent<UpgradeUI>();
            upgradeUIComponent.SetUpgradeInfo(upgradeInfo.Title, upgradeInfo.Description, upgradeInfo.Cost);
            upgradeUIComponent.SetUpOnClickedEvent(() =>
            {
                upgradeInfo.OnClicked();
                GetDataAndRenderUpgrades();
            });
        }
    }
}
