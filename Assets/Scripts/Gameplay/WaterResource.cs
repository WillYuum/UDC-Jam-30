using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class WaterResource : MonoBehaviour
{
    [SerializeField] public float StartingAmount = 100;
    [HideInInspector] public float CurrentAmount { get; private set; }

    [SerializeField] private TextMeshProUGUI _waterAmountText;


    void Start()
    {
#if UNITY_EDITOR
        if (_waterAmountText == null)
        {
            Debug.LogError("WaterAmountText is not assigned in WaterResource");
        }
#endif

        ToggleWaterAmountText(false);
        CurrentAmount = StartingAmount;
    }

    private bool IsEmpty()
    {
        return CurrentAmount < 1.0;
    }

    public bool DissovleWater(float amount)
    {
        CurrentAmount -= amount;

        float scaleRation = CurrentAmount / StartingAmount;
        Vector3 newScale = transform.localScale * scaleRation;
        //scale down the object 
        transform.DOScale(newScale, 0.5f);

        bool isEmpty = IsEmpty();

        if (isEmpty)
        {
            DOTween.Kill(transform);
            Destroy(gameObject);
        }

        return isEmpty;
    }


    private void ToggleWaterAmountText(bool isActive)
    {
        _waterAmountText.gameObject.SetActive(isActive);
    }


    private void OnMouseOver()
    {
        ToggleWaterAmountText(true);
        _waterAmountText.text = CurrentAmount.ToString();
    }

    private void OnMouseExit()
    {
        ToggleWaterAmountText(false);
    }
}
