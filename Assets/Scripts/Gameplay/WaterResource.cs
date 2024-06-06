using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaterResource : MonoBehaviour
{
    [field: SerializeField] public float Amount { get; private set; }

    [SerializeField] private TextMeshProUGUI _waterAmountText;


    void Start()
    {
#if UNITY_EDITOR
        if (_waterAmountText == null)
        {
            Debug.LogError("WaterAmountText is not assigned in WaterResource");
        }
        else if (Amount <= 0)
        {
            Debug.LogError("WaterAmount is not assigned in WaterResource");
        }
#endif

        ToggleWaterAmountText(false);
    }

    public void DissovleWater(float amount)
    {
        Amount -= amount;
    }


    private void ToggleWaterAmountText(bool isActive)
    {
        _waterAmountText.gameObject.SetActive(isActive);
    }


    private void OnMouseOver()
    {
        ToggleWaterAmountText(true);
        _waterAmountText.text = Amount.ToString();
    }

    private void OnMouseExit()
    {
        ToggleWaterAmountText(false);
    }
}
