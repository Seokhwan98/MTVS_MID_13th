using System;

public static class FishTouchEvents
{
    public static event Action<string> OnFishTouched;

    public static void InvokeFishTouched(string name)
    {
        OnFishTouched?.Invoke(name);
    }
}