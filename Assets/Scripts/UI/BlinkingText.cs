using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float blinkSpeed = 1f; // 깜빡이는 속도

    private void Update()
    {
        if (text == null) return;

        float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f; // 0~1로 반복
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}