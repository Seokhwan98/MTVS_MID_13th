using TMPro;
using UnityEngine;

public class ChangeQuizData : MonoBehaviour
{
    public enum Region
    {
        none = 0,
        hot= 1,
        warm = 2
    }

    private int regionNow;
    private string currentAnswerKoreanName;
    private FishData fishData;
    
    [SerializeField] private Transform quizParent;
    [SerializeField] private TMP_Text koreanNameText;
    [SerializeField] private TMP_Text habitatText;
    [SerializeField] private TMP_Text characteristicsText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text behaviorText;
    [SerializeField] private TMP_Text etcText;

    private GameObject currentInstance;
    
    public void SetRegionNow(int habitat)
    {
        regionNow = habitat;
    }
    
    public void OnClick()
    {
        fishData = FishDatabase.Instance.GetRandomFishNameByHabitat(regionNow);
        
        currentAnswerKoreanName = fishData.koreanName;
        
        koreanNameText.text = fishData.koreanName;
        habitatText.text = ((Region)fishData.habitat == Region.hot) ? "열대 바다" : "온대 바다";
        characteristicsText.text = fishData.characteristics;
        foodText.text = fishData.food;
        behaviorText.text = fishData.behavior;
        etcText.text = fishData.etc;

        fishData.prefab.GetComponent<FreeSwimming>().enabled = false;
        fishData.prefab.GetComponent<FishTouchHandler>().enabled = false;
        currentInstance = Instantiate(fishData.prefab, quizParent);
        currentInstance.transform.localPosition = Vector3.zero;
        currentInstance.transform.localRotation = Quaternion.identity;
        currentInstance.transform.localScale = Vector3.one;
    }

    public void DestroyInstance()
    {
        Destroy(currentInstance);
    }
    
    public bool CheckAnswer(string userInput)
    {
        return userInput == currentAnswerKoreanName;
    }
    
    public string CurrentAnswerName => currentAnswerKoreanName;
}
    
