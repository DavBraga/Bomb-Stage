using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI score,highestscore;
    [SerializeField] GameObject endScreen;

    private void Start() {
        GameManager.Instance.onChangeLevel += CleanUP;
        GameManager.Instance.onChangeLevel += BindHUD;
        GameManager.Instance.onChangeLevel += UpdateHighScore;
        
    }
    public void BindHUD(int binder)
    {
        GameManager.Instance.onChangeLevel -= BindHUD;
        GameManager.Instance.currentLevel.onScorePoints += UpdateScore;
        GameManager.Instance.onGameWon +=ShowEndScreen;
    }
    public void UpdateScore(int value)
    {
        string textValue = HandleIntValues(value);
        score.SetText(textValue);
    }


    public void UpdateHighScore(int value)
    {
        string textValue = HandleIntValues(value);
        highestscore.SetText(textValue);
    }

    private static string HandleIntValues(int value)
    {
        string textValue = value.ToString();
        if (value < 100) textValue = "0" + textValue;
        if (value < 10) textValue = "0" + textValue;
        return textValue;
    }

    public void CleanUP(int binder)
    {
        GameManager.Instance.onChangeLevel -= UpdateHighScore;
        GameManager.Instance.onGameWon -=ShowEndScreen;
        GameManager.Instance.onChangeLevel -= CleanUP;
        
    }
    public void ShowEndScreen()
    {
        StartCoroutine(DelayedEndScreen());
    }
    IEnumerator DelayedEndScreen()
    {
        yield return new WaitForSeconds(1f);
        endScreen.SetActive(true);
        
    }

    public void ReloadLevel()
    {
        GameManager.Instance?.ReloadLevel();
    }
}
