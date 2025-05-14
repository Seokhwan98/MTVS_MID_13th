using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FishNameDisplayUI : MonoBehaviour
{
    [SerializeField] private SoundSoureceControl wrongSound;
    [SerializeField] private SoundSoureceControl rightSound;
    [SerializeField] private TMP_Text wrongMark;
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private ChangeQuizData quizData;
    [SerializeField] private GameObject nextUIPanel;
    
    private bool inGame = false;

    private void Start()
    {
        inGame = false;
    }

    private void OnEnable()
    {
        FishTouchEvents.OnFishTouched += HandleFishTouched;
    }

    private void OnDisable()
    {
        FishTouchEvents.OnFishTouched -= HandleFishTouched;
    }

    private void HandleFishTouched(string fishName)
    {
        if (!inGame)
        {
            fishNameText.text = fishName;
        }
        else
        {
            // 여기서 fishNameText.text는 지금 퀴즈에서 저장된 이름
            // 그리고 터치될때마다 비교하고 결과 판단하기
            string answer = quizData.CurrentAnswerName;
            
            Debug.Log(answer);

            if (fishName == answer)
            {
                // 정답일 경우는 다음 ui로 가도록 하기
                StartCoroutine(ShowRightMark());
            }
            else
            {
                // 오답일 경우는 화면에 x 잠깐 출력하고 지우기
                StartCoroutine(ShowWrongMark());
            }
        }
    }
    
    private IEnumerator ShowWrongMark()
    {
        wrongSound.Play();
        wrongMark.text = "X";
        wrongMark.color = Color.red;
        yield return new WaitForSeconds(1f); // 1초 표시
        wrongMark.text = "";
    }
    
    private IEnumerator ShowRightMark()
    {
        rightSound.Play();
        fishNameText.text = "";
        wrongMark.text = "O";
        wrongMark.color = Color.green;
        yield return new WaitForSeconds(1f); // 1초 표시
        wrongMark.text = "";
        nextUIPanel.SetActive(true);
        inGame = false;
    }
    
    public void SetInGameTrue()
    {
        inGame = true;
    }

    public void SetFishNameText()
    {
        fishNameText.text = quizData.CurrentAnswerName;
    }
}