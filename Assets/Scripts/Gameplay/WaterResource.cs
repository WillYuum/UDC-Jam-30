using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaterResource : MonoBehaviour
{
    [field: SerializeField] public float WaterAmount { get; private set; }

    [SerializeField] private TextMeshProUGUI _waterAmountText;


    void Start()
    {
#if UNITY_EDITOR
        if (_waterAmountText == null)
        {
            Debug.LogError("WaterAmountText is not assigned in WaterResource");
        }
        else if (WaterAmount <= 0)
        {
            Debug.LogError("WaterAmount is not assigned in WaterResource");
        }
#endif

        ToggleWaterAmountText(false);
    }

    public void DissovleWater(float amount)
    {
        WaterAmount -= amount;
    }

    private void ToggleWaterAmountText(bool isActive)
    {
        _waterAmountText.gameObject.SetActive(isActive);
    }


    private void OnMouseOver()
    {
        ToggleWaterAmountText(true);
        _waterAmountText.text = WaterAmount.ToString();
    }

    private void OnMouseExit()
    {
        ToggleWaterAmountText(false);
    }
}
