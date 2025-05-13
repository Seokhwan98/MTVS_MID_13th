using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static Const;

public static class WebApiClient
{
    public static IEnumerator Get<T>(string endpoint, Action<T> onSuccess)
    {
        using var request = UnityWebRequest.Get($"{BASE_URL}/{endpoint}");
        request.SetRequestHeader("Content-Type", "text/plain");
        
        Debug.Log($"요청시작: {request.url}");
        
        yield return request.SendWebRequest();

        Debug.Log($"요청 끝");
        
        if (WebApiErrorHandler.Check(request)) yield break;

        try
        {
            var response = ApiResponse<T>.Parse(request.downloadHandler.text);
            if (!response.status) Debug.LogWarning($"API 실패: {response.message}");
            else onSuccess?.Invoke(typeof(T) == typeof(string) ? (T)(object)response.message : response.data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Parse Error] {ex.Message}");
        }
    }
    
    public static IEnumerator PostForm<T>(string endpoint, WWWForm form, Action<T> onSuccess)
    {
        var request = UnityWebRequest.Post($"{BASE_URL}/{endpoint}", form);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (WebApiErrorHandler.Check(request)) yield break;

        try
        {
            var response = ApiResponse<T>.Parse(request.downloadHandler.text);
            if (!response.status) Debug.LogWarning($"요청 실패: {response.message}");
            else onSuccess?.Invoke(response.data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Parse Error] {ex.Message}");
        }
    }

    public static IEnumerator Post<T>(string endpoint, string json, Action<T> onSuccess)
    {
        var request = new UnityWebRequest($"{BASE_URL}/{endpoint}", "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (WebApiErrorHandler.Check(request)) yield break;

        try
        {
            var response = ApiResponse<T>.Parse(request.downloadHandler.text);
            if (!response.status) Debug.LogWarning($"API 실패: {response.message}");
            else onSuccess?.Invoke(typeof(T) == typeof(string) ? (T)(object)response.message : response.data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Parse Error] {ex.Message}");
        }
    }

    public static IEnumerator UploadFile<T>(string endpoint, byte[] data, string filename, string mimeType, Action<T> onSuccess)
    {
        var form = new WWWForm();
        form.AddBinaryData("file", data, filename, mimeType);

        var request = UnityWebRequest.Post($"{BASE_URL}/{endpoint}", form);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (WebApiErrorHandler.Check(request)) yield break;

        try
        {
            var response = ApiResponse<T>.Parse(request.downloadHandler.text);
            if (!response.status) Debug.LogWarning($"업로드 실패: {response.message}");
            else onSuccess?.Invoke(response.data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Parse Error] {ex.Message}");
        }
    }

    public static IEnumerator DownloadFile<T>(string filename, Action<T> onSuccess)
    {
        string url = $"{BASE_URL}/get-file/{UnityWebRequest.EscapeURL(filename)}";

        UnityWebRequest request = typeof(T) == typeof(Sprite)
            ? UnityWebRequestTexture.GetTexture(url)
            : UnityWebRequestMultimedia.GetAudioClip(url, WebApiUtility.GetAudioType(filename));

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"파일 다운로드 실패: {request.error}");
            onSuccess?.Invoke(default);
            yield break;
        }

        if (typeof(T) == typeof(Sprite))
        {
            var tex = DownloadHandlerTexture.GetContent(request);
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            onSuccess?.Invoke((T)(object)sprite);
        }
        else if (typeof(T) == typeof(AudioClip))
        {
            var clip = DownloadHandlerAudioClip.GetContent(request);
            onSuccess?.Invoke((T)(object)clip);
        }
    }
}
