using System;
using UnityEngine;

public static class FishTouchEvents
{
    public static event Action<string> OnFishTouched;
    public static event Action OnWhaleCalled;

    public static void InvokeFishTouched(string name)
    {
        OnFishTouched?.Invoke(name);
    }

    public static void InvokeWhaleCalled()
    {
        OnWhaleCalled?.Invoke();
    }
}