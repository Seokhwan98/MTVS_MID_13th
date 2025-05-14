using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishDatabase : MonoBehaviour
{
    public static FishDatabase Instance { get; private set; }
    
    [SerializeField] private FishData[] fishDataArray;

    private Dictionary<string, FishData> fishByName;
    private Dictionary<string, FishData> fishByEnglishName;
    private Dictionary<int, List<FishData>> fishByHabitat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 제거
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeDatabase();
    }
    
    private void InitializeDatabase()
    {
        fishByName = new Dictionary<string, FishData>();
        fishByEnglishName = new Dictionary<string, FishData>();
        fishByHabitat = new Dictionary<int, List<FishData>>();

        foreach (var fish in fishDataArray)
        {
            fishByName[fish.koreanName] = fish;
            fishByEnglishName[fish.englishName] = fish;

            if (!fishByHabitat.ContainsKey(fish.habitat))
                fishByHabitat[fish.habitat] = new List<FishData>();

            fishByHabitat[fish.habitat].Add(fish);
        }
    }

    /// <summary>
    /// habitat(1: 열대, 2: 온대)에 따라 랜덤한 물고기의 이름 반환
    /// </summary>
    public FishData GetRandomFishNameByHabitat(int habitat)
    {
        if (!fishByHabitat.ContainsKey(habitat) || fishByHabitat[habitat].Count == 0)
            return null;

        var list = fishByHabitat[habitat];
        var fish = list[Random.Range(0, list.Count)];
        return fish;
    }

    /// <summary>
    /// 이름이 주어졌을 때 먹이 정보 반환
    /// </summary>
    public string GetFoodByName(string koreanName)
    {
        return fishByName.TryGetValue(koreanName, out var fish)
            ? fish.food
            : "해당 이름의 물고기를 찾을 수 없습니다.";
    }
    
    public string GetKoreanNameByEnglish(string englishName)
    {
        return fishByEnglishName.TryGetValue(englishName, out var fish)
            ? fish.koreanName
            : "알 수 없음";
    }
}