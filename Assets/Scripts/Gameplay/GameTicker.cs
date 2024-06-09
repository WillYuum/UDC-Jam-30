using System;
using UnityEngine;

public class GameTicker : MonoBehaviour
{
    [SerializeField] private int _tickRatePerSecond = 1;
    [HideInInspector] public float GameTime { get; private set; }

    [field: SerializeField] public float FastForwardMultiplier { get; private set; } = 2f;

    public event Action OnTick;

    private float _normalTickDuration;
    private float _fastTickDuration;
    private float _timeTillNextTick;

    public bool IsFastMode { get; private set; } = false;

    public float TickDuration => IsFastMode ? _fastTickDuration : _normalTickDuration;

    void Start()
    {
        _normalTickDuration = 1f / _tickRatePerSecond;
        _fastTickDuration = _normalTickDuration / FastForwardMultiplier;
        _timeTillNextTick = _normalTickDuration;

        GameTime = 0f;
    }

    void Update()
    {
        HandleModeSwitch();

        _timeTillNextTick -= Time.deltaTime;

        if (_timeTillNextTick <= 0)
        {
            InvokeTick();
        }

        GameTime += IsFastMode ? Time.deltaTime * FastForwardMultiplier : Time.deltaTime;
    }

    private void HandleModeSwitch()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (!IsFastMode)
            {
                SetFastMode(true);
            }
        }
        else
        {
            if (IsFastMode)
            {
                SetFastMode(false);
            }
        }
    }

    private void SetFastMode(bool isFast)
    {
        IsFastMode = isFast;
        _timeTillNextTick = IsFastMode ? _fastTickDuration : _normalTickDuration;
    }

    private void InvokeTick()
    {
        OnTick?.Invoke();
        _timeTillNextTick = IsFastMode ? _fastTickDuration : _normalTickDuration;
    }

    public void ToggleTicker(bool active)
    {
        enabled = active;
    }
}
