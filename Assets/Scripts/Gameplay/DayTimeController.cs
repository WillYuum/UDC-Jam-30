using System;
using System.Net.NetworkInformation;
using UnityEngine;

public class DayTimeController : MonoBehaviour
{
    [SerializeField] private Transform _sunTransform;
    [SerializeField] private Transform _moonTransform;
    [SerializeField] private SkyVisual _skyVisual;

    private float _moonSunDistFromCenter = 4.75f;
    private TimeManager _timeManager;

    private void Awake()
    {
        _timeManager = FindObjectOfType<TimeManager>();
        if (_timeManager == null)
        {
            Debug.LogError("TimeManager not found in the scene.");
            enabled = false;
            return;
        }

        _timeManager.OnDayStart += () => HandleChangeDayNightState(TimeManager.DayNightState.Day);
        _timeManager.OnNightStart += () => HandleChangeDayNightState(TimeManager.DayNightState.Night);
    }

    private void OnDestroy()
    {
        if (_timeManager != null)
        {
            _timeManager.OnDayStart -= () => HandleChangeDayNightState(TimeManager.DayNightState.Day);
            _timeManager.OnNightStart -= () => HandleChangeDayNightState(TimeManager.DayNightState.Night);
        }
    }

    public void HandleChangeDayNightState(TimeManager.DayNightState state)
    {
        if (state == TimeManager.DayNightState.Day)
        {
            _sunTransform.gameObject.SetActive(true);
            _moonTransform.gameObject.SetActive(false);
            _sunTransform.position = new Vector2(-_moonSunDistFromCenter, 0f); // Start sun on the left side
        }
        else
        {
            _sunTransform.gameObject.SetActive(false);
            _moonTransform.gameObject.SetActive(true);
            _moonTransform.position = new Vector2(-_moonSunDistFromCenter, 0f); // Start moon on the left side
        }
    }

    private void Update()
    {
        float ratioToComplete = _timeManager.CurrentState == TimeManager.DayNightState.Day ? _timeManager.GetDayTimeRatio() : _timeManager.GetNightTimeRatio();

        Transform activeTransform = _timeManager.GetCurrentState() == TimeManager.DayNightState.Day ? _sunTransform : _moonTransform;
        RotateVisuals(activeTransform, ratioToComplete);

        if (_timeManager.CurrentState == TimeManager.DayNightState.Day)
        {
            _skyVisual.InterplateSkyColorToDark(ratioToComplete);
        }
        else
        {
            _skyVisual.InterplateSkyColorToLight(ratioToComplete);
        }
    }

    private void RotateVisuals(Transform activeTransform, float ratioToComplete)
    {
        Vector2 center = Vector2.zero;
        float angle = ratioToComplete * Mathf.PI; // 180 degrees (π radians) scaled by the ratio

        // Rotate clockwise from left to right
        activeTransform.position = new Vector2(center.x + _moonSunDistFromCenter * -Mathf.Cos(angle),
                                               center.y + _moonSunDistFromCenter * Mathf.Sin(angle));
    }
}


[System.Serializable]
public class SkyVisual
{
    [SerializeField] private SpriteRenderer _skySpriteRenderer;


    private Color lightSkyColor = new Color(1.0f, 1.0f, 1.0f, 1f);
    private Color darkSkyColor = new Color(0.1f, 0.1f, 0.1f, 1f);

    public void InterplateSkyColorToDark(float ratio)
    {
        ratio = EasingFunctions.EaseInExpo(ratio);
        _skySpriteRenderer.color = Color.Lerp(lightSkyColor, darkSkyColor, ratio);
    }

    public void InterplateSkyColorToLight(float ratio)
    {
        ratio = EasingFunctions.EaseInExpo(ratio);
        _skySpriteRenderer.color = Color.Lerp(darkSkyColor, lightSkyColor, ratio);
    }
}