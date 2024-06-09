using System;
using UnityEngine;

public class GameTicker : MonoBehaviour
{
    private int _tickRatePerSecond = 1;
    private float _timeTillNextTick;
    public bool IsFastMode = false;
    private float _normalTickDuration;
    private float _fastTickDuration;

    [HideInInspector] public float GameTime { get; private set; }

    [field: SerializeField] public float FastForwardMultiplier { get; private set; } = 2f;

    public event Action OnTick;

    void Start()
    {
        _normalTickDuration = 1f / _tickRatePerSecond;
        _fastTickDuration = _normalTickDuration / FastForwardMultiplier;
        _timeTillNextTick = _normalTickDuration;

        GameTime = 0f;
    }

    void Update()
    {
        // Check if the "E" key is being held down
        if (Input.GetKey(KeyCode.E))
        {
            if (!IsFastMode)
            {
                IsFastMode = true;
                _timeTillNextTick = _fastTickDuration;
            }
        }
        else
        {
            if (IsFastMode)
            {
                IsFastMode = false;
                _timeTillNextTick = _normalTickDuration;
            }
        }

        _timeTillNextTick -= Time.deltaTime;

        if (_timeTillNextTick <= 0)
        {
            InvokeTick();
        }

        GameTime += IsFastMode ? Time.deltaTime * FastForwardMultiplier : Time.deltaTime;
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
