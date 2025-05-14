using TMPro;
using UnityEngine;

public class FishNameDisplayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text fishNameText;

    private void OnEnable()
    {
        FishTouchEvents.OnFishTouched += HandleFishTouched;
    }

    private void OnDisable()
    {
        FishTouchEvents.OnFishTouched -= HandleFishTouched;
    }

    private void HandleFishTouched(string fishName)
    {
        fishNameText.text = $"{fishName}";
    }
}