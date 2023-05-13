using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public UnityAction<int> onScorePoints;
    float currentScore=0;
    [SerializeField]int maxScore = 0;
    [SerializeField] string levelName;

    [SerializeField]GameObject gameManagerPrefab;
    [SerializeField] AudioSource levelMusic;

    [SerializeField] int musicVolume=0;
    private void Awake() {
        if(!GameManager.Instance) 
            Instantiate(gameManagerPrefab); 
        levelMusic = GetComponent<AudioSource>();
        musicVolume = PlayerPrefs.GetInt("musicVol");
    }
    // Start is called before the first frame update
    void Start()
    {
        if(musicVolume<1) TurnMusicON();
        else TurnMusicOff();
        GameManager.Instance.SetCurrentLevel(this);
        maxScore =PlayerPrefs.GetInt(levelName);
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance) return;
        if(GameManager.Instance.currentState != GameManager.GameStates.Active) return;
        currentScore+= Time.deltaTime; 
        onScorePoints?.Invoke(GetCurrentScore());     
    }

    public string GetLevelName()
    {
        return levelName;
    }

    public AudioSource GetAudioSource()
    {
        return levelMusic;
    }
    public int GetCurrentScore()
    {
        return Mathf.FloorToInt(currentScore);
    }

    public void TurnMusicON()
    {
        levelMusic.Play();
    }
    public void TurnMusicOff()
    {
        levelMusic.Stop();
    }

    public void TogleMusic()
    {
        if(musicVolume>0)
        {
            musicVolume = 0;
            PlayerPrefs.SetInt("musicVol",musicVolume);
            
            TurnMusicON();
        }
        else
        {
            musicVolume =100;
            PlayerPrefs.SetInt("musicVol",musicVolume);
            TurnMusicOff();
        } 
    }
}
