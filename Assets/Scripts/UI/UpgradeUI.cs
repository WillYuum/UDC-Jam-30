using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _cost;


    void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(() => OnClicked?.Invoke());
    }

    void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public struct UpgradeInfo
    {
        public string Title;
        public string Description;
        public float Cost;
        public Action OnClicked;
        public Action GetInfoDescription;
        public Action GetCost;
    }

    private event Action OnClicked;
    public void SetUpgradeInfo(string title, string description, float cost)
    {
        _title.text = title;
        _description.text = description;
        _cost.text = cost.ToString();
    }

    public void SetUpOnClickedEvent(System.Action onClicked)
    {
        OnClicked += onClicked;
    }

    public void HandleOnUpdate(UpgradeInfo upgradeInfo)
    {

        // SetUpgradeInfo(upgradeInfo.Title, upgradeInfo.Description, upgradeInfo.Cost);
        // SetUpOnClickedEvent(upgradeInfo.OnClicked);
    }


}
