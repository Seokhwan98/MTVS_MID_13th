using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "New Item/신규플레이어")]
public class PlayerInfo : ScriptableObject
{
    public string playerName;
    public string playerID;
    public string playerPassword;
}
