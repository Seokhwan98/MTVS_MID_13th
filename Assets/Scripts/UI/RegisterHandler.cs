using TMPro;
using UnityEngine;

public class RegisterHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField passwordInput;

    public void OnClickRegister()
    {
        // 입력값 가져오기
        string name = nameInput.text;
        string id = idInput.text;
        string password = passwordInput.text;

        // ScriptableObject 생성 (런타임 메모리용)
        PlayerInfo info = ScriptableObject.CreateInstance<PlayerInfo>();
        info.playerName = name;
        info.playerID = id;
        info.playerPassword = password;

        // Debug.Log($"등록됨: {info.playerName}, {info.playerID}, {info.playerPassword}");

        // 이후 사용 예시: UI에 반영하거나 저장 로직 연결
        PlayerInfoManager.Instance.AddPlayerInfo(info);
    }
}