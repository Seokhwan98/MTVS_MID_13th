using System;
using UnityEngine;
using Newtonsoft.Json;

public class WebAiManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public static string UserId => PlayerPrefs.GetString("user_id", "");

    #region 로그인

    [Serializable]
    public class LoginRequest { public string nickname; }
    [Serializable]
    public class LoginResponse { public string user_id; }

    public void Login(string nickname, Action<string> onSuccess = null)
    {
        var request = new LoginRequest { nickname = nickname };
        string json = JsonConvert.SerializeObject(request);

        StartCoroutine(WebApiClient.Post<LoginResponse>("login", json, data =>
        {
            PlayerPrefs.SetString("user_id", data.user_id);
            Debug.Log($"[로그인 성공] user_id: {data.user_id}");
            onSuccess?.Invoke(data.user_id);
        }));
    }

    #endregion

    #region 질문 요청 (GPT)

    [Serializable]
    public class AskRequest
    {
        public string user_id;
        public string flower;
        public string question;
    }

    [Serializable]
    public class AskResponse
    {
        public string answer;
        public string source;
    }

    public void AskQuestion(string flower, string question, Action<string> onAnswer = null)
    {
        if (string.IsNullOrEmpty(UserId))
        {
            Debug.LogWarning("로그인이 되어 있지 않습니다.");
            return;
        }

        var request = new AskRequest
        {
            user_id = UserId,
            flower = flower,
            question = question
        };
        string json = JsonConvert.SerializeObject(request);

        StartCoroutine(WebApiClient.Post<AskResponse>("ask", json, data =>
        {
            Debug.Log($"[AI 응답] {data.answer} (출처: {data.source})");
            onAnswer?.Invoke(data.answer);
            RequestTTS(data.answer); // 음성도 바로 출력
        }));
    }

    #endregion

    #region TTS 요청

    [Serializable]
    public class TTSRequest { public string text; }
    [Serializable]
    public class TTSResponse { public string audio_url; }

    public void RequestTTS(string text)
    {
        var request = new TTSRequest { text = text };
        string json = JsonConvert.SerializeObject(request);

        StartCoroutine(WebApiClient.Post<TTSResponse>("tts", json, data =>
        {
            Debug.Log($"[TTS 재생 요청] {data.audio_url}");
            StartCoroutine(WebApiClient.DownloadFile<AudioClip>(data.audio_url, clip =>
            {
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("TTS 음성 다운로드 실패");
                }
            }));
        }));
    }

    #endregion
}
