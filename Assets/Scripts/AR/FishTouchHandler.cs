using UnityEngine;

public class FishTouchHandler : MonoBehaviour
{
    // 예시: 터치되면 로그 출력
    public void OnTouched()
    {
        Debug.Log($"[터치됨] {gameObject.name}");
        FishTouchEvents.InvokeFishTouched(gameObject.name);
        // 여기에 이펙트 재생, 반응, 사라짐 등 추가 가능
    }
}