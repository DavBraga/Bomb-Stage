using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UnityAction<int> onChangeLevel;
    public UnityAction onEndLevel;
    public UnityAction onGameWon, onGameLost;
    public static GameManager Instance{get; private set;}
    public int highestScore;

    public enum GameStates{Active, Paused, GameOver }

    public GameStates currentState = GameStates.Active; 
    bool musicOn = true;
    public LevelManager currentLevel{get; private set;}
    // Start is called before the first frame update
    [Tooltip("DONT CHANGE IT. For Inspection only.")]
    [SerializeField] string currentLevelName;
    [SerializeField] AudioClip gameOverTheme;
    [SerializeField] AudioClip gameWonTheme;

    private void Awake() {
        if(Instance && Instance!=this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndLevel(bool isVictory = false)
    {
        onEndLevel?.Invoke();
        if(currentState == GameStates.GameOver) return;
        currentLevel.GetAudioSource().Stop();
        if (!isVictory)
            GameOver();
        else
            GameWon(); 
    }

    private void GameOver()
    {
        onGameLost?.Invoke();
        currentLevel.GetAudioSource().PlayOneShot(gameOverTheme);
        currentState = GameStates.GameOver;
        StartCoroutine(DelayedReloadGame(5f));
    }

    private void GameWon()
    {
        onGameWon?.Invoke();
        currentLevel.GetAudioSource().PlayOneShot(gameWonTheme);
        TrySaveScore(currentLevel.GetLevelName(), currentLevel.GetCurrentScore());
        currentState = GameStates.GameOver;
    }

    IEnumerator DelayedReloadGame(float delay=5f)
    {
        yield return new WaitForSeconds(delay);
        ReloadLevel();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().name);
    }
    public void SetCurrentLevel(LevelManager levelManager)
    {
        this.currentLevel = levelManager;
        currentLevelName = levelManager.GetLevelName();
        onChangeLevel?.Invoke(GetHighestScore());
    }

    public bool TrySaveScore(string stageName, int value)
    {
        int highestScore = PlayerPrefs.GetInt(stageName);
        if(value> highestScore)
        {
            PlayerPrefs.SetInt(stageName, value);
            return true;
        }
        return false;
        
    }

    public int GetHighestScore()
    {
        return PlayerPrefs.GetInt(currentLevelName);
    }
    private void OnDestroy() {
        if(Instance ==this) Instance = null;
    }
}
