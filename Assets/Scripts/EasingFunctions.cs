
using UnityEngine;

public static class EasingFunctions
{
    public static float EaseInQuart(float t)
    {
        return t * t * t * t;
    }

    public static float EaseInExpo(float x)
    {
        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    }
}