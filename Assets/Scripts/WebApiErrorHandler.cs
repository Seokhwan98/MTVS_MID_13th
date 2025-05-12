using UnityEngine;
using UnityEngine.Networking;

public static class WebApiErrorHandler
{
    public static bool Check(UnityWebRequest req)
    {
        switch (req.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                Debug.LogWarning("서버 연결 오류");
                return true;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogWarning($"HTTP 오류: {req.responseCode}");
                return true;
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogWarning("데이터 처리 오류");
                return true;
            default:
                return false;
        }
    }
}