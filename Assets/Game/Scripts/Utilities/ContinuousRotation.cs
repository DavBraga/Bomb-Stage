using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeedInDeegrees = 15f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.currentState!= GameManager.GameStates.Active) return;
        transform.Rotate(0,rotationSpeedInDeegrees*Time.deltaTime,0);
        
    }
}
