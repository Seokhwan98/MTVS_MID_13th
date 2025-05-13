using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager Instance { get; private set; }

    private readonly List<PlayerInfo> playerInfos = new List<PlayerInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// PlayerInfo를 리스트에 추가
    /// </summary>
    public void AddPlayerInfo(PlayerInfo info)
    {
        playerInfos.Add(info);
        Debug.Log($"Player 등록됨: {info.playerName}, 총 등록 수: {playerInfos.Count}");
    }

    /// <summary>
    /// 모든 등록된 PlayerInfo 반환
    /// </summary>
    public List<PlayerInfo> GetAllPlayerInfos()
    {
        return new List<PlayerInfo>(playerInfos); // 복사본 반환
    }

    /// <summary>
    /// 전체 초기화
    /// </summary>
    public void ClearAll()
    {
        playerInfos.Clear();
    }
}