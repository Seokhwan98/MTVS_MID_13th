using UnityEngine;

[CreateAssetMenu(fileName = "NewFishData", menuName = "AR/Fish Data", order = 0)]
public class FishData : ScriptableObject
{
    public GameObject prefab;
    
    [Header("서식 지역")]
    public int habitat; // 1은 열대 바다, 2는 온대 바다
    
    [Header("기본 정보")]
    public string englishName;
    public string koreanName;

    [Header("행동")]
    [TextArea(2, 3)] public string behavior;
    
    [Header("기타 정보")]
    [TextArea(2, 4)] public string etc;
    
    [Header("특징")]
    [TextArea(3, 5)] public string characteristics;
    

    [Header("먹이")]
    [TextArea(2, 3)] public string food;


}