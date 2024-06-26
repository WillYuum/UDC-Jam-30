using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Net.NetworkInformation;

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

    [field: SerializeField] public TreeStatsUI TreeStatsUI { get; private set; }

    [field: SerializeField] public DeathCountDownController DeathCountDownController { get; private set; }



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


        bool CanUpgrade(UpgradableAbility upgradableAbility, float cost)
        {
            return gameloopManager.TreeStats.EnergyLevel.Value >= cost && upgradableAbility.IsMaxLevel() == false;
        }


        void UpdateLevelIndicators()
        {
            TreeStats treeStats = gameloopManager.TreeStats;

            float eneryLevlVal = treeStats.EnergyLevel.Value;
            float waterLevelVal = treeStats.WaterLevel.Value;

            LevelIndicators.UpdateEnergyLevel(eneryLevlVal / treeStats.MaxEnergyLevel.Value, eneryLevlVal);
            LevelIndicators.UpdateWaterLevel(waterLevelVal / treeStats.MaxWaterLevel.Value, waterLevelVal);
        }

        TreeStats treeStats = gameloopManager.TreeStats;
        DisplayUpgradeUICards(new UpgradeUI.UpgradeInfo[] {
                new() {
                    Title = "Max Energy Level",
                    Description = treeStats.MaxEnergyLevel.GetUpgradeInfo(),
                    Cost = treeStats.MaxEnergyLevel.GetUpgradeCost(),
                    OnClicked = () =>
                    {
                        if(CanUpgrade(treeStats.MaxEnergyLevel, treeStats.MaxEnergyLevel.GetUpgradeCost())){
                        treeStats.EnergyLevel.Consume(treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost());
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
                        if(CanUpgrade(treeStats.MaxWaterLevel, treeStats.MaxWaterLevel.GetUpgradeCost())){
                        treeStats.EnergyLevel.Consume(treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost());
                            treeStats.MaxWaterLevel.Upgrade();
                            UpdateLevelIndicators();

                        }
                    }
                },
                new()
                {
                    Title = "Water to energy",
                    Description = treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeInfo(),
                    Cost = treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost(),
                    OnClicked = () =>
                    {
                        if(CanUpgrade(treeStats.WaterToEnergyLogic.UpgradableAbility, treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost())){

                        treeStats.EnergyLevel.Consume(treeStats.WaterToEnergyLogic.UpgradableAbility.GetUpgradeCost());
                            treeStats.WaterToEnergyLogic.UpdateWaterToEnergyRate();
                            UpdateLevelIndicators();
                        }
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



[System.Serializable]
public class DeathCountDownController
{
    [field: SerializeField] public TextMeshProUGUI DeathCountdownText { get; private set; }
    [field: SerializeField] public GameObject Holder { get; private set; }


    public void UpdateDeathCountdown(float countdown)
    {
        DeathCountdownText.text = countdown.ToString("00.0");
    }

    public void ToggleDeathCountdown(bool value, bool tween = false)
    {
        Holder.SetActive(value);

        if (value)
        {
            RectTransform rectTransform = Holder.GetComponent<RectTransform>();

            Vector3 originalPosition = rectTransform.anchoredPosition;

            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);


            rectTransform.anchoredPosition = new Vector3(originalPosition.x, 0, originalPosition.z);
            rectTransform.localScale = new Vector3(0, 0, 0);
            rectTransform.DOScale(1, 1.5f).OnComplete(() =>
            {
                rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMin = new Vector2(0.5f, 1.0f);
                rectTransform.anchoredPosition = new Vector3(originalPosition.x, -Screen.height / 2, originalPosition.z);
                rectTransform.DOAnchorPosY(-11, 0.75f).SetDelay(0.15f).OnComplete(() =>
                {
                    rectTransform.anchoredPosition = originalPosition;
                });
            });
        }
    }
}



[System.Serializable]
public class TreeStatsUI
{
    [SerializeField] private TextMeshProUGUI _energyGainText;
    [SerializeField] private TextMeshProUGUI _waterGainText;
    [SerializeField] private TextMeshProUGUI _costOfLivingText;


    public void UpdateAllText(float energyGain, float waterGain, float costOfLiving)
    {
        UpdateText(_energyGainText, "Energy Gain: ", energyGain);
        UpdateText(_waterGainText, "Water Gain: ", waterGain);
        UpdateText(_costOfLivingText, "Cost of Living: ", costOfLiving);
    }


    private void UpdateText(TextMeshProUGUI text, String prefix, float value)
    {
        text.text = prefix + value.ToString();
    }
}