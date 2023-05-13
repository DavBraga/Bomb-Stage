using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StartGame : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera startCamera;
    [SerializeField] CinemachineVirtualCamera gameCamera;
    [SerializeField] PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.onEndLevel+=GameEnd;
    }

    public void GameStart()
    {
        GameManager.Instance.currentState= GameManager.GameStates.Active; 
        startCamera.Priority = 0;
        gameCamera.Priority =1;
    }
    public void GameEnd()
    {
        startCamera.Priority = 1;
        gameCamera.Priority =0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
