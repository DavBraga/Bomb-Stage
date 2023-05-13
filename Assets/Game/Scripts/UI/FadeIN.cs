using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIN : MonoBehaviour
{
    CanvasGroup canvasGroup;
    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    // Start is called before the first frame updat
    // Update is called once per frame
    void Update()
    {
        if(!canvasGroup) return;
        canvasGroup.alpha += Time.fixedDeltaTime;
        
    }
}
