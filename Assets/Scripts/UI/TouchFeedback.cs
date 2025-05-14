using UnityEngine;
using UnityEngine.InputSystem;

public class TouchFeedback : MonoBehaviour
{
    [SerializeField] private AudioClip tapSound;
    [SerializeField] private GameObject touchEffectPrefab;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (Touchscreen.current == null)
            return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            // 1. 소리 재생
            if (tapSound != null)
                audioSource.PlayOneShot(tapSound);

            // 2. 화면에 이펙트 생성 (월드 스페이스가 아닌 화면 기준)
            Vector2 screenPos = Touchscreen.current.primaryTouch.position.ReadValue();

            if (touchEffectPrefab != null)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0.5f));
                Instantiate(touchEffectPrefab, worldPos, Quaternion.identity);
            }
        }
    }
}